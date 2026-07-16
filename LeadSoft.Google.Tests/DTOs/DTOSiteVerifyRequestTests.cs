namespace LeadSoft.Google.Tests.DTOs;

public class DTOSiteVerifyRequestTests
{
    [Fact]
    public void DefaultConstructor_InitializesWithEmptyValues()
    {
        var dto = new DTOSiteVerifyRequest();

        Assert.Equal(string.Empty, dto.Secret);
        Assert.Equal(string.Empty, dto.Response);
        Assert.Equal(string.Empty, dto.RemoteIp);
    }

    [Fact]
    public void ParameterizedConstructor_SetsAllPropertiesCorrectly()
    {
        var dto = new DTOSiteVerifyRequest("my-secret", "user-token", "192.168.0.1");

        Assert.Equal("my-secret", dto.Secret);
        Assert.Equal("user-token", dto.Response);
        Assert.Equal("192.168.0.1", dto.RemoteIp);
    }

    [Fact]
    public void ParameterizedConstructor_RemoteIp_DefaultsToEmptyString()
    {
        var dto = new DTOSiteVerifyRequest("my-secret", "user-token");

        Assert.Equal(string.Empty, dto.RemoteIp);
    }

    [Fact]
    public void Properties_CanBeSetAfterConstruction()
    {
        var dto = new DTOSiteVerifyRequest
        {
            Secret = "secret-value",
            Response = "response-value",
            RemoteIp = "10.0.0.1"
        };

        Assert.Equal("secret-value", dto.Secret);
        Assert.Equal("response-value", dto.Response);
        Assert.Equal("10.0.0.1", dto.RemoteIp);
    }
}
