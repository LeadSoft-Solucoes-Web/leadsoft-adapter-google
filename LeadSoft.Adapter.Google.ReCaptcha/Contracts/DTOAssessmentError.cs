using Newtonsoft.Json;

namespace LeadSoft.Adapter.Google.ReCaptcha.Contracts;

[Serializable]
public sealed partial record DTOAssessmentError
{
    [JsonProperty("code")]
    public int Code { get; private set; } = 0;

    [JsonProperty("message")]
    public string Message { get; private set; } = string.Empty;

    [JsonProperty("status")]
    public string Status { get; private set; } = string.Empty;

    [JsonProperty("details")]
    public IList<object> Details { get; private set; } = [];

    public string GetErrorMsg()
        => $"Google reCAPTCHA Enterprise API Error: {Code}: {Message} [{Status}]";
}
