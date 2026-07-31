using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using LeadSoft.Adapter.Google.Workspace.Contracts;
using LeadSoft.Common.GlobalDomain.Entities;
using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Exceptions;
using LeadSoft.Common.Library.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection;

namespace LeadSoft.Adapter.Google.Workspace;

/// <summary>
/// Implementação do adapter para autenticação via Google SSO (Single Sign-On) e acesso à People API.
/// Encapsula a validação de ID Token (<c>GoogleJsonWebSignature</c>) e a consulta ao perfil expandido do usuário.
/// </summary>
/// <remarks>
/// Variáveis de ambiente requeridas:
/// <list type="bullet">
///   <item><description><c>GOOGLE_SSO_CLIENT_ID</c> — Client ID do projeto OAuth2 no Google Cloud Console.</description></item>
///   <item><description><c>GOOGLE_SSO_CLIENT_SECRET</c> — Client Secret do projeto OAuth2 no Google Cloud Console.</description></item>
///   <item><description><c>GOOGLE_SSO_HOSTED_DOMAIN</c> — (Opcional) Lista de domínios permitidos separados por vírgula (ex.: <c>empresa.com,parceiro.com</c>). Use <c>gmail.com</c> na lista para aceitar contas pessoais do Google. Quando definido, bloqueia contas fora da lista.</description></item>
/// </list>
/// <para>
/// Em ambientes de desenvolvimento e staging, o log inclui stack trace completo.
/// Em produção, apenas a mensagem de erro é registrada para evitar exposição de dados internos.
/// </para>
/// </remarks>
public sealed partial class GoogleSSO : IGoogleSSO
{
    private readonly ILogger _logger;
    private bool disposedValue;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="GoogleSSO"/>.
    /// </summary>
    /// <param name="logger">Logger opcional. Quando omitido, nenhum log é emitido.</param>
    public GoogleSSO(ILogger<GoogleSSO>? logger = null)
    {
        _logger = logger ?? NullLogger<GoogleSSO>.Instance;
    }

    /// <inheritdoc/>
    public async Task<DTOGoogleUserResponse?> GetOAuthSSOAsync(string idToken, CancellationToken cancellationToken = default)
    {
        if (idToken.IsNothing())
            throw new BadRequestAppException("Id Token não pode ser nulo ou vazio.");

        _logger.LogDebug("Iniciando validação de ID Token Google SSO.");

        try
        {
            GoogleJsonWebSignature.ValidationSettings validationSettings = new()
            {
                Audience = [EnvUtil.Get(EnvVariable.Google_SSO_Client_Id)]
            };

            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings).ConfigureAwait(false);

            string hostedDomainEnv = EnvUtil.Get(EnvVariable.Google_SSO_Hosted_Domain);
            if (hostedDomainEnv.IsSomething())
            {
                string[] allowed = hostedDomainEnv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                bool isWorkspace = allowed.Contains(payload.HostedDomain, StringComparer.OrdinalIgnoreCase) ||
                                    (payload.HostedDomain.IsNothing() &&
                                     payload.Email?.GetDomain().Equals(IGoogleSSO.GmailDomain, StringComparison.OrdinalIgnoreCase) == false &&
                                     allowed.Contains(payload.Email?.GetDomain(), StringComparer.OrdinalIgnoreCase) == true);

                bool isGmail = allowed.Contains(IGoogleSSO.GmailDomain, StringComparer.OrdinalIgnoreCase)
                                   && payload.HostedDomain.IsNothing()
                                   && payload.Email?.GetDomain().Equals(IGoogleSSO.GmailDomain, StringComparison.OrdinalIgnoreCase) == true;

                if (!isWorkspace && !isGmail)
                {
                    string blockedDomain = payload.HostedDomain ?? payload.Email ?? "desconhecido";
                    _logger.LogWarning("Login SSO bloqueado: domínio '{Domain}' não está na lista de domínios autorizados.", blockedDomain);
                    throw new ForbiddenAppException($"Acesso negado: domínio '{blockedDomain}' não autorizado.");
                }
            }

            _logger.LogInformation("Login via Google SSO bem-sucedido. Usuário: {Email}, Domínio: {Domain}.",
                payload.Email, payload.HostedDomain ?? IGoogleSSO.GmailDomain);

            return new(payload.Subject, payload.Email, payload.Name, payload.Picture, payload.HostedDomain ?? string.Empty);
        }
        catch (ForbiddenAppException)
        {
            throw;
        }
        catch (InvalidJwtException e)
        {
            _logger.LogWarning("Token Google SSO inválido ou expirado. {Message}", e.Message);
            throw new UnauthorizedAppException("Token do Google inválido ou expirado. Solicite um novo login.");
        }
        catch (Exception e)
        {
            if (EnvUtil.IsProduction())
                _logger.LogError("Erro inesperado ao validar token do Google SSO. {Message}", e.Message);
            else
                _logger.LogError(e, "Erro inesperado ao validar token do Google SSO. {Message}", e.Message);

            throw new BadRequestAppException("Erro ao processar o login com Google. Tente novamente.");
        }
    }

    /// <inheritdoc/>
    public async Task<DTOGoogleUserExpandedResponse?> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        if (accessToken.IsNothing())
            return null;

        _logger.LogDebug("Iniciando consulta ao perfil expandido via People API.");

        try
        {
            GoogleCredential credential = GoogleCredential.FromAccessToken(accessToken);

            // O SDK do Google gerencia internamente o ciclo de vida do HttpClient
            using PeopleServiceService peopleService = new(new()
            {
                HttpClientInitializer = credential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            PeopleResource.GetRequest request = peopleService.People.Get("people/me");
            request.PersonFields = "names,emailAddresses,photos,birthdays,phoneNumbers";

            Person profile = await request.ExecuteAsync(cancellationToken).ConfigureAwait(false);

            if (profile is null)
            {
                _logger.LogWarning("People API retornou perfil nulo para o access token fornecido.");
                return null;
            }

            string id = profile.ResourceName?.Replace("people/", "", StringComparison.Ordinal) ?? string.Empty;
            string email = profile.EmailAddresses?.FirstOrDefault()?.Value ?? string.Empty;
            string name = profile.Names?.FirstOrDefault()?.DisplayName ?? string.Empty;
            string picture = profile.Photos?.FirstOrDefault()?.Url ?? string.Empty;
            string? phoneNumber = profile.PhoneNumbers?.FirstOrDefault()?.Value;

            DateTime? birthday = null;
            Date? googleBirthday = profile.Birthdays?.FirstOrDefault()?.Date;
            if (googleBirthday?.Year is not null && googleBirthday?.Month is not null && googleBirthday?.Day is not null)
                birthday = new DateTime(googleBirthday.Year.Value, googleBirthday.Month.Value, googleBirthday.Day.Value);

            _logger.LogInformation("Perfil expandido obtido com sucesso. Usuário: {Email}.", email);

            return new(id, email, name, picture, phoneNumber, birthday);
        }
        catch (Exception e)
        {
            if (EnvUtil.IsProduction())
                _logger.LogError("Erro ao obter perfil expandido via People API. {Message}", e.Message);
            else
                _logger.LogError(e, "Erro ao obter perfil expandido via People API. {Message}", e.Message);

            return null;
        }
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
            disposedValue = true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
