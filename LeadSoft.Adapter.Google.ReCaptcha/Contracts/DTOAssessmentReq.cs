using Newtonsoft.Json;

namespace LeadSoft.Adapter.Google.ReCaptcha.Contracts;

// https://cloud.google.com/recaptcha/docs/reference/rest/v1/projects.assessments#resource:-assessment

/// <summary>
/// DTO de requisição para criação de uma avaliação (Assessment) no reCAPTCHA Enterprise.
/// </summary>
[Serializable]
public sealed partial record DTOAssessmentReq
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
    /// <param name="siteKey">Chave do site reCAPTCHA Enterprise.</param>
    public DTOAssessmentReq(string token, string siteKey)
    {
        Event = new DTOAssessmentEventReq(token, siteKey);
    }
}
