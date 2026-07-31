using LeadSoft.Adapter.Google.ReCaptcha.Contracts;
using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Exceptions;
using LeadSoft.Common.Library.Extensions;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace LeadSoft.Adapter.Google.ReCaptcha.Attibutes;

/// <summary>
/// Atributo de validação para verificar a resposta do reCAPTCHA Enterprise do Google.
/// </summary>
/// <remarks>
/// Aplique a propriedades, campos ou parâmetros que recebem o token gerado pelo reCAPTCHA Enterprise client-side.
/// Os valores necessários podem ser fornecidos diretamente no construtor ou via variáveis de ambiente:
/// <list type="bullet">
///   <item><term><c>GOOGLE_RECAPTCHA_ENTERPRISE_SITE_KEY</c></term><description>Chave pública do site reCAPTCHA Enterprise.</description></item>
///   <item><term><c>GOOGLE_RECAPTCHA_ENTERPRISE_PROJECT_ID</c></term><description>ID do projeto no Google Cloud.</description></item>
///   <item><term><c>GOOGLE_RECAPTCHA_ENTERPRISE_API_KEY</c></term><description>Chave de API do Google Cloud Console.</description></item>
/// </list>
/// <para>
/// Quando o <see cref="ILoggerFactory"/> estiver disponível no contexto de validação,
/// o atributo emite logs detalhados. Em produção, stack traces são suprimidos.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class ReCAPTCHAEnterpriseResponseAttribute : ValidationAttribute
{
    private string SiteKey { get; }
    private string ProjectId { get; }
    private string ApiKey { get; }

    /// <summary>
    /// Inicializa o atributo de validação do reCAPTCHA Enterprise.
    /// </summary>
    /// <param name="siteKey">Site Key pública do reCAPTCHA Enterprise. Se omitida, lê de <c>GOOGLE_RECAPTCHA_ENTERPRISE_SITE_KEY</c>.</param>
    /// <param name="projectId">ID do projeto no Google Cloud. Se omitido, lê de <c>GOOGLE_RECAPTCHA_ENTERPRISE_PROJECT_ID</c>.</param>
    /// <param name="apiKey">Chave de API do Google Cloud Console. Se omitida, lê de <c>GOOGLE_RECAPTCHA_ENTERPRISE_API_KEY</c>.</param>
    public ReCAPTCHAEnterpriseResponseAttribute(string siteKey = "", string projectId = "", string apiKey = "")
    {
        SiteKey = siteKey.IsSomething() ? siteKey : EnvUtil.Get(EnvVariable.Google_ReCAPTCHA_Enterprise_SiteKey);
        ProjectId = projectId.IsSomething() ? projectId : EnvUtil.Get(EnvVariable.Google_ReCAPTCHA_Enterprise_Project_Id);
        ApiKey = apiKey.IsSomething() ? apiKey : EnvUtil.Get(EnvVariable.Google_ReCAPTCHA_Enterprise_Api_Key);

        ErrorMessage = "O campo {0} possui uma resposta de reCAPTCHA Enterprise inválida.";
    }

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        ILoggerFactory? loggerFactory = validationContext?.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
        ILogger? logger = loggerFactory?.CreateLogger<ReCAPTCHAEnterpriseResponseAttribute>();

        if (value is null)
            return ValidationResult.Success;

        string token = value.ToString()!;

        if (SiteKey.IsNothing())
            return Error("Site Key do reCAPTCHA Enterprise não fornecida.", validationContext);

        if (ProjectId.IsNothing())
            return Error("Project ID do reCAPTCHA Enterprise não fornecido.", validationContext);

        if (ApiKey.IsNothing())
            return Error("API Key do reCAPTCHA Enterprise não fornecida.", validationContext);

        logger?.LogDebug("Iniciando validação de token reCAPTCHA Enterprise via atributo. Projeto: {ProjectId}.", ProjectId);

        using ReCAPTCHAEnterprise recaptcha = new(ProjectId, loggerFactory?.CreateLogger<ReCAPTCHAEnterprise>());
        try
        {
            DTOAssessmentResp dto = Task.Run(() => recaptcha.CreateAssessmentsAsync(new DTOAssessmentReq(token, SiteKey), ApiKey)).GetAwaiter().GetResult();

            if (dto.TokenProperties?.Valid == true)
            {
                logger?.LogInformation("Token reCAPTCHA Enterprise válido. Host: {Hostname}.", dto.TokenProperties.Hostname);
                return ValidationResult.Success;
            }

            string reason = dto.TokenProperties?.InvalidReason ?? "desconhecido";
            logger?.LogWarning("Token reCAPTCHA Enterprise inválido. Motivo: {Reason}.", reason);
            return Error($"Token reCAPTCHA Enterprise inválido: {reason}", validationContext);
        }
        catch (BadRequestAppException ex)
        {
            string msg = string.Join(" | ", ex.Messages);
            logger?.LogWarning("Erro de validação reCAPTCHA Enterprise: {Message}.", msg);
            return Error($"Erro de validação reCAPTCHA Enterprise: {msg}", validationContext);
        }
        catch (AppException ex)
        {
            string msg = string.Join(" | ", ex.Messages);
            if (EnvUtil.IsProduction())
                logger?.LogError("Erro ao validar reCAPTCHA Enterprise. {Message}", msg);
            else
                logger?.LogError(ex, "Erro ao validar reCAPTCHA Enterprise. {Message}", msg);
            return Error("Erro ao processar a validação reCAPTCHA Enterprise. Tente novamente.", validationContext);
        }
        catch (Exception ex)
        {
            if (EnvUtil.IsProduction())
                logger?.LogError("Erro fatal ao validar reCAPTCHA Enterprise. {Message}", ex.Message);
            else
                logger?.LogError(ex, "Erro fatal ao validar reCAPTCHA Enterprise. {Message}", ex.Message);
            return Error("Erro interno ao validar reCAPTCHA Enterprise. Tente novamente.", validationContext);
        }
    }

    private static ValidationResult Error(string message, ValidationContext? context)
    {
        IEnumerable<string>? memberNames = context?.MemberName is string name ? [name] : null;
        return new ValidationResult(message, memberNames);
    }
}
