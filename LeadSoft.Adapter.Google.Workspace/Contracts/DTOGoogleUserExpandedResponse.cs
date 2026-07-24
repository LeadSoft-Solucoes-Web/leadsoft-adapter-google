using System.Runtime.Serialization;

namespace LeadSoft.Adapter.Google.Workspace.Contracts;

/// <summary>
/// Representa o perfil expandido do usuário autenticado via Google People API.
/// Retornado por <c>IGoogleSSO.GetUserProfileAsync</c> quando disponível.
/// </summary>
[Serializable]
[DataContract]
public sealed record DTOGoogleUserExpandedResponse(string Id, string Email, string Name, string Picture, string? PhoneNumber = null, DateTime? Birthday = null)
{
    /// <summary>
    /// Identificador único do usuário no Google (campo <c>resourceName</c> da People API, sem o prefixo <c>people/</c>).
    /// </summary>
    [DataMember]
    public string Id { get; init; } = Id;

    /// <summary>
    /// Endereço de e-mail principal do usuário.
    /// </summary>
    [DataMember]
    public string Email { get; init; } = Email;

    /// <summary>
    /// Nome de exibição completo do usuário.
    /// </summary>
    [DataMember]
    public string Name { get; init; } = Name;

    /// <summary>
    /// URL da foto de perfil do usuário.
    /// </summary>
    [DataMember]
    public string Picture { get; init; } = Picture;

    /// <summary>
    /// Número de telefone do usuário, quando cadastrado e disponível. Opcional.
    /// </summary>
    [DataMember]
    public string? PhoneNumber { get; init; } = PhoneNumber;

    /// <summary>
    /// Data de nascimento do usuário, quando cadastrada e disponível. Opcional.
    /// </summary>
    [DataMember]
    public DateTime? Birthday { get; init; } = Birthday;
}
