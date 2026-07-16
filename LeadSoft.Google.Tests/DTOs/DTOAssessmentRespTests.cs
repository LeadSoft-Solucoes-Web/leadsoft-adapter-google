namespace LeadSoft.Google.Tests.DTOs;

public class DTOAssessmentRespTests
{
    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        var createTime = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        var dto = new DTOAssessmentResp
        {
            Name = "projects/test-project/assessments/abc123",
            Event = new DTOAssessmentEventResp { Token = "tok", SiteKey = "sk" },
            TokenProperties = new DTOAssessmentTokenPropertiesResp
            {
                Valid = true,
                Hostname = "test.localhost",
                Action = "submit",
                CreateTime = createTime,
                InvalidReason = ""
            }
        };

        Assert.Equal("projects/test-project/assessments/abc123", dto.Name);
        Assert.Equal("tok", dto.Event.Token);
        Assert.Equal("sk", dto.Event.SiteKey);
        Assert.True(dto.TokenProperties.Valid);
        Assert.Equal("test.localhost", dto.TokenProperties.Hostname);
        Assert.Equal("submit", dto.TokenProperties.Action);
        Assert.Equal(createTime, dto.TokenProperties.CreateTime);
    }

    [Fact]
    public void TokenProperties_Valid_IsFalseByDefault()
    {
        var props = new DTOAssessmentTokenPropertiesResp();

        Assert.False(props.Valid);
    }

    [Fact]
    public void TokenProperties_SupportsAllPlatformFields()
    {
        var props = new DTOAssessmentTokenPropertiesResp
        {
            Hostname = "web.example.com",
            AndroidPackageName = "com.example.app",
            IosBundleId = "com.example.app.ios"
        };

        Assert.Equal("web.example.com", props.Hostname);
        Assert.Equal("com.example.app", props.AndroidPackageName);
        Assert.Equal("com.example.app.ios", props.IosBundleId);
    }
}
