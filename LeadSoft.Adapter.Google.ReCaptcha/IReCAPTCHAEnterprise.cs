using LeadSoft.Adapter.Google.ReCaptcha.DTOs;

namespace LeadSoft.Adapter.Google.ReCaptcha;

/// <summary>
/// Define o contrato para integração com o serviço Google reCAPTCHA Enterprise.
/// </summary>
public interface IReCAPTCHAEnterprise : IDisposable
{
    /// <summary>
    /// Cria uma avaliação (Assessment) da probabilidade de um evento ser legítimo.
    /// </summary>
    /// <param name="aDtoRequest">Dados do evento reCAPTCHA Enterprise para avaliação.</param>
    /// <param name="apiKey">Chave de API do Google Cloud Console.</param>
    /// <returns>Resultado da avaliação com propriedades do token e indicador de validade.</returns>
    /// <exception cref="InvalidOperationException">Lançado quando a API retorna um status HTTP de erro.</exception>
    Task<DTOAssessmentResp> CreateAssessmentsAsync(DTOAssessmentReq aDtoRequest, string apiKey);
}
