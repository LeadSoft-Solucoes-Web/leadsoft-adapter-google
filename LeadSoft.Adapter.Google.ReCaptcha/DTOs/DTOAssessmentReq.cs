using Newtonsoft.Json;

namespace LeadSoft.Adapter.Google.ReCaptcha.DTOs;

// https://cloud.google.com/recaptcha/docs/reference/rest/v1/projects.assessments#resource:-assessment

/// <summary>
/// DTO de requisição para criação de uma avaliação (Assessment) no reCAPTCHA Enterprise.
/// </summary>
[Serializable]
public sealed partial record DTOAssessmentReq(string token, string siteKey)
{
    /// <summary>
    /// Evento reCAPTCHA contendo o token e a chave do site para avaliação.
    /// </summary>
    [JsonProperty("event")]
    public DTOAssessmentEventReq Event { get; set; } = new(token, siteKey);
}