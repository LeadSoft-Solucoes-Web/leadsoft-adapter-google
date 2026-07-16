namespace LeadSoft.Google.Tests.DTOs;

public class DTOAssessmentReqTests
{
    [Fact]
    public void Constructor_SetsEventWithCorrectTokenAndSiteKey()
    {
        var dto = new DTOAssessmentReq("my-token", "my-site-key");

        Assert.NotNull(dto.Event);
        Assert.Equal("my-token", dto.Event.Token);
        Assert.Equal("my-site-key", dto.Event.SiteKey);
    }

    [Fact]
    public void EventConstructor_NullToken_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DTOAssessmentEventReq(null!, "my-site-key"));
    }

    [Fact]
    public void EventConstructor_NullSiteKey_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DTOAssessmentEventReq("my-token", null!));
    }

    [Fact]
    public void EventConstructor_ValidArguments_SetsPropertiesCorrectly()
    {
        var evt = new DTOAssessmentEventReq("token-value", "site-key-value");

        Assert.Equal("token-value", evt.Token);
        Assert.Equal("site-key-value", evt.SiteKey);
    }
}
