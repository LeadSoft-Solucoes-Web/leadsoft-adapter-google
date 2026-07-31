using LeadSoft.Adapter.Google.ReCaptcha.Contracts;
using LeadSoft.Common.Library;
using LeadSoft.Common.Library.EnvUtils;
using LeadSoft.Common.Library.Exceptions;
using LeadSoft.Common.Library.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection;

namespace LeadSoft.Adapter.Google.ReCaptcha;

/// <summary>
/// Implementação do adapter para integração com o Google reCAPTCHA Enterprise.
/// Encapsula as chamadas HTTP à API de avaliação de eventos (<c>assessments</c>).
/// </summary>
/// <remarks>
/// Forneça as credenciais do reCAPTCHA Enterprise via variáveis de ambiente:
/// <list type="bullet">
///   <item><term><c>GOOGLE_RECAPTCHA_ENTERPRISE_SITE_KEY</c></term><description>Chave pública do site reCAPTCHA Enterprise.</description></item>
///   <item><term><c>GOOGLE_RECAPTCHA_ENTERPRISE_PROJECT_ID</c></term><description>ID do projeto no Google Cloud.</description></item>
///   <item><term><c>GOOGLE_RECAPTCHA_ENTERPRISE_API_KEY</c></term><description>Chave de API do Google Cloud Console.</description></item>
/// </list>
/// <para>
/// Em ambientes de desenvolvimento e staging, o log inclui stack trace completo.
/// Em produção, apenas a mensagem de erro é registrada para evitar exposição de dados internos.
/// </para>
/// </remarks>
public sealed partial class ReCAPTCHAEnterprise : IReCAPTCHAEnterprise
{
    private readonly HttpClient _Client = null;
    private readonly ILogger _logger;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ReCAPTCHAEnterprise"/> para o projeto informado.
    /// </summary>
    /// <param name="projectId">ID do projeto no Google Cloud associado ao reCAPTCHA Enterprise.</param>
    /// <param name="logger">Logger opcional. Quando omitido, nenhum log é emitido.</param>
    public ReCAPTCHAEnterprise(string projectId, ILogger<ReCAPTCHAEnterprise>? logger = null)
    {
        _logger = logger ?? NullLogger<ReCAPTCHAEnterprise>.Instance;
        _Client = new HttpClient
        {
            BaseAddress = new Uri(Google_ReCaptcha_BaseURL.Enterprise.Fill(projectId))
        };
        _Client.DefaultRequestHeaders.UserAgent.ParseAdd($"LeadSoft.Adapter.Google.ReCaptchaService/{Assembly.GetExecutingAssembly().GetName().Version} (+https://www.nuget.org/packages/LeadSoft.Adapter.Google.ReCaptchaService)");
    }

    /// <summary>
    /// Construtor interno para injeção de <see cref="HttpMessageHandler"/> em testes unitários.
    /// </summary>
    internal ReCAPTCHAEnterprise(string projectId, HttpMessageHandler handler, ILogger<ReCAPTCHAEnterprise>? logger = null)
    {
        _logger = logger ?? NullLogger<ReCAPTCHAEnterprise>.Instance;
        _Client = new HttpClient(handler)
        {
            BaseAddress = new Uri(Google_ReCaptcha_BaseURL.Enterprise.Fill(projectId))
        };
        _Client.DefaultRequestHeaders.UserAgent.ParseAdd($"LeadSoft.Adapter.Google.ReCaptchaService/{Assembly.GetExecutingAssembly().GetName().Version} (+https://www.nuget.org/packages/LeadSoft.Adapter.Google.ReCaptchaService)");
    }

    /// <inheritdoc/>
    public async Task<DTOAssessmentResp> CreateAssessmentsAsync(DTOAssessmentReq aDtoRequest, string apiKey)
    {
        _logger.LogDebug("Iniciando avaliação reCAPTCHA Enterprise.");

        HttpResponseMessage response = await HttpCall.SendAsync(
            _Client,
            HttpMethod.Post,
            Google_ReCaptcha_Enterprise_EndPoint.Post_Assessment.Fill(apiKey),
            aObject: aDtoRequest);

        try
        {
            if (response.IsSuccessStatusCode)
            {
                DTOAssessmentResp dto = await response.ReadContentToObjectAsync<DTOAssessmentResp>();

                if (dto.TokenProperties?.Valid == true)
                    _logger.LogInformation("Avaliação reCAPTCHA Enterprise concluída. Token válido. Host: {Hostname}.", dto.TokenProperties.Hostname);
                else
                    _logger.LogWarning("Avaliação reCAPTCHA Enterprise: token inválido. Motivo: {Reason}.", dto.TokenProperties?.InvalidReason ?? "desconhecido");

                return dto;
            }

            DTOAssessmentErrorResp error = await response.ReadContentToObjectAsync<DTOAssessmentErrorResp>().ConfigureAwait(false);
            _logger.LogWarning("API reCAPTCHA Enterprise retornou erro HTTP {StatusCode}: {ErrorMsg}.", (int)response.StatusCode, error.GetErrorMsg());
            throw new BadRequestAppException(error.GetErrorMsg());
        }
        catch (BadRequestAppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            if (EnvUtil.IsProduction())
                _logger.LogError("Erro inesperado ao criar avaliação reCAPTCHA Enterprise. {Message}", ex.Message);
            else
                _logger.LogError(ex, "Erro inesperado ao criar avaliação reCAPTCHA Enterprise. {Message}", ex.Message);

            throw new AppException("Erro interno ao processar a avaliação reCAPTCHA Enterprise. Tente novamente.");
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _Client.Dispose();
    }
}
