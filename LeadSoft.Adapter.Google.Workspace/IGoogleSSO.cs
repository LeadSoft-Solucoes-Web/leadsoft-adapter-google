using LeadSoft.Adapter.Google.Workspace.Contracts;

namespace LeadSoft.Adapter.Google.Workspace;

/// <summary>
/// Define o contrato para autenticação via Google SSO (Single Sign-On) e consulta ao perfil do usuário.
/// </summary>
public interface IGoogleSSO : IDisposable
{
    /// <summary>
    /// Valor reservado para a variável <c>GOOGLE_SSO_HOSTED_DOMAIN</c> que autoriza contas pessoais do Google (<c>@gmail.com</c>).
    /// </summary>
    /// <remarks>
    /// Contas <c>@gmail.com</c> não possuem o campo <c>HostedDomain</c> no token JWT do Google.
    /// O adapter identifica essas contas pelo campo <c>email</c> do token ao encontrar este valor na lista de domínios.
    /// </remarks>
    public const string GmailDomain = "gmail.com";
    /// <summary>
    /// Valida um ID Token do Google e retorna as informações básicas do usuário autenticado.
    /// </summary>
    /// <remarks>
    /// Variáveis de ambiente requeridas:
    /// <list type="bullet">
    ///   <item><description><c>GOOGLE_SSO_CLIENT_ID</c> — Client ID do projeto OAuth2 no Google Cloud Console.</description></item>
    ///   <item><description><c>GOOGLE_SSO_CLIENT_SECRET</c> — Client Secret do projeto OAuth2 no Google Cloud Console.</description></item>
    ///   <item><description>
    ///     <c>GOOGLE_SSO_HOSTED_DOMAIN</c> — (Opcional) Lista de domínios permitidos separados por vírgula
    ///     (ex.: <c>empresa.com,parceiro.com</c>). Inclua <c>gmail.com</c> para aceitar contas pessoais do Google
    ///     (ex.: <c>empresa.com,gmail.com</c>). Quando definido, bloqueia contas fora da lista.
    ///     Contas <c>@gmail.com</c> são identificadas pelo e-mail, pois não possuem o campo <c>HostedDomain</c> no token.
    ///   </description></item>
    /// </list>
    /// </remarks>
    /// <param name="idToken">Token JWT emitido pelo Google OAuth após o login do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação assíncrona.</param>
    /// <returns>
    /// Um <see cref="DTOGoogleUserResponse"/> com os dados do usuário autenticado.
    /// </returns>
    /// <exception cref="LeadSoft.Common.Library.Exceptions.BadRequestAppException">Lançada quando o token é nulo, vazio ou a validação falha de forma inesperada.</exception>
    /// <exception cref="LeadSoft.Common.Library.Exceptions.UnauthorizedAppException">Lançada quando o token JWT é inválido ou expirou.</exception>
    /// <exception cref="LeadSoft.Common.Library.Exceptions.ForbiddenAppException">Lançada quando o domínio do usuário não está na lista de domínios autorizados.</exception>
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