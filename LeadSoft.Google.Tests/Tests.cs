using LeadSoft.Google.Tests.Helpers;
using Newtonsoft.Json;
using System.Net;

namespace LeadSoft.Google.Tests;

public class ReCAPTCHATests
{
    private static string SiteVerifySuccessJson() =>
        JsonConvert.SerializeObject(new
        {
            success = true,
            challenge_ts = "2024-01-15T10:30:00Z",
            hostname = "test.localhost"
        });

    private static string SiteVerifyFailureJson() =>
        JsonConvert.SerializeObject(new
        {
            success = false,
            challenge_ts = "2024-01-15T10:30:00Z",
            hostname = "test.localhost"
        });

    [Fact]
    public void Constructor_CreatesInstanceWithoutThrowing()
    {
        using var recaptcha = new ReCAPTCHA(new FakeHttpMessageHandler("{}"));

        Assert.NotNull(recaptcha);
    }

    [Fact]
    public async Task PostSiteVerifyAsync_SuccessResponse_ReturnsDtoWithSuccessTrue()
    {
        using var recaptcha = new ReCAPTCHA(new FakeHttpMessageHandler(SiteVerifySuccessJson()));
        var request = new DTOSiteVerifyRequest("secret-key", "user-response-token");

        var result = await recaptcha.PostSiteVerifyAsync(request);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("test.localhost", result.Hostname);
    }

    [Fact]
    public async Task PostSiteVerifyAsync_FailureResponse_ReturnsDtoWithSuccessFalse()
    {
        using var recaptcha = new ReCAPTCHA(new FakeHttpMessageHandler(SiteVerifyFailureJson()));
        var request = new DTOSiteVerifyRequest("secret-key", "invalid-token");

        var result = await recaptcha.PostSiteVerifyAsync(request);

        Assert.NotNull(result);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task PostSiteVerifyAsync_WithRemoteIp_CompletesSuccessfully()
    {
        using var recaptcha = new ReCAPTCHA(new FakeHttpMessageHandler(SiteVerifySuccessJson()));
        var request = new DTOSiteVerifyRequest("secret-key", "user-response-token", "192.168.0.1");

        var result = await recaptcha.PostSiteVerifyAsync(request);

        Assert.True(result.Success);
    }
}
