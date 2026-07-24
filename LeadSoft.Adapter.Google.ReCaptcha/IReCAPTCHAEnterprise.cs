using LeadSoft.Adapter.Google.ReCaptcha.Contracts;

namespace LeadSoft.Adapter.Google.ReCaptcha;

/// <summary>
/// Define o contrato para integração com o serviço Google reCAPTCHA Enterprise.
/// </summary>
/// <remarks>
/// Forneça as credenciais do reCAPTCHA Enterprise via variáveis de ambiente.
/// <list type="bullet">
///   <item><term><c>GOOGLE_RECAPTCHA_ENTERPRISE_SITE_KEY</c></term><description>Chave pública do site reCAPTCHA Enterprise.</description></item>
///   <item><term><c>GOOGLE_RECAPTCHA_ENTERPRISE_PROJECT_ID</c></term><description>ID do projeto no Google Cloud.</description></item>
///   <item><term><c>GOOGLE_RECAPTCHA_ENTERPRISE_API_KEY</c></term><description>Chave de API do Google Cloud Console.</description></item>
/// </list>
/// </remarks>
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
