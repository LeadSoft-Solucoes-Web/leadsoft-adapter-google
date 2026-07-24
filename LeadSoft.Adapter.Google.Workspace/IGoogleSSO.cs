using LeadSoft.Adapter.Google.Workspace.Contracts;

namespace LeadSoft.Adapter.Google.Workspace;

/// <summary>
/// Define o contrato para autenticação via Google SSO (Single Sign-On) e consulta ao perfil do usuário.
/// </summary>
public interface IGoogleSSO : IDisposable
{
    /// <summary>
    /// Valida um ID Token do Google e retorna as informações básicas do usuário autenticado.
    /// </summary>
    /// <remarks>
    /// Variáveis de ambiente requeridas:
    /// <list type="bullet">
    ///   <item><description><c>GOOGLE_SSO_CLIENT_ID</c> — Client ID do projeto OAuth2 no Google Cloud Console.</description></item>
    ///   <item><description><c>GOOGLE_SSO_CLIENT_SECRET</c> — Client Secret do projeto OAuth2 no Google Cloud Console.</description></item>
    ///   <item><description><c>GOOGLE_SSO_HOSTED_DOMAIN</c> — (Opcional) Domínio Workspace permitido (ex.: <c>empresa.com</c>). Quando definido, bloqueia contas fora do domínio.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="idToken">Token JWT emitido pelo Google OAuth após o login do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>
    /// Um <see cref="DTOGoogleUserResponse"/> com os dados do usuário autenticado.
    /// </returns>
    /// <exception cref="LeadSoft.Common.Library.Exceptions.BadRequestAppException">Lançada quando o token é nulo, vazio ou a validação falha de forma inesperada.</exception>
    /// <exception cref="LeadSoft.Common.Library.Exceptions.UnauthorizedAppException">Lançada quando o token JWT é inválido ou expirou.</exception>
    /// <exception cref="LeadSoft.Common.Library.Exceptions.ForbiddenAppException">Lançada quando o domínio do usuário não corresponde ao domínio Workspace configurado.</exception>
    Task<DTOGoogleUserResponse?> GetOAuthSSOAsync(string idToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta o perfil expandido do usuário autenticado via Google People API.
    /// </summary>
    /// <param name="accessToken">Access Token OAuth2 obtido após a autenticação do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>
    /// Um <see cref="DTOGoogleUserExpandedResponse"/> com informações detalhadas do usuário,
    /// ou <see langword="null"/> se o acesso falhar ou o token for inválido.
    /// </returns>
    Task<DTOGoogleUserExpandedResponse?> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken = default);
}