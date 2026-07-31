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
/// Implementação do adapter para integração com o Google reCAPTCHA v3.
/// Encapsula as chamadas HTTP à API de verificação de token (<c>siteverify</c>).
/// </summary>
/// <remarks>
/// A <b>Secret Key</b> (chave privada do servidor) pode ser fornecida diretamente no construtor
/// ou via variável de ambiente <c>GOOGLE_RECAPTCHA_SECRET_KEY</c>.
///
/// <para>⚠️ Não confundir com a <i>Site Key</i> (chave pública usada no HTML) —
/// a validação server-side exige a <b>Secret Key</b>.</para>
///
/// <para>
/// Em ambientes de desenvolvimento e staging, o log inclui stack trace completo.
/// Em produção, apenas a mensagem de erro é registrada para evitar exposição de dados internos.
/// </para>
/// </remarks>
public sealed partial class ReCAPTCHA : IReCAPTCHA
{
    private readonly HttpClient _Client = null;
    private readonly ILogger _logger;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ReCAPTCHA"/> com um <see cref="HttpClient"/> configurado.
    /// </summary>
    /// <param name="logger">Logger opcional. Quando omitido, nenhum log é emitido.</param>
    public ReCAPTCHA(ILogger<ReCAPTCHA>? logger = null)
    {
        _logger = logger ?? NullLogger<ReCAPTCHA>.Instance;
        _Client = new HttpClient
        {
            BaseAddress = new Uri(Google_ReCaptcha_BaseURL.v3v2)
        };
        _Client.DefaultRequestHeaders.UserAgent.ParseAdd($"LeadSoft.Adapter.Google.ReCaptchaService/{Assembly.GetExecutingAssembly().GetName().Version} (+https://www.nuget.org/packages/LeadSoft.Adapter.Google.ReCaptchaService)");
    }

    /// <summary>
    /// Construtor interno para injeção de <see cref="HttpMessageHandler"/> em testes unitários.
    /// </summary>
    internal ReCAPTCHA(HttpMessageHandler handler, ILogger<ReCAPTCHA>? logger = null)
    {
        _logger = logger ?? NullLogger<ReCAPTCHA>.Instance;
        _Client = new HttpClient(handler)
        {
            BaseAddress = new Uri(Google_ReCaptcha_BaseURL.v3v2)
        };
        _Client.DefaultRequestHeaders.UserAgent.ParseAdd($"LeadSoft.Adapter.Google.ReCaptchaService/{Assembly.GetExecutingAssembly().GetName().Version} (+https://www.nuget.org/packages/LeadSoft.Adapter.Google.ReCaptchaService)");
    }

    /// <inheritdoc/>
    public async Task<DTOSiteVerifyResponse> PostSiteVerifyAsync(DTOSiteVerifyRequest aDtoRequest)
    {
        _logger.LogDebug("Iniciando verificação de token reCAPTCHA v3.");

        HttpResponseMessage response = await HttpCall.SendAsync(_Client, HttpMethod.Post,
            string.Format(Google_ReCaptcha_EndPoint.Post_SiteVerify_v1,
                aDtoRequest.Secret,
                aDtoRequest.Response,
                aDtoRequest.RemoteIp));
        try
        {
            if (response.IsSuccessStatusCode)
            {
                DTOSiteVerifyResponse dto = await response.ReadContentToObjectAsync<DTOSiteVerifyResponse>();

                if (dto.Success)
                    _logger.LogInformation("Token reCAPTCHA verificado com sucesso. Host: {Hostname}.", dto.Hostname);
                else
                    _logger.LogWarning("Token reCAPTCHA inválido. Códigos: {Codes}.", string.Join(", ", dto.ErrorCodes ?? []));

                return dto;
            }

            DTOAssessmentErrorResp error = await response.ReadContentToObjectAsync<DTOAssessmentErrorResp>().ConfigureAwait(false);
            _logger.LogWarning("API reCAPTCHA retornou erro HTTP {StatusCode}: {ErrorMsg}.", (int)response.StatusCode, error.GetErrorMsg());
            throw new BadRequestAppException(error.GetErrorMsg());
        }
        catch (BadRequestAppException)
        {
            throw;
        }
        catch (Exception ex)
        {
            if (EnvUtil.IsProduction())
                _logger.LogError("Erro inesperado ao verificar token reCAPTCHA. {Message}", ex.Message);
            else
                _logger.LogError(ex, "Erro inesperado ao verificar token reCAPTCHA. {Message}", ex.Message);

            throw new AppException("Erro interno ao verificar o token reCAPTCHA. Tente novamente.");
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _Client.Dispose();
    }
}
