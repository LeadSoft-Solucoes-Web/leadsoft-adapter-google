# LeadSoft® Google reCAPTCHA Integration Adapter
## [LeadSoft.Adapter.Google.ReCaptcha](https://www.nuget.org/packages/LeadSoft.Adapter.Google.ReCaptcha)

Adapter .NET para integrar com os serviços do Google reCAPTCHA — tanto a versão padrão v3 quanto o reCAPTCHA Enterprise.  
Fornece uma camada leve, testável e orientada a interfaces para validação de tokens e criação de avaliações de risco, encapsulando chamadas HTTP, mapeamento de modelos e tratamento de erros de forma consistente para aplicações .NET 10.

> Este pacote é um tributo independente e não é afiliado oficialmente ao [Google](https://www.google.com/).  
> Somos gratos pela disponibilização das APIs públicas do [Google reCAPTCHA](https://developers.google.com/recaptcha) e do [reCAPTCHA Enterprise](https://cloud.google.com/recaptcha). Ao utilizar este pacote, você concorda automaticamente com os [Termos de Serviço do Google](https://policies.google.com/terms).

#### [Nuget.Org: LeadSoft.Adapter.Google.ReCaptcha](https://www.nuget.org/packages/LeadSoft.Adapter.Google.ReCaptcha)
#### [GitHub Repo: leadsoft-adapter-google](https://github.com/LeadSoft-Solucoes-Web/leadsoft-adapter-google)

> [!WARNING]
> **Migração necessária:** Este pacote substitui o antigo [`LeadSoft.Adapter.Google`](https://www.nuget.org/packages/LeadSoft.Adapter.Google/).  
> O pacote anterior está descontinuado e não receberá mais atualizações. Recomendamos a migração para `LeadSoft.Adapter.Google.ReCaptcha` o quanto antes.  
> As interfaces (`IReCAPTCHA`, `IReCAPTCHAEnterprise`) e os namespaces foram revisados — consulte a seção **Uso básico** abaixo para adaptar seu código.

## Principais características
- Compatível com .NET 10.0.
- Suporte ao Google reCAPTCHA v3 (`siteverify`) e ao reCAPTCHA Enterprise (`assessments`).
- Chamadas assíncronas com `async`/`await`.
- Fácil integração com injeção de dependência (`IServiceCollection`).
- Interfaces `IReCAPTCHA` e `IReCAPTCHAEnterprise` para facilitar testes e mocking.
- Suporte a registro como `Scoped` ou `Singleton`.
- Tratamento centralizado de erros e respostas HTTP.
- Preparado para extensão com políticas de resiliência (ex.: Polly).
- Open Source (MIT License).

## Métodos disponíveis

### reCAPTCHA v3 — `IReCAPTCHA`

- `Task<DTOSiteVerifyResponse> PostSiteVerifyAsync(DTOSiteVerifyRequest aDtoRequest)`
    - Valida o token de resposta do usuário junto à API do Google reCAPTCHA v3.
    - Retorna o resultado da verificação com indicador de sucesso e possíveis códigos de erro.

### reCAPTCHA Enterprise — `IReCAPTCHAEnterprise`

- `Task<DTOAssessmentResp> CreateAssessmentsAsync(DTOAssessmentReq aDtoRequest, string apiKey)`
    - Cria uma avaliação (Assessment) da probabilidade de um evento ser legítimo.
    - Retorna propriedades do token, validade e plataforma de origem (web, Android ou iOS).
    - Lança `InvalidOperationException` quando a API retorna um status HTTP de erro.

## Instalação
Pelo CLI do .NET:

```bash
dotnet add package LeadSoft.Adapter.Google.ReCaptcha
```

Ou via NuGet Package Manager no Visual Studio (pesquise por `LeadSoft.Adapter.Google.ReCaptcha`).

## Uso básico (exemplo)
Abaixo um exemplo genérico de como registrar e usar o adapter em uma aplicação ASP.NET Core / Console com DI.

```csharp
// Program.cs (exemplo)
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LeadSoft.Adapter.Google.ReCaptcha;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// reCAPTCHA v3
builder.Services.AddReCAPTCHAApi();        // scoped (padrão)
// builder.Services.AddReCAPTCHAApi(true); // singleton

// reCAPTCHA Enterprise (requer o ID do projeto no Google Cloud)
builder.Services.AddReCAPTCHAEnterpriseApi("meu-projeto-google-cloud");
// builder.Services.AddReCAPTCHAEnterpriseApi("meu-projeto-google-cloud", useSingleton: true);

WebApplication app = builder.Build();
app.Run();
```

Exemplo de chamada via injeção de dependência — reCAPTCHA v3:

```csharp
// Em um Controller, Service ou Minimal API endpoint:
public class ValidacaoService(IReCAPTCHA recaptcha)
{
    public async Task<bool> ValidarTokenAsync(string secret, string token, string ip = "")
    {
        DTOSiteVerifyResponse resultado = await recaptcha.PostSiteVerifyAsync(
            new DTOSiteVerifyRequest(secret, token, ip));

        return resultado.Success;
    }
}
```

Exemplo de chamada via injeção de dependência — reCAPTCHA Enterprise:

```csharp
// Em um Controller, Service ou Minimal API endpoint:
public class AvaliacaoService(IReCAPTCHAEnterprise recaptchaEnterprise)
{
    public async Task<bool> AvaliarEventoAsync(string token, string siteKey, string apiKey)
    {
        DTOAssessmentResp avaliacao = await recaptchaEnterprise.CreateAssessmentsAsync(
            new DTOAssessmentReq(token, siteKey), apiKey);

        return avaliacao.TokenProperties.Valid;
    }
}
```

## DTOs de requisição

### `DTOSiteVerifyRequest` — reCAPTCHA v3
| Propriedade | Tipo     | Descrição                                               |
|-------------|----------|---------------------------------------------------------|
| `Secret`    | `string` | Obrigatório. Chave secreta compartilhada com o reCAPTCHA |
| `Response`  | `string` | Obrigatório. Token de resposta do usuário               |
| `RemoteIp`  | `string` | Opcional. Endereço IP do usuário                        |

### `DTOAssessmentReq` — reCAPTCHA Enterprise
| Propriedade | Tipo                    | Descrição                                      |
|-------------|-------------------------|------------------------------------------------|
| `Event`     | `DTOAssessmentEventReq` | Evento contendo o token e a chave do site      |

### `DTOAssessmentEventReq` — reCAPTCHA Enterprise
| Propriedade | Tipo     | Descrição                                                        |
|-------------|----------|------------------------------------------------------------------|
| `Token`     | `string` | Token de resposta do usuário gerado pelo reCAPTCHA Enterprise    |
| `SiteKey`   | `string` | Chave do site utilizada para invocar o reCAPTCHA Enterprise      |

## DTOs de retorno

### `DTOSiteVerifyResponse` — reCAPTCHA v3
| Propriedade   | Tipo             | Descrição                                               |
|---------------|------------------|---------------------------------------------------------|
| `Success`     | `bool`           | Indica se o desafio foi concluído com sucesso           |
| `ChallengeTs` | `DateTime`       | Data e hora em que o desafio foi carregado              |
| `Hostname`    | `string`         | Host do site onde o reCAPTCHA foi resolvido             |
| `ErrorCodes`  | `IList<string>`  | Códigos de erro, quando houver falha na validação       |

Códigos de erro possíveis:

| Código                    | Descrição                                            |
|---------------------------|------------------------------------------------------|
| `missing-input-secret`    | O parâmetro `secret` está ausente                    |
| `invalid-input-secret`    | O parâmetro `secret` é inválido ou malformado        |
| `missing-input-response`  | O parâmetro `response` está ausente                  |
| `invalid-input-response`  | O parâmetro `response` é inválido ou malformado      |
| `bad-request`             | A requisição é inválida ou malformada                |
| `timeout-or-duplicate`    | O token expirou ou já foi utilizado anteriormente    |

### `DTOAssessmentResp` — reCAPTCHA Enterprise
| Propriedade       | Tipo                            | Descrição                                          |
|-------------------|---------------------------------|----------------------------------------------------|
| `Name`            | `string`                        | Nome do recurso da avaliação gerado pela API       |
| `Event`           | `DTOAssessmentEventResp`        | Evento reCAPTCHA que originou a avaliação          |
| `TokenProperties` | `DTOAssessmentTokenPropertiesResp` | Propriedades e validade do token avaliado       |

### `DTOAssessmentTokenPropertiesResp` — reCAPTCHA Enterprise
| Propriedade          | Tipo       | Descrição                                                              |
|----------------------|------------|------------------------------------------------------------------------|
| `Valid`              | `bool`     | Indica se o token é válido                                             |
| `InvalidReason`      | `string`   | Motivo pelo qual o token foi considerado inválido                      |
| `Hostname`           | `string`   | Host do site onde o reCAPTCHA foi resolvido (integrações web)          |
| `AndroidPackageName` | `string`   | Nome do pacote Android (integrações mobile Android)                    |
| `IosBundleId`        | `string`   | Bundle ID do app iOS (integrações mobile iOS)                          |
| `Action`             | `string`   | Ação associada ao token, conforme definida na integração client-side   |
| `CreateTime`         | `DateTime` | Data e hora em que o token foi criado                                  |

## Configuração recomendada
- Use `IServiceCollection` com `AddReCAPTCHAApi()` ou `AddReCAPTCHAEnterpriseApi()` — evita problemas com ciclo de vida do `HttpClient`.
- Configure `Timeout` e cabeçalhos necessários conforme os limites das APIs do Google.
- Adicione políticas de resiliência com Polly (`Retry`, `Circuit Breaker`) para chamadas de rede.
- Propague `CancellationToken` em todas as chamadas assíncronas.
- Armazene chaves secretas e API Keys em variáveis de ambiente ou em cofre seguro (ex.: Azure Key Vault, AWS Secrets Manager) — nunca em código-fonte.

## Boas práticas de integração
- Valide o token no servidor imediatamente após o envio do formulário — nunca confie apenas na validação client-side.
- Para o reCAPTCHA v3, defina um limiar de pontuação (score) adequado ao risco da operação (ex.: `>= 0.5` para login, `>= 0.7` para transações).
- Para o reCAPTCHA Enterprise, inspecione `TokenProperties.Valid` e `TokenProperties.Action` para garantir que o token corresponde à ação esperada.
- Não exponha diretamente os DTOs HTTP ao seu domínio — mapeie para modelos de domínio quando necessário.
- Capture e logue erros com `ILogger<T>` para diagnóstico: inclua status code e corpo quando aplicável.

## Gerador de Token para Testes

O repositório inclui o arquivo [`LeadSoft.Google.Tests/GoogleReCAPTCHA.html`](LeadSoft.Google.Tests/GoogleReCAPTCHA.html) — uma página HTML standalone que pode ser aberta diretamente no navegador para gerar tokens de resposta do reCAPTCHA v3 sem necessidade de servidor.

**Como usar:**
1. Abra o arquivo `GoogleReCAPTCHA.html` no navegador (duplo clique ou `Ctrl+O`).
2. Informe a **Site Key pública** do seu projeto reCAPTCHA.
3. Clique em **Gerar Token** — o token aparece no campo de saída.
4. Copie o token e use-o como valor de `Response` no `DTOSiteVerifyRequest`, combinando com sua **Secret Key** para testar o método `PostSiteVerifyAsync`.

> O token gerado é válido por aproximadamente 2 minutos. Gere um novo token para cada teste.

## Testes
- Injete um `HttpMessageHandler` falso no `HttpClient` para simular respostas da API do Google sem realizar chamadas reais.
- As interfaces `IReCAPTCHA` e `IReCAPTCHAEnterprise` permitem criar mocks facilmente com bibliotecas como Moq ou NSubstitute.
- Cubra cenários: token válido, token inválido, token expirado, token duplicado, erros de rede e respostas HTTP de erro.
- Use fixtures com exemplos de payloads JSON conhecidos para manter os testes determinísticos.

## Versionamento e Compatibilidade
- Destinado a .NET 10.0. Verifique a compatibilidade do pacote com seu projeto.
- Siga versionamento semântico: breaking changes → major, novas features → minor, correções → patch.

## Documentação de referência

| Recurso | Link |
|---------|------|
| Google reCAPTCHA v3 | [developers.google.com/recaptcha/docs/v3](https://developers.google.com/recaptcha/docs/v3) |
| Google reCAPTCHA Enterprise | [cloud.google.com/recaptcha/docs/overview](https://cloud.google.com/recaptcha/docs/overview) |
| reCAPTCHA Enterprise REST API | [projects.assessments](https://cloud.google.com/recaptcha/docs/reference/rest/v1/projects.assessments) |
| reCAPTCHA Admin Console | [google.com/recaptcha/admin](https://www.google.com/recaptcha/admin) |

## Licença
Consulte o arquivo de licença no repositório para detalhes sobre uso e redistribuição.

---

LeadSoft.Adapter.Google.ReCaptcha — adapter simples e testável para integração com o Google reCAPTCHA v3 e reCAPTCHA Enterprise em aplicações .NET 10.

### Development  
Desenvolvido pelo time da LeadSoft® Soluções Web.
* [Lucas Resende Tavares](https://www.linkedin.com/in/lucasrtavares/)
* ~~Frederico Ferreira Bitencourt~~
* ~~Pedro Foresti Leão~~

#### Nossa empresa
LeadSoft Soluções Web Ltda  
CNPJ 38.043.762/0001-48

#### Como nos encontrar:
- [Nosso Site](https://www.leadsoft.inf.br)
- [GitHub](https://github.com/LeadSoft-Solucoes-Web)
- [LinkedIn](https://www.linkedin.com/company/leadsoft-solucoes-web)
- [Behance](https://www.behance.net/leadsofsolue)
- [YouTube](https://www.youtube.com/@LeadsoftSolucoesWeb)
- [Instagram](https://www.instagram.com/leadsoft.inf/)
- [Facebook](https://www.facebook.com/leadsoft.inf.br)

#### INFORMAÇÕES DE CONTATO  Se você tiver alguma dúvida sobre estes Termos ou Serviços, entre em contato conosco em
[developers@leadsoft.inf.br](mailto:developers@leadsoft.inf.br).
