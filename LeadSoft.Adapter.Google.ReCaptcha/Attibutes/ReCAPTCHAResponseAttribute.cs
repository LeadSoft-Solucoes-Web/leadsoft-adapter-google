using LeadSoft.Adapter.Google.ReCaptcha.Contracts;
using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Exceptions;
using LeadSoft.Common.Library.Extensions;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace LeadSoft.Adapter.Google.ReCaptcha.Attibutes;

/// <summary>
/// Atributo de validação para verificar a resposta do reCAPTCHA v2/v3 do Google.
/// </summary>
/// <remarks>
/// Aplique a propriedades, campos ou parâmetros que recebem o token gerado pelo reCAPTCHA client-side.
/// A <b>Secret Key</b> (chave privada do servidor) pode ser fornecida diretamente no construtor
/// ou via variável de ambiente <c>GOOGLE_RECAPTCHA_SECRET_KEY</c>.
///
/// <para>⚠️ Não confundir com a <i>Site Key</i> (chave pública usada no HTML) —
/// a validação server-side exige a <b>Secret Key</b>.</para>
///
/// <para>
/// Quando o <see cref="ILoggerFactory"/> estiver disponível no contexto de validação,
/// o atributo emite logs detalhados. Em produção, stack traces são suprimidos.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class ReCAPTCHAResponseAttribute : ValidationAttribute
{
    /// <summary>
    /// Nome da variável de ambiente usada para ler a Secret Key quando nenhuma chave é fornecida explicitamente.
    /// </summary>
    public const string EnvVariable_SecretKey = "GOOGLE_RECAPTCHA_SECRET_KEY";

    // Instância estática: evita criar um novo HttpClient a cada chamada de IsValid.
    private static readonly ReCAPTCHA _recaptcha = new();

    private string SecretKey { get; }

    /// <summary>
    /// Inicializa o atributo de validação do reCAPTCHA v2/v3.
    /// </summary>
    /// <param name="secretKey">
    /// Secret Key do reCAPTCHA (chave privada do servidor).
    /// Se omitida, lê da variável de ambiente <c>GOOGLE_RECAPTCHA_SECRET_KEY</c>.
    /// </param>
    public ReCAPTCHAResponseAttribute(string secretKey = "")
    {
        SecretKey = secretKey.IsSomething() ? secretKey : EnvUtil.Get(EnvVariable.Google_ReCAPTCHA_Secret_Key);
        ErrorMessage = "O campo {0} possui uma resposta de reCAPTCHA inválida.";
    }

    /// <summary>
    /// Valida a resposta do reCAPTCHA v2/v3 do Google.
    /// </summary>
    /// <param name="value">Token de resposta gerado pelo reCAPTCHA client-side.</param>
    /// <param name="validationContext">Contexto de validação fornecido pelo framework.</param>
    /// <returns>
    /// <see cref="ValidationResult.Success"/> se o token for válido ou nulo;
    /// um <see cref="ValidationResult"/> com a descrição do erro caso contrário.
    /// </returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        ILogger? logger = (validationContext?.GetService(typeof(ILoggerFactory)) as ILoggerFactory)
            ?.CreateLogger<ReCAPTCHAResponseAttribute>();

        if (value is null)
            return ValidationResult.Success;

        string response = value.ToString()!;

        if (SecretKey.IsNothing())
            return Error("Secret Key do Google reCAPTCHA não fornecida.", validationContext);

        logger?.LogDebug("Iniciando validação de token reCAPTCHA v3 via atributo.");

        try
        {
            DTOSiteVerifyResponse dto = Task.Run(() => _recaptcha.PostSiteVerifyAsync(new DTOSiteVerifyRequest(SecretKey, response))).GetAwaiter().GetResult();

            if (dto.Success)
            {
                logger?.LogInformation("Token reCAPTCHA válido. Host: {Hostname}.", dto.Hostname);
                return ValidationResult.Success;
            }

            string errorDetail = dto.ErrorCodes is { Count: > 0 }
                ? string.Join(", ", dto.ErrorCodes)
                : "resposta inválida";

            logger?.LogWarning("Token reCAPTCHA inválido. Códigos: {Codes}.", errorDetail);
            return Error(errorDetail, validationContext);
        }
        catch (BadRequestAppException ex)
        {
            string msg = string.Join(" | ", ex.Messages);
            logger?.LogWarning("Erro de validação reCAPTCHA: {Message}.", msg);
            return Error($"Erro de validação reCAPTCHA: {msg}", validationContext);
        }
        catch (AppException ex)
        {
            string msg = string.Join(" | ", ex.Messages);
            if (EnvUtil.IsProduction())
                logger?.LogError("Erro ao validar reCAPTCHA. {Message}", msg);
            else
                logger?.LogError(ex, "Erro ao validar reCAPTCHA. {Message}", msg);
            return Error("Erro ao processar a validação reCAPTCHA. Tente novamente.", validationContext);
        }
        catch (Exception ex)
        {
            if (EnvUtil.IsProduction())
                logger?.LogError("Erro fatal ao validar reCAPTCHA. {Message}", ex.Message);
            else
                logger?.LogError(ex, "Erro fatal ao validar reCAPTCHA. {Message}", ex.Message);
            return Error("Erro interno ao validar reCAPTCHA. Tente novamente.", validationContext);
        }
    }

    private static ValidationResult Error(string message, ValidationContext? context)
    {
        IEnumerable<string>? memberNames = context?.MemberName is string name ? [name] : null;
        return new ValidationResult(message, memberNames);
    }
}
