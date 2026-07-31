using Newtonsoft.Json;

namespace LeadSoft.Adapter.Google.ReCaptcha.Contracts;

[Serializable]
public sealed partial record DTOAssessmentErrorResp
{
    [JsonProperty("error")]
    public DTOAssessmentError Error { get; private set; } = new();

    public string GetErrorMsg() => Error?.GetErrorMsg() ?? "Assessment Error não identificado.";
}
