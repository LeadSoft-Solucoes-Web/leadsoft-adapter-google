using LeadSoft.Adapter.Google.ReCaptchaService.DTOs;

namespace LeadSoft.Adapter.Google.ReCaptchaService;

/// <summary>
/// Define o contrato para integração com o serviço de verificação do Google reCAPTCHA v3.
/// </summary>
public interface IReCAPTCHA : IDisposable
{
    /// <summary>
    /// Valida o token de resposta do usuário junto à API do Google reCAPTCHA v3.
    /// </summary>
    /// <param name="aDtoRequest">Dados da requisição contendo o token e a chave secreta.</param>
    /// <returns>Resultado da verificação com indicador de sucesso e possíveis códigos de erro.</returns>
    Task<DTOSiteVerifyResponse> PostSiteVerifyAsync(DTOSiteVerifyRequest aDtoRequest);
}
