using LeadSoft.Adapter.Google.ReCaptcha;
using LeadSoft.Adapter.Google.ReCaptchaService.DTOs;
using LeadSoft.Common.Library;
using LeadSoft.Common.Library.Extensions;

namespace LeadSoft.Adapter.Google.ReCaptchaService
{
    /// <summary>
    /// Implementação do adapter para integração com o Google reCAPTCHA Enterprise.
    /// Encapsula as chamadas HTTP à API de avaliação de eventos (<c>assessments</c>).
    /// </summary>
    public partial class ReCAPTCHAEnterprise : IReCAPTCHAEnterprise
    {
        private readonly HttpClient _Client = null;
        private const string _BaseURL = "https://recaptchaenterprise.googleapis.com/v1/projects/{0}/";

        /// <summary>
        /// Inicializa uma nova instância de <see cref="ReCAPTCHAEnterprise"/> para o projeto informado.
        /// </summary>
        /// <param name="projectId">ID do projeto no Google Cloud associado ao reCAPTCHA Enterprise.</param>
        public ReCAPTCHAEnterprise(string projectId)
        {
            _Client = new HttpClient
            {
                BaseAddress = new Uri(string.Format(_BaseURL, projectId))
            };
        }

        /// <summary>
        /// Construtor interno para injeção de <see cref="HttpMessageHandler"/> em testes unitários.
        /// </summary>
        internal ReCAPTCHAEnterprise(string projectId, HttpMessageHandler handler)
        {
            _Client = new HttpClient(handler)
            {
                BaseAddress = new Uri(string.Format(_BaseURL, projectId))
            };
        }

        /// <inheritdoc/>
        public async Task<DTOAssessmentResp> CreateAssessmentsAsync(DTOAssessmentReq aDtoRequest, string apiKey)
        {
            HttpResponseMessage response = await HttpCall.SendAsync(
                _Client,
                HttpMethod.Post,
                string.Format(Google_ReCaptcha_Enterprise_EndPoint.Post_Assessment, apiKey),
                aObject: aDtoRequest);

            if (response.IsSuccessStatusCode)
            {
                return await response.ReadContentToObjectAsync<DTOAssessmentResp>();
            }
            else
            {
                throw new InvalidOperationException(await response.Content.ReadAsStringAsync());
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _Client.Dispose();
        }
    }
}
