using Newtonsoft.Json;
using System.ComponentModel;

namespace LeadSoft.Adapter.Google.ReCaptchaService.DTOs;

/// <summary>
/// DTO de requisição para verificação de token do Google reCAPTCHA v3.
/// </summary>
public partial class DTOSiteVerifyRequest
{
    /// <summary>
    /// Obrigatório. A chave secreta compartilhada entre o seu site e o reCAPTCHA.
    /// </summary>
    [JsonProperty("secret")]
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Obrigatório. O token de resposta do usuário fornecido pela integração client-side do reCAPTCHA.
    /// </summary>
    [JsonProperty("response")]
    public string Response { get; set; } = string.Empty;

    /// <summary>
    /// Opcional. O endereço IP do usuário.
    /// </summary>
    [DefaultValue("")]
    [JsonProperty("remoteip", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string RemoteIp { get; set; } = string.Empty;

    /// <summary>
    /// Construtor padrão.
    /// </summary>
    public DTOSiteVerifyRequest()
    {
    }

    /// <summary>
    /// Construtor com informações da requisição.
    /// </summary>
    /// <param name="aSecret">Chave secreta do reCAPTCHA.</param>
    /// <param name="aResponse">Token de resposta do usuário.</param>
    /// <param name="aRemoteIp">Endereço IP do usuário (opcional).</param>
    public DTOSiteVerifyRequest(string aSecret, string aResponse, string aRemoteIp = "")
    {
        Secret = aSecret;
        Response = aResponse;
        RemoteIp = aRemoteIp;
    }
}
