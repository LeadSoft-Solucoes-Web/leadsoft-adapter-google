using LeadSoft.Google.Tests.Helpers;
using Newtonsoft.Json;
using System.Net;

namespace LeadSoft.Google.Tests;

public class ReCAPTCHAEnterpriseTests
{
    private const string TestProjectId = "my-test-project";

    private static string AssessmentSuccessJson() =>
        JsonConvert.SerializeObject(new
        {
            name = $"projects/{TestProjectId}/assessments/abc123",
            @event = new { token = "test-token", siteKey = "test-site-key" },
            tokenProperties = new
            {
                valid = true,
                invalidReason = "",
                hostname = "test.localhost",
                androidPackageName = "",
                iosBundleId = "",
                action = "submit",
                createTime = "2024-01-15T10:30:00Z"
            }
        });

    private static string AssessmentInvalidTokenJson() =>
        JsonConvert.SerializeObject(new
        {
            name = $"projects/{TestProjectId}/assessments/xyz789",
            @event = new { token = "bad-token", siteKey = "test-site-key" },
            tokenProperties = new
            {
                valid = false,
                invalidReason = "EXPIRED",
                hostname = "test.localhost",
                androidPackageName = "",
                iosBundleId = "",
                action = "submit",
                createTime = "2024-01-15T10:30:00Z"
            }
        });

    [Fact]
    public void Constructor_CreatesInstanceWithoutThrowing()
    {
        using var enterprise = new ReCAPTCHAEnterprise(TestProjectId, new FakeHttpMessageHandler("{}"));

        Assert.NotNull(enterprise);
    }

    [Fact]
    public async Task CreateAssessmentsAsync_SuccessResponse_ReturnsDtoWithValidTrue()
    {
        using var enterprise = new ReCAPTCHAEnterprise(TestProjectId, new FakeHttpMessageHandler(AssessmentSuccessJson()));
        var request = new DTOAssessmentReq("test-token", "test-site-key");

        var result = await enterprise.CreateAssessmentsAsync(request, "my-api-key");

        Assert.NotNull(result);
        Assert.NotNull(result.TokenProperties);
        Assert.True(result.TokenProperties.Valid);
        Assert.Equal("test.localhost", result.TokenProperties.Hostname);
        Assert.Equal("submit", result.TokenProperties.Action);
    }

    [Fact]
    public async Task CreateAssessmentsAsync_InvalidToken_ReturnsDtoWithValidFalseAndReason()
    {
        using var enterprise = new ReCAPTCHAEnterprise(TestProjectId, new FakeHttpMessageHandler(AssessmentInvalidTokenJson()));
        var request = new DTOAssessmentReq("bad-token", "test-site-key");

        var result = await enterprise.CreateAssessmentsAsync(request, "my-api-key");

        Assert.NotNull(result);
        Assert.False(result.TokenProperties.Valid);
        Assert.Equal("EXPIRED", result.TokenProperties.InvalidReason);
    }

    [Fact]
    public async Task CreateAssessmentsAsync_HttpBadRequest_ThrowsInvalidOperationException()
    {
        const string errorBody = "{\"error\":{\"code\":400,\"message\":\"API key not valid.\",\"status\":\"INVALID_ARGUMENT\"}}";
        using var enterprise = new ReCAPTCHAEnterprise(TestProjectId, new FakeHttpMessageHandler(errorBody, HttpStatusCode.BadRequest));
        var request = new DTOAssessmentReq("test-token", "test-site-key");

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => enterprise.CreateAssessmentsAsync(request, "invalid-api-key"));

        Assert.Contains("INVALID_ARGUMENT", exception.Message);
    }

    [Fact]
    public async Task CreateAssessmentsAsync_HttpUnauthorized_ThrowsInvalidOperationException()
    {
        const string errorBody = "{\"error\":{\"code\":401,\"message\":\"Request had invalid authentication credentials.\",\"status\":\"UNAUTHENTICATED\"}}";
        using var enterprise = new ReCAPTCHAEnterprise(TestProjectId, new FakeHttpMessageHandler(errorBody, HttpStatusCode.Unauthorized));
        var request = new DTOAssessmentReq("test-token", "test-site-key");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => enterprise.CreateAssessmentsAsync(request, "bad-api-key"));
    }

    [Fact]
    public async Task CreateAssessmentsAsync_ResultContainsEventData()
    {
        using var enterprise = new ReCAPTCHAEnterprise(TestProjectId, new FakeHttpMessageHandler(AssessmentSuccessJson()));
        var request = new DTOAssessmentReq("test-token", "test-site-key");

        var result = await enterprise.CreateAssessmentsAsync(request, "my-api-key");

        Assert.NotNull(result.Event);
        Assert.Equal("test-token", result.Event.Token);
        Assert.Equal("test-site-key", result.Event.SiteKey);
    }
}
