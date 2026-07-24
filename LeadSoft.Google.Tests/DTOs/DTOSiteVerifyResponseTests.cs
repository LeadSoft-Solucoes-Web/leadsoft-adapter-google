using LeadSoft.Adapter.Google.ReCaptcha.Contracts;

namespace LeadSoft.Google.Tests.DTOs;

public class DTOSiteVerifyResponseTests
{
    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        var timestamp = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        DTOSiteVerifyResponse dto = new(true, timestamp, "test.localhost", ["missing-input-secret"]);


        Assert.True(dto.Success);
        Assert.Equal(timestamp, dto.ChallengeTs);
        Assert.Equal("test.localhost", dto.Hostname);
        Assert.Single(dto.ErrorCodes);
        Assert.Equal("missing-input-secret", dto.ErrorCodes[0]);
    }

    [Fact]
    public void ErrorCodes_IsNullByDefault()
    {
        var dto = new DTOSiteVerifyResponse();

        Assert.Null(dto.ErrorCodes);
    }

    [Fact]
    public void Success_IsFalseByDefault()
    {
        var dto = new DTOSiteVerifyResponse();

        Assert.False(dto.Success);
    }
}
