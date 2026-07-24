namespace LeadSoft.Google.Tests.GoogleSSO;

/// <summary>
/// Exemplos de como usar Moq para simular <see cref="IGoogleSSO"/> em testes unitários
/// de serviços consumidores. Demonstram que a interface é facilmente mockável sem
/// necessidade de credenciais reais do Google.
/// </summary>
public class GoogleSSOMockTests
{
    // ─── GetOAuthSSOAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task IGoogleSSO_MockedLogin_ReturnsExpectedUser()
    {
        var mock = new Mock<IGoogleSSO>();
        mock.Setup(s => s.GetOAuthSSOAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DTOGoogleUserResponse("123", "user@empresa.com", "User Name", "https://photo.url", "empresa.com"));

        var result = await mock.Object.GetOAuthSSOAsync("qualquer-token");

        Assert.NotNull(result);
        Assert.Equal("123", result.Id);
        Assert.Equal("user@empresa.com", result.Email);
        Assert.Equal("empresa.com", result.Domain);
        mock.Verify(s => s.GetOAuthSSOAsync("qualquer-token", default), Times.Once);
    }

    [Fact]
    public async Task IGoogleSSO_MockedInvalidToken_ThrowsUnauthorizedAppException()
    {
        var mock = new Mock<IGoogleSSO>();
        mock.Setup(s => s.GetOAuthSSOAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAppException("Token do Google inválido ou expirado."));

        await Assert.ThrowsAsync<UnauthorizedAppException>(
            () => mock.Object.GetOAuthSSOAsync("token-expirado"));
    }

    [Fact]
    public async Task IGoogleSSO_MockedWrongDomain_ThrowsForbiddenAppException()
    {
        var mock = new Mock<IGoogleSSO>();
        mock.Setup(s => s.GetOAuthSSOAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ForbiddenAppException("Domínio 'gmail.com' não é permitido."));

        await Assert.ThrowsAsync<ForbiddenAppException>(
            () => mock.Object.GetOAuthSSOAsync("token-de-gmail"));
    }

    [Fact]
    public async Task IGoogleSSO_VerifySpecificTokenPassed()
    {
        const string expectedToken = "meu-id-token-especifico";
        var mock = new Mock<IGoogleSSO>();
        mock.Setup(s => s.GetOAuthSSOAsync(expectedToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DTOGoogleUserResponse("99", "u@e.com", "User", "", "e.com"));

        await mock.Object.GetOAuthSSOAsync(expectedToken);

        mock.Verify(s => s.GetOAuthSSOAsync(expectedToken, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ─── GetUserProfileAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task IGoogleSSO_MockedExpandedProfile_ReturnsAllFields()
    {
        var birthday = new DateTime(1990, 5, 20);
        var mock = new Mock<IGoogleSSO>();
        mock.Setup(s => s.GetUserProfileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DTOGoogleUserExpandedResponse(
                Id:          "456",
                Email:       "user@empresa.com",
                Name:        "User Name",
                Picture:     "https://photo.url",
                PhoneNumber: "+55 11 91234-5678",
                Birthday:    birthday));

        var result = await mock.Object.GetUserProfileAsync("access-token");

        Assert.NotNull(result);
        Assert.Equal("456", result.Id);
        Assert.Equal("+55 11 91234-5678", result.PhoneNumber);
        Assert.Equal(birthday, result.Birthday);
    }

    [Fact]
    public async Task IGoogleSSO_MockedExpandedProfile_ReturnsNullOnFailure()
    {
        var mock = new Mock<IGoogleSSO>();
        mock.Setup(s => s.GetUserProfileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DTOGoogleUserExpandedResponse?)null);

        var result = await mock.Object.GetUserProfileAsync("access-token-invalido");

        Assert.Null(result);
    }

    [Fact]
    public async Task IGoogleSSO_MockedProfileWithoutOptionalFields_HasNullPhoneAndBirthday()
    {
        var mock = new Mock<IGoogleSSO>();
        mock.Setup(s => s.GetUserProfileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DTOGoogleUserExpandedResponse("1", "a@b.com", "Name", ""));

        var result = await mock.Object.GetUserProfileAsync("token");

        Assert.NotNull(result);
        Assert.Null(result.PhoneNumber);
        Assert.Null(result.Birthday);
    }

    // ─── CancellationToken ────────────────────────────────────────────────────────

    [Fact]
    public async Task IGoogleSSO_CancellationRequested_PropagatesCancellation()
    {
        var cts = new CancellationTokenSource();
        var mock = new Mock<IGoogleSSO>();
        mock.Setup(s => s.GetOAuthSSOAsync(It.IsAny<string>(), cts.Token))
            .ThrowsAsync(new OperationCanceledException());

        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => mock.Object.GetOAuthSSOAsync("token", cts.Token));
    }
}
