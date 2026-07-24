using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using LeadSoft.Adapter.Google.Workspace.Contracts;
using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Exceptions;
using LeadSoft.Common.Library.Extensions;
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
///   <item><description><c>GOOGLE_SSO_HOSTED_DOMAIN</c> — (Opcional) Domínio Workspace permitido (ex.: <c>empresa.com</c>). Quando definido, bloqueia contas fora do domínio.</description></item>
/// </list>
/// </remarks>
public sealed partial class GoogleSSO : IGoogleSSO
{
    private bool disposedValue;

    /// <inheritdoc/>
    public async Task<DTOGoogleUserResponse?> GetOAuthSSOAsync(string idToken, CancellationToken cancellationToken = default)
    {
        if (idToken.IsNothing())
            throw new BadRequestAppException("Id Token não pode ser nulo ou vazio.");

        try
        {
            GoogleJsonWebSignature.ValidationSettings validationSettings = new()
            {
                Audience = [EnvUtil.Get(EnvVariable.Google_SSO_Client_Id)]
            };

            // O SDK do Google valida a assinatura, expiração e o Client ID automaticamente
            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings).ConfigureAwait(false);

            // Restrição opcional: bloqueia usuários fora do domínio Workspace configurado (ex.: impede @gmail.com)
            string hostedDomain = EnvUtil.Get(EnvVariable.Google_SSO_Hosted_Domain);
            if (hostedDomain.IsSomething() && payload.HostedDomain != hostedDomain)
                throw new ForbiddenAppException($"Tentativa de login bloqueada: usuário pertence ao domínio '{payload.HostedDomain}', esperado '{hostedDomain}'.");

            return new(payload.Subject, payload.Email, payload.Name, payload.Picture, payload.HostedDomain ?? string.Empty);
        }
        catch (ForbiddenAppException)
        {
            throw;
        }
        catch (InvalidJwtException)
        {
            throw new UnauthorizedAppException("Token do Google inválido ou expirado.");
        }
        catch (Exception)
        {
            throw new BadRequestAppException("Erro inesperado ao validar token do Google.");
        }
    }

    /// <inheritdoc/>
    public async Task<DTOGoogleUserExpandedResponse?> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        if (accessToken.IsNothing())
            return null;

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
                return null;

            string id = profile.ResourceName?.Replace("people/", "", StringComparison.Ordinal) ?? string.Empty;
            string email = profile.EmailAddresses?.FirstOrDefault()?.Value ?? string.Empty;
            string name = profile.Names?.FirstOrDefault()?.DisplayName ?? string.Empty;
            string picture = profile.Photos?.FirstOrDefault()?.Url ?? string.Empty;
            string? phoneNumber = profile.PhoneNumbers?.FirstOrDefault()?.Value;

            DateTime? birthday = null;
            Date? googleBirthday = profile.Birthdays?.FirstOrDefault()?.Date;
            if (googleBirthday?.Year is not null && googleBirthday?.Month is not null && googleBirthday?.Day is not null)
                birthday = new DateTime(googleBirthday.Year.Value, googleBirthday.Month.Value, googleBirthday.Day.Value);

            return new(id, email, name, picture, phoneNumber, birthday);
        }
        catch
        {
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
