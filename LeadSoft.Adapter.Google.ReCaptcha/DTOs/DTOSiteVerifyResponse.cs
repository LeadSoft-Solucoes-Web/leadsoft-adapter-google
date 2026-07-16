using Newtonsoft.Json;

namespace LeadSoft.Adapter.Google.ReCaptchaService.DTOs;

/// <summary>
/// DTO de resposta da verificação de token do Google reCAPTCHA v3.
/// </summary>
public partial class DTOSiteVerifyResponse
{
    /// <summary>
    /// Indica se o desafio reCAPTCHA foi concluído com sucesso.
    /// </summary>
    [JsonProperty("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Data e hora em que o desafio foi carregado (formato ISO yyyy-MM-dd'T'HH:mm:ssZZ).
    /// </summary>
    [JsonProperty("challenge_ts")]
    public DateTime ChallengeTs { get; set; }

    /// <summary>
    /// Nome do host do site onde o reCAPTCHA foi resolvido.
    /// </summary>
    [JsonProperty("hostname")]
    public string Hostname { get; set; }

    /// <summary>
    /// Lista de códigos de erro retornados pela API, quando houver falha na validação.
    /// </summary>
    /// <remarks>
    /// Descrição dos códigos de erro:
    ///
    /// - <b>missing-input-secret</b>: O parâmetro <c>secret</c> está ausente.
    ///
    /// - <b>invalid-input-secret</b>: O parâmetro <c>secret</c> é inválido ou malformado.
    ///
    /// - <b>missing-input-response</b>: O parâmetro <c>response</c> está ausente.
    ///
    /// - <b>invalid-input-response</b>: O parâmetro <c>response</c> é inválido ou malformado.
    ///
    /// - <b>bad-request</b>: A requisição é inválida ou malformada.
    ///
    /// - <b>timeout-or-duplicate</b>: O token não é mais válido — expirou ou já foi utilizado anteriormente.
    /// </remarks>
    [JsonProperty("error-codes", NullValueHandling = NullValueHandling.Ignore)]
    public IList<string> ErrorCodes { get; set; }
}
