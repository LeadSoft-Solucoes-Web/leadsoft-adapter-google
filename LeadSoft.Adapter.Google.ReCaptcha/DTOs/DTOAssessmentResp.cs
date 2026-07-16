using Newtonsoft.Json;

namespace LeadSoft.Adapter.Google.ReCaptchaService.DTOs;

// https://cloud.google.com/recaptcha/docs/reference/rest/v1/projects.assessments#Assessment

/// <summary>
/// DTO de resposta da avaliação (Assessment) do reCAPTCHA Enterprise.
/// </summary>
[Serializable]
public partial class DTOAssessmentResp
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
}

// https://cloud.google.com/recaptcha/docs/reference/rest/v1/projects.assessments#TokenProperties

/// <summary>
/// DTO com as propriedades do token avaliado pelo reCAPTCHA Enterprise.
/// </summary>
[Serializable]
public class DTOAssessmentTokenPropertiesResp
{
    /// <summary>
    /// Indica se o token é válido.
    /// </summary>
    [JsonProperty("valid")]
    public bool Valid { get; set; }

    /// <summary>
    /// Motivo pelo qual o token foi considerado inválido, quando aplicável.
    /// </summary>
    [JsonProperty("invalidReason")]
    public string InvalidReason { get; set; }

    /// <summary>
    /// Nome do host do site onde o reCAPTCHA foi resolvido (para integrações web).
    /// </summary>
    [JsonProperty("hostname")]
    public string Hostname { get; set; }

    /// <summary>
    /// Nome do pacote Android onde o reCAPTCHA foi resolvido (para integrações mobile Android).
    /// </summary>
    [JsonProperty("androidPackageName")]
    public string AndroidPackageName { get; set; }

    /// <summary>
    /// Bundle ID do aplicativo iOS onde o reCAPTCHA foi resolvido (para integrações mobile iOS).
    /// </summary>
    [JsonProperty("iosBundleId")]
    public string IosBundleId { get; set; }

    /// <summary>
    /// Ação associada ao token reCAPTCHA, conforme definida na integração client-side.
    /// </summary>
    [JsonProperty("action")]
    public string Action { get; set; }

    /// <summary>
    /// Data e hora em que o token foi criado.
    /// </summary>
    [JsonProperty("createTime")]
    public DateTime CreateTime { get; set; }
}

/// <summary>
/// DTO do evento reCAPTCHA retornado na resposta da avaliação Enterprise.
/// </summary>
public class DTOAssessmentEventResp
{
    /// <summary>
    /// Token de resposta do usuário que foi avaliado.
    /// </summary>
    [JsonProperty("token")]
    public string Token { get; set; }

    /// <summary>
    /// Chave do site utilizada para gerar o token avaliado.
    /// </summary>
    [JsonProperty("siteKey")]
    public string SiteKey { get; set; }
}
