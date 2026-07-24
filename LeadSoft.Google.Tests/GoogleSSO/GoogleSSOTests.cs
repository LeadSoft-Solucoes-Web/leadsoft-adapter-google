using LeadSoft.Google.Tests.Helpers;

namespace LeadSoft.Google.Tests.GoogleSSO;

/// <summary>
/// Testes unitários e de integração para <see cref="LeadSoft.Adapter.Google.Workspace.GoogleSSO"/>.
/// Testes unitários não requerem credenciais reais.
/// Testes de integração são ignorados automaticamente enquanto <see cref="GoogleSSOFixture.TestIdToken"/>
/// e <see cref="GoogleSSOFixture.TestAccessToken"/> não estiverem preenchidos na fixture.
/// </summary>
public class GoogleSSOTests(GoogleSSOFixture fixture) : IClassFixture<GoogleSSOFixture>
{
    // ─── Construção ───────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_CreatesInstanceWithoutThrowing()
    {
        var exception = Record.Exception(() =>
        {
            using var sso = new Adapter.Google.Workspace.GoogleSSO();
            Assert.NotNull(sso);
        });

        Assert.Null(exception);
    }

    // ─── GetOAuthSSOAsync — validações de guarda (sem chamadas à rede) ────────────

    [Fact]
    public async Task GetOAuthSSOAsync_NullToken_ThrowsBadRequestAppException()
    {
        using var sso = new Adapter.Google.Workspace.GoogleSSO();

        await Assert.ThrowsAsync<BadRequestAppException>(
            () => sso.GetOAuthSSOAsync(null!));
    }

    [Fact]
    public async Task GetOAuthSSOAsync_EmptyToken_ThrowsBadRequestAppException()
    {
        using var sso = new Adapter.Google.Workspace.GoogleSSO();

        await Assert.ThrowsAsync<BadRequestAppException>(
            () => sso.GetOAuthSSOAsync(string.Empty));
    }

    [Fact]
    public async Task GetOAuthSSOAsync_WhitespaceToken_ThrowsBadRequestAppException()
    {
        using var sso = new Adapter.Google.Workspace.GoogleSSO();

        await Assert.ThrowsAsync<BadRequestAppException>(
            () => sso.GetOAuthSSOAsync("   "));
    }

    [Fact]
    public async Task GetOAuthSSOAsync_MalformedJwt_ThrowsUnauthorizedAppException()
    {
        // Um JWT sem pontos falha no parsing local sem chamar a rede
        using var sso = new Adapter.Google.Workspace.GoogleSSO();

        await Assert.ThrowsAsync<UnauthorizedAppException>(
            () => sso.GetOAuthSSOAsync("token-invalido-sem-estrutura-jwt"));
    }

    // ─── GetUserProfileAsync — validações de guarda (sem chamadas à rede) ────────

    [Fact]
    public async Task GetUserProfileAsync_NullToken_ReturnsNull()
    {
        using var sso = new Adapter.Google.Workspace.GoogleSSO();

        var result = await sso.GetUserProfileAsync(null!);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserProfileAsync_EmptyToken_ReturnsNull()
    {
        using var sso = new Adapter.Google.Workspace.GoogleSSO();

        var result = await sso.GetUserProfileAsync(string.Empty);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserProfileAsync_WhitespaceToken_ReturnsNull()
    {
        using var sso = new Adapter.Google.Workspace.GoogleSSO();

        var result = await sso.GetUserProfileAsync("   ");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserProfileAsync_InvalidToken_ReturnsNullWithoutThrowing()
    {
        // Token com formato inválido deve retornar null (sem lançar exceção)
        using var sso = new Adapter.Google.Workspace.GoogleSSO();

        var result = await sso.GetUserProfileAsync("access-token-invalido");

        Assert.Null(result);
    }

    // ─── Integração: GetOAuthSSOAsync com token real ───────────────────────────────

    [Fact]
    public async Task GetOAuthSSOAsync_ValidToken_ReturnsUserWithEmail()
    {
        if (!fixture.HasIdToken)
            Assert.Skip("Teste de integração ignorado — preencha TestIdToken na GoogleSSOFixture.");

        using var sso = new Adapter.Google.Workspace.GoogleSSO();

        var result = await sso.GetOAuthSSOAsync(GoogleSSOFixture.TestIdToken);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Id);
        Assert.NotEmpty(result.Email);
        Assert.Contains("@", result.Email);
    }

    [Fact]
    public async Task GetOAuthSSOAsync_ValidToken_ReturnsUserWithName()
    {
        if (!fixture.HasIdToken)
            Assert.Skip("Teste de integração ignorado — preencha TestIdToken na GoogleSSOFixture.");

        using var sso = new Adapter.Google.Workspace.GoogleSSO();

        var result = await sso.GetOAuthSSOAsync(GoogleSSOFixture.TestIdToken);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Name);
    }

    [Fact]
    public async Task GetOAuthSSOAsync_ExpiredToken_ThrowsUnauthorizedAppException()
    {
        // Forneça um token expirado real para validar este cenário
        if (!fixture.HasCredentials)
            Assert.Skip("Teste de integração ignorado — preencha ClientId na GoogleSSOFixture.");

        const string expiredToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjEifQ.eyJzdWIiOiIxMjM0NTY3ODkwIiwiZW1haWwiOiJ0ZXN0QHRlc3QuY29tIiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjE1MTYyMzkwMjJ9.invalid";

        using var sso = new Adapter.Google.Workspace.GoogleSSO();

        await Assert.ThrowsAsync<UnauthorizedAppException>(
            () => sso.GetOAuthSSOAsync(expiredToken));
    }

    // ─── Integração: GetUserProfileAsync com token real ───────────────────────────

    [Fact]
    public async Task GetUserProfileAsync_ValidAccessToken_ReturnsExpandedProfile()
    {
        if (!fixture.HasAccessToken)
            Assert.Skip("Teste de integração ignorado — preencha TestAccessToken na GoogleSSOFixture.");

        using var sso = new Adapter.Google.Workspace.GoogleSSO();

        var result = await sso.GetUserProfileAsync(GoogleSSOFixture.TestAccessToken);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Id);
        Assert.NotEmpty(result.Email);
    }

    [Fact]
    public async Task GetUserProfileAsync_ValidAccessToken_EmailContainsAtSign()
    {
        if (!fixture.HasAccessToken)
            Assert.Skip("Teste de integração ignorado — preencha TestAccessToken na GoogleSSOFixture.");

        using var sso = new Adapter.Google.Workspace.GoogleSSO();

        var result = await sso.GetUserProfileAsync(GoogleSSOFixture.TestAccessToken);

        Assert.NotNull(result);
        Assert.Contains("@", result.Email);
    }
}
