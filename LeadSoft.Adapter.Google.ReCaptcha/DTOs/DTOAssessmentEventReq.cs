using Newtonsoft.Json;

namespace LeadSoft.Adapter.Google.ReCaptcha.DTOs;

// https://cloud.google.com/recaptcha/docs/reference/rest/v1/projects.assessments#Event

/// <summary>
/// DTO do evento reCAPTCHA Enterprise contendo o token e a chave do site.
/// </summary>
[Serializable]
public sealed partial record DTOAssessmentEventReq
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
