using Newtonsoft.Json;

namespace LeadSoft.Adapter.Google.ReCaptcha.DTOs;

[Serializable]
public sealed partial record DTOAssessmentErrorResp
{
    [JsonProperty("code")]
    public int Code { get; private set; } = 0;

    [JsonProperty("message")]
    public string Message { get; private set; } = string.Empty;

    [JsonProperty("status")]
    public string Status { get; private set; } = string.Empty;

    public string GetErrorMsg()
        => $"Google reCAPTCHA Enterprise API Error: {Code}: {Message} [{Status}]";
}
