using LeadSoft.Adapter.Google.ReCaptcha.Contracts;
using LeadSoft.Common.Library;
using LeadSoft.Common.Library.Exceptions;
using LeadSoft.Common.Library.Extensions;
using System.Reflection;

namespace LeadSoft.Adapter.Google.ReCaptcha;

/// <summary>
/// Implementação do adapter para integração com o Google reCAPTCHA v3.
/// Encapsula as chamadas HTTP à API de verificação de token (<c>siteverify</c>).
/// </summary>
/// <remarks>
/// Configuração do serviço via variáveis de ambiente:
/// A <b>Secret Key</b> (chave privada do servidor) pode ser fornecida diretamente no construtor
/// ou via variável de ambiente <c>GOOGLE_RECAPTCHA_SECRET_KEY</c>.
///
/// <para>⚠️ Não confundir com a <i>Site Key</i> (chave pública usada no HTML) —
/// a validação server-side exige a <b>Secret Key</b>.</para>
/// </remarks>
public sealed partial class ReCAPTCHA : IReCAPTCHA
{
    private readonly HttpClient _Client = null;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ReCAPTCHA"/> com um <see cref="HttpClient"/> configurado.
    /// </summary>
    public ReCAPTCHA()
    {
        _Client = new HttpClient
        {
            BaseAddress = new Uri(Google_ReCaptcha_BaseURL.v3v2)
        };
        _Client.DefaultRequestHeaders.UserAgent.ParseAdd($"LeadSoft.Adapter.Google.ReCaptchaService/{Assembly.GetExecutingAssembly().GetName().Version} (+https://www.nuget.org/packages/LeadSoft.Adapter.Google.ReCaptchaService)");
    }

    /// <summary>
    /// Construtor interno para injeção de <see cref="HttpMessageHandler"/> em testes unitários.
    /// </summary>
    internal ReCAPTCHA(HttpMessageHandler handler)
    {
        _Client = new HttpClient(handler)
        {
            BaseAddress = new Uri(Google_ReCaptcha_BaseURL.v3v2)
        };
        _Client.DefaultRequestHeaders.UserAgent.ParseAdd($"LeadSoft.Adapter.Google.ReCaptchaService/{Assembly.GetExecutingAssembly().GetName().Version} (+https://www.nuget.org/packages/LeadSoft.Adapter.Google.ReCaptchaService)");
    }

    /// <inheritdoc/>
    public async Task<DTOSiteVerifyResponse> PostSiteVerifyAsync(DTOSiteVerifyRequest aDtoRequest)
    {

        HttpResponseMessage response = await HttpCall.SendAsync(_Client, HttpMethod.Post, string.Format(Google_ReCaptcha_EndPoint.Post_SiteVerify_v1,
                                                                                aDtoRequest.Secret,
                                                                                aDtoRequest.Response,
                                                                                aDtoRequest.RemoteIp));

        try
        {
            if (response.IsSuccessStatusCode)
                return await response.ReadContentToObjectAsync<DTOSiteVerifyResponse>();

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
