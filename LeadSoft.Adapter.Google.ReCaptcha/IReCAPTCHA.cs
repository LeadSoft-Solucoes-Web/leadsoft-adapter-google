using LeadSoft.Adapter.Google.ReCaptcha.Contracts;

namespace LeadSoft.Adapter.Google.ReCaptcha;

/// <summary>
/// Define o contrato para integração com o serviço de verificação do Google reCAPTCHA v3.
/// </summary>
/// /// <remarks>
/// Configuração do serviço via variáveis de ambiente:
/// A <b>Secret Key</b> (chave privada do servidor) pode ser fornecida diretamente no construtor
/// ou via variável de ambiente <c>GOOGLE_RECAPTCHA_SECRET_KEY</c>.
///
/// <para>⚠️ Não confundir com a <i>Site Key</i> (chave pública usada no HTML) —
/// a validação server-side exige a <b>Secret Key</b>.</para>
/// </remarks>
public interface IReCAPTCHA : IDisposable
{
    /// <summary>
    /// Valida o token de resposta do usuário junto à API do Google reCAPTCHA v3.
    /// </summary>
    /// <param name="aDtoRequest">Dados da requisição contendo o token e a chave secreta.</param>
    /// <returns>Resultado da verificação com indicador de sucesso e possíveis códigos de erro.</returns>
    Task<DTOSiteVerifyResponse> PostSiteVerifyAsync(DTOSiteVerifyRequest aDtoRequest);
}
