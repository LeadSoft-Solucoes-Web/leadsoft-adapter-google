namespace LeadSoft.Google.Tests;

/// <summary>
/// Exemplos de como usar Moq para simular os serviços do reCAPTCHA em testes unitários
/// de aplicações consumidoras. Demonstram que as interfaces são facilmente mockáveis.
/// </summary>
public class MockUsageExamplesTests
{
    [Fact]
    public async Task IReCAPTCHA_MockedSuccess_ReturnsExpectedResult()
    {
        var mock = new Mock<IReCAPTCHA>();
        mock.Setup(r => r.PostSiteVerifyAsync(It.IsAny<DTOSiteVerifyRequest>()))
            .ReturnsAsync(new DTOSiteVerifyResponse { Success = true, Hostname = "mocked.host" });

        var result = await mock.Object.PostSiteVerifyAsync(new DTOSiteVerifyRequest("secret", "token"));

        Assert.True(result.Success);
        Assert.Equal("mocked.host", result.Hostname);
        mock.Verify(r => r.PostSiteVerifyAsync(It.IsAny<DTOSiteVerifyRequest>()), Times.Once);
    }

    [Fact]
    public async Task IReCAPTCHA_MockedFailure_ReturnsSuccessFalseWithErrorCodes()
    {
        var mock = new Mock<IReCAPTCHA>();
        mock.Setup(r => r.PostSiteVerifyAsync(It.IsAny<DTOSiteVerifyRequest>()))
            .ReturnsAsync(new DTOSiteVerifyResponse
            {
                Success = false,
                ErrorCodes = new List<string> { "invalid-input-response" }
            });

        var result = await mock.Object.PostSiteVerifyAsync(new DTOSiteVerifyRequest("secret", "bad-token"));

        Assert.False(result.Success);
        Assert.Contains("invalid-input-response", result.ErrorCodes);
    }

    [Fact]
    public async Task IReCAPTCHAEnterprise_MockedSuccess_ReturnsExpectedAssessment()
    {
        var mock = new Mock<IReCAPTCHAEnterprise>();
        mock.Setup(e => e.CreateAssessmentsAsync(It.IsAny<DTOAssessmentReq>(), It.IsAny<string>()))
            .ReturnsAsync(new DTOAssessmentResp
            {
                Name = "projects/my-project/assessments/abc123",
                TokenProperties = new DTOAssessmentTokenPropertiesResp
                {
                    Valid = true,
                    Action = "submit",
                    Hostname = "mocked.host"
                }
            });

        var result = await mock.Object.CreateAssessmentsAsync(
            new DTOAssessmentReq("token", "site-key"), "api-key");

        Assert.True(result.TokenProperties.Valid);
        Assert.Equal("submit", result.TokenProperties.Action);
        mock.Verify(e => e.CreateAssessmentsAsync(It.IsAny<DTOAssessmentReq>(), "api-key"), Times.Once);
    }

    [Fact]
    public async Task IReCAPTCHAEnterprise_MockedException_PropagatesCorrectly()
    {
        var mock = new Mock<IReCAPTCHAEnterprise>();
        mock.Setup(e => e.CreateAssessmentsAsync(It.IsAny<DTOAssessmentReq>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("API key not valid."));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => mock.Object.CreateAssessmentsAsync(new DTOAssessmentReq("token", "key"), "invalid"));

        Assert.Equal("API key not valid.", exception.Message);
    }

    [Fact]
    public async Task IReCAPTCHA_VerifySpecificSecretPassed()
    {
        const string expectedSecret = "super-secret";
        var mock = new Mock<IReCAPTCHA>();
        mock.Setup(r => r.PostSiteVerifyAsync(It.Is<DTOSiteVerifyRequest>(dto => dto.Secret == expectedSecret)))
            .ReturnsAsync(new DTOSiteVerifyResponse { Success = true });

        var result = await mock.Object.PostSiteVerifyAsync(new DTOSiteVerifyRequest(expectedSecret, "token"));

        Assert.True(result.Success);
        mock.Verify(r => r.PostSiteVerifyAsync(
            It.Is<DTOSiteVerifyRequest>(dto => dto.Secret == expectedSecret)), Times.Once);
    }
}
