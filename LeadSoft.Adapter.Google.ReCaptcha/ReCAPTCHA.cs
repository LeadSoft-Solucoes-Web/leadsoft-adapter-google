using LeadSoft.Adapter.Google.ReCaptcha;
using LeadSoft.Adapter.Google.ReCaptchaService.DTOs;
using LeadSoft.Common.Library;
using LeadSoft.Common.Library.Extensions;

namespace LeadSoft.Adapter.Google.ReCaptchaService
{
    /// <summary>
    /// Implementação do adapter para integração com o Google reCAPTCHA v3.
    /// Encapsula as chamadas HTTP à API de verificação de token (<c>siteverify</c>).
    /// </summary>
    public partial class ReCAPTCHA : IReCAPTCHA
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
        }

        /// <inheritdoc/>
        public async Task<DTOSiteVerifyResponse> PostSiteVerifyAsync(DTOSiteVerifyRequest aDtoRequest) =>
            await (await HttpCall.SendAsync(_Client, HttpMethod.Post, string.Format(Google_ReCaptcha_EndPoint.Post_SiteVerify_v1,
                                                                                    aDtoRequest.Secret,
                                                                                    aDtoRequest.Response,
                                                                                    aDtoRequest.RemoteIp))).ReadContentToObjectAsync<DTOSiteVerifyResponse>();

        /// <inheritdoc/>
        public void Dispose()
        {
            _Client.Dispose();
        }
    }
}
