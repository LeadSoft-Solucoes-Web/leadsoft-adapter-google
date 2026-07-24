namespace LeadSoft.Google.Tests.Helpers;

/// <summary>
/// Fixture compartilhada para testes do Google SSO.
/// Configura as variáveis de ambiente necessárias e expõe os tokens de integração.
/// </summary>
/// <remarks>
/// Para executar os testes de integração (marcados com <c>Skip.If</c>), preencha as
/// constantes abaixo e gere tokens válidos via fluxo Google Sign-In no frontend.
/// Os testes unitários não dependem de nenhuma configuração aqui.
/// </remarks>
public sealed class GoogleSSOFixture : IDisposable
{
    // ┌──────────────────────────────────────────────────────────────────────────────┐
    // │  CONFIGURAÇÃO — preencha apenas estas constantes para os testes de integração │
    // └──────────────────────────────────────────────────────────────────────────────┘

    /// <summary>Client ID do projeto OAuth2 (variável: GOOGLE_SSO_CLIENT_ID).</summary>
    private const string ClientId = "";

    /// <summary>Client Secret do projeto OAuth2 (variável: GOOGLE_SSO_CLIENT_SECRET).</summary>
    private const string ClientSecret = "";

    /// <summary>
    /// Domínio Workspace restrito, ex.: <c>empresa.com</c> (variável: GOOGLE_SSO_HOSTED_DOMAIN).
    /// Deixe vazio para permitir qualquer domínio Google.
    /// </summary>
    private const string HostedDomain = "";

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// ID Token JWT válido emitido pelo Google — necessário para <c>GetOAuthSSOAsync</c>.
    /// Gere via fluxo Google Sign-In no frontend; válido por aproximadamente 1 hora.
    /// </summary>
    public const string TestIdToken = "";

    /// <summary>
    /// Access Token OAuth2 válido — necessário para <c>GetUserProfileAsync</c>.
    /// Obtido junto com o ID Token no fluxo completo de autorização OAuth2.
    /// </summary>
    public const string TestAccessToken = "";

    // ─────────────────────────────────────────────────────────────────────────────

    private readonly string? _originalClientId;
    private readonly string? _originalClientSecret;
    private readonly string? _originalHostedDomain;

    /// <summary>Indica se as credenciais de configuração estão preenchidas.</summary>
    public bool HasCredentials => !string.IsNullOrWhiteSpace(ClientId);

    /// <summary>Indica se um ID Token de teste está disponível.</summary>
    public bool HasIdToken => !string.IsNullOrWhiteSpace(TestIdToken);

    /// <summary>Indica se um Access Token de teste está disponível.</summary>
    public bool HasAccessToken => !string.IsNullOrWhiteSpace(TestAccessToken);

    /// <summary>
    /// Persiste as variáveis de ambiente atuais e aplica as credenciais configuradas.
    /// </summary>
    public GoogleSSOFixture()
    {
        _originalClientId = Environment.GetEnvironmentVariable("GOOGLE_SSO_CLIENT_ID");
        _originalClientSecret = Environment.GetEnvironmentVariable("GOOGLE_SSO_CLIENT_SECRET");
        _originalHostedDomain = Environment.GetEnvironmentVariable("GOOGLE_SSO_HOSTED_DOMAIN");

        if (!string.IsNullOrWhiteSpace(ClientId))
            Environment.SetEnvironmentVariable("GOOGLE_SSO_CLIENT_ID", ClientId);

        if (!string.IsNullOrWhiteSpace(ClientSecret))
            Environment.SetEnvironmentVariable("GOOGLE_SSO_CLIENT_SECRET", ClientSecret);

        Environment.SetEnvironmentVariable("GOOGLE_SSO_HOSTED_DOMAIN", HostedDomain);
    }

    /// <summary>
    /// Restaura as variáveis de ambiente ao valor original pré-teste.
    /// </summary>
    public void Dispose()
    {
        Environment.SetEnvironmentVariable("GOOGLE_SSO_CLIENT_ID", _originalClientId);
        Environment.SetEnvironmentVariable("GOOGLE_SSO_CLIENT_SECRET", _originalClientSecret);
        Environment.SetEnvironmentVariable("GOOGLE_SSO_HOSTED_DOMAIN", _originalHostedDomain);
    }
}
