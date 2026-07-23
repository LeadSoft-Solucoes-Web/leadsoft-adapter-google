using LeadSoft.Adapter.Google.ReCaptcha.DTOs;
using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Extensions;
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
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
public class ReCAPTCHAResponseAttribute : ValidationAttribute
{
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
        if (value is null)
            return ValidationResult.Success;

        string response = value.ToString()!;

        if (SecretKey.IsNothing())
            return Error("Secret Key do Google reCAPTCHA não fornecida.", validationContext);

        DTOSiteVerifyResponse dto = Task.Run(
            () => _recaptcha.PostSiteVerifyAsync(new DTOSiteVerifyRequest(SecretKey, response)))
            .GetAwaiter().GetResult();

        if (dto.Success)
            return ValidationResult.Success;

        string errorDetail = dto.ErrorCodes is { Count: > 0 }
            ? string.Join(", ", dto.ErrorCodes)
            : "resposta inválida";

        return Error(errorDetail, validationContext);
    }

    private static ValidationResult Error(string message, ValidationContext? context)
    {
        IEnumerable<string>? memberNames = context?.MemberName is string name ? [name] : null;
        return new ValidationResult(message, memberNames);
    }
}
