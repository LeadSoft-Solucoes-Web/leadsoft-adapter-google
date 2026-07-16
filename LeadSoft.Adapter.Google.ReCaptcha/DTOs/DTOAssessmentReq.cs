using Newtonsoft.Json;

namespace LeadSoft.Adapter.Google.ReCaptchaService.DTOs;

// https://cloud.google.com/recaptcha/docs/reference/rest/v1/projects.assessments#resource:-assessment

/// <summary>
/// DTO de requisição para criação de uma avaliação (Assessment) no reCAPTCHA Enterprise.
/// </summary>
[Serializable]
public partial class DTOAssessmentReq
{
    /// <summary>
    /// Evento reCAPTCHA contendo o token e a chave do site para avaliação.
    /// </summary>
    [JsonProperty("event")]
    public DTOAssessmentEventReq Event { get; set; }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="DTOAssessmentReq"/>.
    /// </summary>
    /// <param name="token">Token de resposta do usuário gerado pelo reCAPTCHA Enterprise client-side.</param>
    /// <param name="siteKey">Chave do site utilizada para invocar o reCAPTCHA Enterprise.</param>
    public DTOAssessmentReq(string token, string siteKey)
    {
        Event = new DTOAssessmentEventReq(token, siteKey);
    }
}

// https://cloud.google.com/recaptcha/docs/reference/rest/v1/projects.assessments#Event

/// <summary>
/// DTO do evento reCAPTCHA Enterprise contendo o token e a chave do site.
/// </summary>
[Serializable]
public class DTOAssessmentEventReq
{
    /// <summary>
    /// Token de resposta do usuário fornecido pela integração client-side do reCAPTCHA Enterprise.
    /// </summary>
    [JsonProperty("token")]
    public string Token { get; set; }

    /// <summary>
    /// Chave do site utilizada para invocar o reCAPTCHA Enterprise e gerar o token.
    /// </summary>
    [JsonProperty("siteKey")]
    public string SiteKey { get; set; }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="DTOAssessmentEventReq"/>.
    /// </summary>
    /// <param name="token">Token de resposta do usuário.</param>
    /// <param name="siteKey">Chave do site reCAPTCHA Enterprise.</param>
    /// <exception cref="ArgumentNullException">Lançado quando <paramref name="token"/> ou <paramref name="siteKey"/> for nulo.</exception>
    public DTOAssessmentEventReq(string token, string siteKey)
    {
        ArgumentNullException.ThrowIfNull(token);
        ArgumentNullException.ThrowIfNull(siteKey);

        Token = token;
        SiteKey = siteKey;
    }
}
