using Newtonsoft.Json;

namespace LeadSoft.Adapter.Google.ReCaptcha.DTOs;

// https://cloud.google.com/recaptcha/docs/reference/rest/v1/projects.assessments#TokenProperties
/// <summary>
/// DTO com as propriedades do token avaliado pelo reCAPTCHA Enterprise.
/// </summary>
[Serializable]
public sealed partial record DTOAssessmentTokenPropertiesResp
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
