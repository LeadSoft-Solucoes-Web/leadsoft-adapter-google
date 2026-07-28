using Microsoft.Extensions.DependencyInjection;

namespace LeadSoft.Adapter.Google.Workspace;

/// <summary>
/// Extensões de injeção de dependência para os serviços do Google Workspace.
/// </summary>
public static class Injection
{
    /// <summary>
    /// Registra o serviço de SSO do Google Workspace no <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">A coleção de serviços onde o adapter será registrado.</param>
    /// <param name="useSingleton">
    /// <see langword="true"/> para registrar como singleton (compartilhado em toda a aplicação);
    /// <see langword="false"/> para registrar como scoped (por requisição). Padrão: <see langword="false"/>.
    /// </param>
    /// <remarks>
    /// Variáveis de ambiente requeridas:
    /// <list type="bullet">
    ///   <item><description><c>GOOGLE_SSO_CLIENT_ID</c> — Client ID do projeto OAuth2 no Google Cloud Console.</description></item>
    ///   <item><description><c>GOOGLE_SSO_CLIENT_SECRET</c> — Client Secret do projeto OAuth2 no Google Cloud Console.</description></item>
    ///   <item><description>
    ///     <c>GOOGLE_SSO_HOSTED_DOMAIN</c> — (Opcional) Lista de domínios permitidos separados por vírgula
    ///     (ex.: <c>empresa.com,parceiro.com</c>). Inclua <c>gmail.com</c> para aceitar também contas pessoais do Google
    ///     (ex.: <c>empresa.com,gmail.com</c>). Quando definido, bloqueia contas fora da lista.
    ///   </description></item>
    /// </list>
    /// </remarks>
    public static void AddGoogleSSO(this IServiceCollection services, bool useSingleton = false)
    {
        if (useSingleton)
            services.AddSingleton<IGoogleSSO, GoogleSSO>();
        else
            services.AddScoped<IGoogleSSO, GoogleSSO>();
    }
}
