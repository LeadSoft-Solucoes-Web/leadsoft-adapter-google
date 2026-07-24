using System.Runtime.Serialization;

namespace LeadSoft.Adapter.Google.Workspace.Contracts;

/// <summary>
/// Representa a requisição de login via Google SSO enviada pelo cliente para o servidor.
/// Contém o ID Token emitido pelo Google após a autenticação do usuário no frontend.
/// </summary>
[Serializable]
[DataContract]
public sealed record DTOGoogleLoginRequest()
{
    /// <summary>
    /// Token JWT emitido pelo Google OAuth2 após a autenticação do usuário.
    /// Deve ser enviado ao servidor para validação via <c>IGoogleSSO.GetOAuthSSOAsync</c>.
    /// </summary>
    [DataMember]
    public string IdToken { get; init; } = string.Empty;

    /// <summary>
    /// Endereço de e-mail do usuário, conforme retornado pelo Google.
    /// </summary>
    [DataMember]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário, conforme retornado pelo Google.
    /// </summary>
    [DataMember]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// URL da foto de perfil do usuário, conforme retornada pelo Google.
    /// </summary>
    [DataMember]
    public string Picture { get; init; } = string.Empty;

    /// <summary>
    /// Número de telefone do usuário. Opcional.
    /// </summary>
    [DataMember]
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Data de nascimento do usuário. Opcional.
    /// </summary>
    [DataMember]
    public DateTime? Birthday { get; init; }
}
