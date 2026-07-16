using System.Net;

namespace LeadSoft.Google.Tests.Helpers;

/// <summary>
/// Implementação fake de <see cref="HttpMessageHandler"/> para simular respostas HTTP em testes unitários
/// sem realizar chamadas reais à rede.
/// </summary>
internal sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _responseBody;

    /// <param name="responseBody">Corpo JSON da resposta simulada.</param>
    /// <param name="statusCode">Código HTTP da resposta simulada. Padrão: 200 OK.</param>
    internal FakeHttpMessageHandler(string responseBody, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        _responseBody = responseBody;
        _statusCode = statusCode;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_responseBody, System.Text.Encoding.UTF8, "application/json")
        });
}
