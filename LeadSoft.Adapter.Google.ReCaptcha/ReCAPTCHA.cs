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
        private const string _BaseURL = "https://www.google.com/recaptcha/api/";

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ReCAPTCHA"/> com um <see cref="HttpClient"/> configurado.
        /// </summary>
        public ReCAPTCHA()
        {
            _Client = new HttpClient
            {
                BaseAddress = new Uri(_BaseURL)
            };
        }

        /// <summary>
        /// Construtor interno para injeção de <see cref="HttpMessageHandler"/> em testes unitários.
        /// </summary>
        internal ReCAPTCHA(HttpMessageHandler handler)
        {
            _Client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_BaseURL)
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
