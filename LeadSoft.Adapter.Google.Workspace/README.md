[← Voltar ao repositório](../README.md)

# LeadSoft® Google Workspace Integration Adapter
## [LeadSoft.Adapter.Google.Workspace](https://www.nuget.org/packages/LeadSoft.Adapter.Google.Workspace)

Adapter .NET para integrar com os serviços de autenticação do Google Workspace — Single Sign-On via OAuth2 e consulta ao perfil expandido do usuário via People API.  
Fornece uma camada leve, testável e orientada a interfaces para validação de ID Tokens e recuperação de dados de perfil, encapsulando o SDK do Google, validação de JWT e tratamento de erros de forma consistente para aplicações .NET 10.

> Este pacote é um tributo independente e não é afiliado oficialmente ao [Google](https://www.google.com/).  
> Somos gratos pela disponibilização das APIs públicas do [Google Identity](https://developers.google.com/identity) e do [Google People API](https://developers.google.com/people). Ao utilizar este pacote, você concorda automaticamente com os [Termos de Serviço do Google](https://policies.google.com/terms).

#### [NuGet.Org: LeadSoft.Adapter.Google.Workspace](https://www.nuget.org/packages/LeadSoft.Adapter.Google.Workspace)
#### [GitHub Repo: leadsoft-adapter-google](https://github.com/LeadSoft-Solucoes-Web/leadsoft-adapter-google)

## Principais características
- Compatível com .NET 10.0.
- Autenticação SSO via Google OAuth2 com validação de ID Token JWT.
- Suporte a lista de domínios permitidos (Workspace e/ou contas pessoais `@gmail.com`).
- Consulta ao perfil expandido do usuário via Google People API (nome, foto, telefone, aniversário).
- Chamadas assíncronas com `async`/`await`.
- Fácil integração com injeção de dependência (`IServiceCollection`).
- Interface `IGoogleSSO` para facilitar testes e mocking.
- Suporte a registro como `Scoped` ou `Singleton`.
- Open Source (MIT License).

## Variáveis de ambiente

| Variável | Obrigatória | Descrição |
|----------|-------------|-----------|
| `GOOGLE_SSO_CLIENT_ID` | Sim | Client ID do projeto OAuth2 no Google Cloud Console. |
| `GOOGLE_SSO_CLIENT_SECRET` | Sim | Client Secret do projeto OAuth2 no Google Cloud Console. |
| `GOOGLE_SSO_HOSTED_DOMAIN` | Não | Lista de domínios permitidos separados por vírgula. Quando definido, bloqueia contas fora da lista. Ver detalhes abaixo. |

#### Detalhes de `GOOGLE_SSO_HOSTED_DOMAIN`

Aceita um ou mais domínios separados por vírgula. Inclua `gmail.com` para aceitar contas pessoais do Google.

| Valor | Comportamento |
|-------|---------------|
| _(não definido)_ | Aceita qualquer conta Google. |
| `empresa.com` | Aceita apenas contas do domínio Workspace `empresa.com`. |
| `empresa.com,parceiro.com` | Aceita contas dos domínios Workspace `empresa.com` e `parceiro.com`. |
| `empresa.com,gmail.com` | Aceita contas Workspace de `empresa.com` **e** contas pessoais `@gmail.com`. |

> **Nota técnica:** contas `@gmail.com` não possuem o campo `HostedDomain` no token JWT do Google. O adapter identifica essas contas pelo campo `email` do token — que é assinado e verificado pelo Google, portanto não é spoofável.

## Métodos disponíveis

### `IGoogleSSO`

- `Task<DTOGoogleUserResponse?> GetOAuthSSOAsync(string idToken, CancellationToken cancellationToken = default)`
    - Valida o ID Token JWT emitido pelo Google após o login do usuário.
    - Verifica assinatura, expiração e Client ID automaticamente via SDK do Google.
    - Opcionalmente restringe login a uma lista de domínios (Workspace e/ou `@gmail.com`).
    - Lança `UnauthorizedAppException` quando o token é inválido ou expirou.
    - Lança `ForbiddenAppException` quando o domínio do usuário não é permitido.

- `Task<DTOGoogleUserExpandedResponse?> GetUserProfileAsync(string accessToken, CancellationToken cancellationToken = default)`
    - Consulta o perfil detalhado do usuário autenticado via Google People API.
    - Retorna dados como telefone e data de nascimento, quando disponíveis.
    - Retorna `null` em caso de falha ou token inválido, sem lançar exceções.

## Instalação
Pelo CLI do .NET:

```bash
dotnet add package LeadSoft.Adapter.Google.Workspace
```

Ou via NuGet Package Manager no Visual Studio (pesquise por `LeadSoft.Adapter.Google.Workspace`).

## Uso básico (exemplo)
Abaixo um exemplo genérico de como registrar e usar o adapter em uma aplicação ASP.NET Core / Console com DI.

```csharp
// Program.cs (exemplo)
using LeadSoft.Adapter.Google.Workspace;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Google SSO
builder.Services.AddGoogleSSOApi();        // scoped (padrão)
// builder.Services.AddGoogleSSOApi(true); // singleton

WebApplication app = builder.Build();
app.Run();
```

Exemplo de uso via injeção de dependência — validação de ID Token (SSO):

```csharp
// Em um Controller, Service ou Minimal API endpoint:
public class AuthService(IGoogleSSO googleSSO)
{
    public async Task<DTOGoogleUserResponse?> LoginAsync(string idToken)
    {
        // Valida o token e retorna os dados básicos do usuário
        return await googleSSO.GetOAuthSSOAsync(idToken);
    }
}
```

Exemplo de uso — perfil expandido (após autenticação OAuth2 completa):

```csharp
public class PerfilService(IGoogleSSO googleSSO)
{
    public async Task<DTOGoogleUserExpandedResponse?> ObterPerfilAsync(string accessToken)
    {
        // Retorna dados detalhados do usuário via People API
        return await googleSSO.GetUserProfileAsync(accessToken);
    }
}
```

## DTOs de retorno

### `DTOGoogleUserResponse` — `GetOAuthSSOAsync`
| Propriedade | Tipo     | Descrição                                                              |
|-------------|----------|------------------------------------------------------------------------|
| `Id`        | `string` | Identificador único do usuário no Google (campo `sub` do JWT)          |
| `Email`     | `string` | Endereço de e-mail do usuário                                          |
| `Name`      | `string` | Nome completo do usuário                                               |
| `Picture`   | `string` | URL da foto de perfil                                                  |
| `Domain`    | `string` | Domínio Workspace do usuário (vazio para contas `@gmail.com`)          |

### `DTOGoogleUserExpandedResponse` — `GetUserProfileAsync`
| Propriedade   | Tipo        | Descrição                                                          |
|---------------|-------------|--------------------------------------------------------------------|
| `Id`          | `string`    | Identificador único do usuário (ResourceName sem prefixo `people/`) |
| `Email`       | `string`    | Endereço de e-mail principal                                       |
| `Name`        | `string`    | Nome de exibição completo                                          |
| `Picture`     | `string`    | URL da foto de perfil                                              |
| `PhoneNumber` | `string?`   | Número de telefone (quando disponível)                             |
| `Birthday`    | `DateTime?` | Data de nascimento (quando disponível)                             |

## Configuração recomendada
- Configure `GOOGLE_SSO_CLIENT_ID` via variáveis de ambiente ou cofre seguro (Azure Key Vault, AWS Secrets Manager) — nunca em código-fonte.
- Defina `GOOGLE_SSO_HOSTED_DOMAIN` para restringir o login. Use vírgula para múltiplos domínios; inclua `gmail.com` para aceitar também contas pessoais do Google.
- Propague `CancellationToken` em todas as chamadas assíncronas.
- Capture e logue erros com `ILogger<T>` para diagnóstico e rastreabilidade.

## Boas práticas de integração
- Valide o ID Token no servidor imediatamente após recebê-lo do frontend — nunca confie apenas na validação client-side.
- Não exponha diretamente os DTOs HTTP ao seu domínio — mapeie para modelos de domínio quando necessário.
- Use `GetUserProfileAsync` apenas quando precisar de dados adicionais (telefone, aniversário) além dos fornecidos pelo ID Token.
- Prefira `GetOAuthSSOAsync` para fluxos de login simples — é mais rápido e não requer um Access Token separado.

## Versionamento e Compatibilidade
- Destinado a .NET 10.0. Verifique a compatibilidade do pacote com seu projeto.
- Siga versionamento semântico: breaking changes → major, novas features → minor, correções → patch.

## Documentação de referência

| Recurso | Link |
|---------|------|
| Google Identity — Sign In with Google | [developers.google.com/identity/gsi/web](https://developers.google.com/identity/gsi/web) |
| Google OAuth2 — ID Token | [developers.google.com/identity/openid-connect/openid-connect](https://developers.google.com/identity/openid-connect/openid-connect) |
| Google People API | [developers.google.com/people](https://developers.google.com/people) |
| Google Cloud Console | [console.cloud.google.com](https://console.cloud.google.com) |

## Licença
Consulte o arquivo de licença no repositório para detalhes sobre uso e redistribuição.

---

LeadSoft.Adapter.Google.Workspace — adapter simples e testável para autenticação via Google SSO e acesso ao perfil do usuário em aplicações .NET 10.

### Development  
Desenvolvido pelo time da LeadSoft® Soluções Web.
* [Lucas Resende Tavares](https://www.linkedin.com/in/lucasrtavares/)

#### Nossa empresa
LeadSoft Soluções Web Ltda  
CNPJ 38.043.762/0001-48

#### Como nos encontrar:
- [Nosso Site](https://www.leadsoft.inf.br)
- [GitHub](https://github.com/LeadSoft-Solucoes-Web)
- [LinkedIn](https://www.linkedin.com/company/leadsoft-solucoes-web)

#### INFORMAÇÕES DE CONTATO — Se você tiver alguma dúvida sobre estes Termos ou Serviços, entre em contato conosco em
[developers@leadsoft.inf.br](mailto:developers@leadsoft.inf.br).
