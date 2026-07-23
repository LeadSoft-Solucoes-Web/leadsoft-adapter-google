using Newtonsoft.Json;

namespace LeadSoft.Adapter.Google.ReCaptcha.DTOs;

/// <summary>
/// DTO do evento reCAPTCHA retornado na resposta da avaliação Enterprise.
/// </summary>
public sealed partial record DTOAssessmentEventResp
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
