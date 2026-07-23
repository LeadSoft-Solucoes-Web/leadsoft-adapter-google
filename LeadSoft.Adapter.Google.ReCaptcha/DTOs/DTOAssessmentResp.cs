using Newtonsoft.Json;

namespace LeadSoft.Adapter.Google.ReCaptcha.DTOs;

// https://cloud.google.com/recaptcha/docs/reference/rest/v1/projects.assessments#Assessment

/// <summary>
/// DTO de resposta da avaliação (Assessment) do reCAPTCHA Enterprise.
/// </summary>
[Serializable]
public sealed partial record DTOAssessmentResp
{
    /// <summary>
    /// Nome do recurso da avaliação gerado pela API do reCAPTCHA Enterprise.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Evento reCAPTCHA que originou a avaliação, refletindo os dados enviados na requisição.
    /// </summary>
    [JsonProperty("event")]
    public DTOAssessmentEventResp Event { get; set; }

    /// <summary>
    /// Propriedades do token avaliado, incluindo validade e plataforma de origem.
    /// </summary>
    [JsonProperty("tokenProperties")]
    public DTOAssessmentTokenPropertiesResp TokenProperties { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? ErrorMessage { get; private set; } = null;

    public DTOAssessmentResp()
    {
    }

    public DTOAssessmentResp(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}
