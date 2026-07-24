using System.Runtime.Serialization;

namespace LeadSoft.Adapter.Google.Workspace.Contracts;

/// <summary>
/// Representa as informações básicas do usuário autenticado via Google SSO.
/// Retornado por <c>IGoogleSSO.GetOAuthSSOAsync</c> após validação bem-sucedida do ID Token.
/// </summary>
[Serializable]
[DataContract]
public sealed record DTOGoogleUserResponse(string Id, string Email, string Name, string Picture, string Domain)
{
    /// <summary>
    /// Identificador único do usuário no Google (campo <c>sub</c> do JWT).
    /// </summary>
    [DataMember]
    public string Id { get; init; } = Id;

    /// <summary>
    /// Endereço de e-mail do usuário autenticado.
    /// </summary>
    [DataMember]
    public string Email { get; init; } = Email;

    /// <summary>
    /// Nome completo do usuário autenticado.
    /// </summary>
    [DataMember]
    public string Name { get; init; } = Name;

    /// <summary>
    /// URL da foto de perfil do usuário.
    /// </summary>
    [DataMember]
    public string Picture { get; init; } = Picture;

    /// <summary>
    /// Domínio do Google Workspace ao qual o usuário pertence (ex.: <c>empresa.com</c>).
    /// Vazio quando o usuário é uma conta pessoal do Google (<c>@gmail.com</c>).
    /// </summary>
    [DataMember]
    public string Domain { get; init; } = Domain;
}
