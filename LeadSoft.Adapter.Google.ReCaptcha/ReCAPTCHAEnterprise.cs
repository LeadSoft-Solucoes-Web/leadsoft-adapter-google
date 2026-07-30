using LeadSoft.Adapter.Google.ReCaptcha.Contracts;
using LeadSoft.Common.Library;
using LeadSoft.Common.Library.Exceptions;
using LeadSoft.Common.Library.Extensions;
using System.Reflection;

namespace LeadSoft.Adapter.Google.ReCaptcha;

/// <summary>
/// Implementação do adapter para integração com o Google reCAPTCHA Enterprise.
/// Encapsula as chamadas HTTP à API de avaliação de eventos (<c>assessments</c>).
/// </summary>
/// <remarks>
/// Forneça as credenciais do reCAPTCHA Enterprise via variáveis de ambiente.
/// <list type="bullet">
///   <item><term><c>GOOGLE_RECAPTCHA_ENTERPRISE_SITE_KEY</c></term><description>Chave pública do site reCAPTCHA Enterprise.</description></item>
///   <item><term><c>GOOGLE_RECAPTCHA_ENTERPRISE_PROJECT_ID</c></term><description>ID do projeto no Google Cloud.</description></item>
///   <item><term><c>GOOGLE_RECAPTCHA_ENTERPRISE_API_KEY</c></term><description>Chave de API do Google Cloud Console.</description></item>
/// </list>
/// </remarks>
public sealed partial class ReCAPTCHAEnterprise : IReCAPTCHAEnterprise
{
    private readonly HttpClient _Client = null;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ReCAPTCHAEnterprise"/> para o projeto informado.
    /// </summary>
    /// <param name="projectId">ID do projeto no Google Cloud associado ao reCAPTCHA Enterprise.</param>
    public ReCAPTCHAEnterprise(string projectId)
    {
        _Client = new HttpClient
        {
            BaseAddress = new Uri(Google_ReCaptcha_BaseURL.Enterprise.Fill(projectId))
        };
        _Client.DefaultRequestHeaders.UserAgent.ParseAdd($"LeadSoft.Adapter.Google.ReCaptchaService/{Assembly.GetExecutingAssembly().GetName().Version} (+https://www.nuget.org/packages/LeadSoft.Adapter.Google.ReCaptchaService)");
    }

    /// <summary>
    /// Construtor interno para injeção de <see cref="HttpMessageHandler"/> em testes unitários.
    /// </summary>
    internal ReCAPTCHAEnterprise(string projectId, HttpMessageHandler handler)
    {
        _Client = new HttpClient(handler)
        {
            BaseAddress = new Uri(Google_ReCaptcha_BaseURL.Enterprise.Fill(projectId))
        };
        _Client.DefaultRequestHeaders.UserAgent.ParseAdd($"LeadSoft.Adapter.Google.ReCaptchaService/{Assembly.GetExecutingAssembly().GetName().Version} (+https://www.nuget.org/packages/LeadSoft.Adapter.Google.ReCaptchaService)");
    }

    /// <inheritdoc/>
    public async Task<DTOAssessmentResp> CreateAssessmentsAsync(DTOAssessmentReq aDtoRequest, string apiKey)
    {
        HttpResponseMessage response = await HttpCall.SendAsync(
            _Client,
            HttpMethod.Post,
            Google_ReCaptcha_Enterprise_EndPoint.Post_Assessment.Fill(apiKey),
            aObject: aDtoRequest);

        try
        {
            if (response.IsSuccessStatusCode)
                return await response.ReadContentToObjectAsync<DTOAssessmentResp>();

            DTOAssessmentErrorResp error = await response.ReadContentToObjectAsync<DTOAssessmentErrorResp>().ConfigureAwait(false);
            throw new BadRequestAppException($"{error.Code} {error.Status}: {error.Message}");
        }
        catch (Exception ex)
        {
            throw new AppException(ex.Message, await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _Client.Dispose();
    }
}
