using LeadSoft.Adapter.Google.ReCaptchaService;
using LeadSoft.Adapter.Google.ReCaptchaService.DTOs;
using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Extensions;
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
    /// <exception cref="NotImplementedException">Este atributo ainda não está implementado.</exception>
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
        if (value is null)
            return ValidationResult.Success;

        string token = value.ToString()!;

        if (SiteKey.IsNothing())
            return Error("Site Key do reCAPTCHA Enterprise não fornecida.", validationContext);

        if (ProjectId.IsNothing())
            return Error("Project ID do reCAPTCHA Enterprise não fornecido.", validationContext);

        if (ApiKey.IsNothing())
            return Error("API Key do reCAPTCHA Enterprise não fornecida.", validationContext);

        using ReCAPTCHAEnterprise recaptcha = new(ProjectId);

        DTOAssessmentResp dto = Task.Run(
            () => recaptcha.CreateAssessmentsAsync(new DTOAssessmentReq(token, SiteKey), ApiKey))
            .GetAwaiter().GetResult();

        if (dto.TokenProperties?.Valid == true)
            return ValidationResult.Success;

        return Error("Resposta de reCAPTCHA Enterprise inválida.", validationContext);
    }

    private static ValidationResult Error(string message, ValidationContext? context)
    {
        IEnumerable<string>? memberNames = context?.MemberName is string name ? [name] : null;
        return new ValidationResult(message, memberNames);
    }
}
