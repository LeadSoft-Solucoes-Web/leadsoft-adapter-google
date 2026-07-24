# LeadSoft® Google API Adapters

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
[![NuGet](https://img.shields.io/badge/NuGet-packages-blue.svg)](https://www.nuget.org/profiles/LeadSoft)

Repositório mono-repo com os adapters .NET da LeadSoft® para integração com APIs do Google.  
Cada projeto é publicado de forma independente como pacote NuGet, com interface própria, injeção de dependência e documentação XML completa.

> Os pacotes são contribuições independentes e não são afiliados oficialmente ao [Google](https://www.google.com/).  
> Ao utilizá-los, você concorda com os [Termos de Serviço do Google](https://policies.google.com/terms) aplicáveis a cada API.

---

## Pacotes disponíveis

| Pacote | NuGet | Descrição |
|--------|-------|-----------|
| [`LeadSoft.Adapter.Google.ReCaptcha`](LeadSoft.Adapter.Google.ReCaptcha/) | [![NuGet](https://img.shields.io/nuget/v/LeadSoft.Adapter.Google.ReCaptcha)](https://www.nuget.org/packages/LeadSoft.Adapter.Google.ReCaptcha) | Validação de tokens reCAPTCHA v3 e reCAPTCHA Enterprise |
| [`LeadSoft.Adapter.Google.Workspace`](LeadSoft.Adapter.Google.Workspace/) | [![NuGet](https://img.shields.io/nuget/v/LeadSoft.Adapter.Google.Workspace)](https://www.nuget.org/packages/LeadSoft.Adapter.Google.Workspace) | Autenticação SSO e consulta ao perfil via Google Workspace |

---

## Estrutura do repositório

```
leadsoft-adapter-google/
├── LeadSoft.Adapter.Google.ReCaptcha/    # Pacote: reCAPTCHA v3 + Enterprise
│   ├── Attibutes/                        # Atributos para validação em propriedades de DTOs
│   ├── Contracts/                        # DTOs de requisição e resposta
│   ├── IReCAPTCHA.cs                     # Interface pública
│   ├── IReCAPTCHAEnterprise.cs           # Interface pública (Enterprise)
│   ├── ReCAPTCHA.cs                      # Implementação
│   ├── ReCAPTCHAEnterprise.cs            # Implementação (Enterprise)
│   └── Injection.cs                      # Extensões IServiceCollection
│
├── LeadSoft.Adapter.Google.Workspace/    # Pacote: Google SSO + People API
│   ├── Contracts/                        # DTOs de requisição e resposta
│   ├── IGoogleSSO.cs                     # Interface pública
│   ├── GoogleSSO.cs                      # Implementação
│   └── Injection.cs                      # Extensões IServiceCollection
│
├── LeadSoft.Google.Tests/               # Suite de testes xUnit (xunit.v3 + Moq)
├── LeadSoft.Google.slnx                 # Arquivo de solução (.NET 10 slnx)
├── LeadSoft.png                         # Ícone dos pacotes NuGet
├── LICENSE                              # MIT License
└── README.md                            # Este arquivo
```

---

## Instalação rápida

### reCAPTCHA

```bash
dotnet add package LeadSoft.Adapter.Google.ReCaptcha
```

### Google Workspace SSO

```bash
dotnet add package LeadSoft.Adapter.Google.Workspace
```

---

## Registro na injeção de dependência

```csharp
// Program.cs
using LeadSoft.Adapter.Google.ReCaptcha;
using LeadSoft.Adapter.Google.Workspace;

// reCAPTCHA v3
builder.Services.AddReCAPTCHAApi();

// reCAPTCHA Enterprise (requer o ID do projeto no Google Cloud)
builder.Services.AddReCAPTCHAEnterpriseApi("meu-projeto-google-cloud");

// Google SSO
builder.Services.AddGoogleSSOApi();
```

Consulte o README de cada pacote para exemplos de uso completos, variáveis de ambiente e DTOs disponíveis:

- [Documentação: LeadSoft.Adapter.Google.ReCaptcha](LeadSoft.Adapter.Google.ReCaptcha/README.md)
- [Documentação: LeadSoft.Adapter.Google.Workspace](LeadSoft.Adapter.Google.Workspace/README.md)

---

## Pré-requisitos de desenvolvimento

| Requisito | Versão mínima |
|-----------|--------------|
| [.NET SDK](https://dotnet.microsoft.com/download/dotnet/10.0) | 10.0 |
| Visual Studio | 2022 v17.14+ (ou VS Code com extensão C#) |
| Git | qualquer versão recente |

---

## Como compilar localmente

Clone o repositório e restaure os pacotes:

```bash
git clone https://github.com/LeadSoft-Solucoes-Web/leadsoft-adapter-google.git
cd leadsoft-adapter-google
dotnet restore
```

Compile todos os projetos:

```bash
dotnet build --configuration Release
```

Compile apenas um pacote específico:

```bash
dotnet build LeadSoft.Adapter.Google.ReCaptcha/LeadSoft.Adapter.Google.ReCaptcha.csproj --configuration Release
dotnet build LeadSoft.Adapter.Google.Workspace/LeadSoft.Adapter.Google.Workspace.csproj --configuration Release
```

Os pacotes `.nupkg` são gerados automaticamente em `bin/Release/` de cada projeto (`<GeneratePackageOnBuild>true</GeneratePackageOnBuild>`).

---

## Como executar os testes

Os testes estão no projeto `LeadSoft.Google.Tests` (xUnit v3 + Moq). Execute:

```bash
dotnet test LeadSoft.Google.Tests/LeadSoft.Google.Tests.csproj
```

Com cobertura de código (requer `coverlet`):

```bash
dotnet test LeadSoft.Google.Tests/LeadSoft.Google.Tests.csproj \
  --collect:"XPlat Code Coverage" \
  --results-directory ./coverage
```

> Os testes usam `FakeHttpMessageHandler` para simular respostas HTTP sem realizar chamadas reais às APIs do Google. Nenhuma chave ou credencial real é necessária para executar a suite.

---

## Variáveis de ambiente

Cada pacote lê suas configurações exclusivamente de variáveis de ambiente — nunca de arquivos de configuração ou código-fonte.

| Variável | Pacote | Obrigatória | Descrição |
|----------|--------|-------------|-----------|
| `GOOGLE_SSO_CLIENT_ID` | Workspace | Sim | Client ID OAuth2 do Google Cloud Console |
| `GOOGLE_SSO_HOSTED_DOMAIN` | Workspace | Não | Restringe login ao domínio Workspace (ex.: `empresa.com`) |

> O pacote reCAPTCHA não lê variáveis de ambiente — as chaves são passadas diretamente nos DTOs de requisição.

---

## Como contribuir

Contribuições são bem-vindas! Siga os passos abaixo:

1. Faça um **fork** do repositório.
2. Crie uma branch descritiva a partir de `master`:
   ```bash
   git checkout -b feature/minha-funcionalidade
   ```
3. Implemente a mudança seguindo os padrões do projeto:
   - Documentação XML (`<summary>`) em português do Brasil em todos os tipos e membros públicos.
   - Sem comentários explicando *o quê* o código faz — use nomes claros.
   - Testes para toda nova funcionalidade ou correção de bug.
   - Sem warnings de análise estática (`AnalysisLevel: latest-all`).
4. Execute os testes para garantir que nada quebrou:
   ```bash
   dotnet test
   ```
5. Abra um **Pull Request** descrevendo o propósito da mudança.

### Padrões de código

- **Injeção de dependência:** registre sempre via `IServiceCollection` extension em `Injection.cs`.
- **Interfaces:** toda implementação pública deve ter uma interface correspondente (`IGoogleSSO`, `IReCAPTCHA`, etc.).
- **DTOs:** use `record` com `[DataContract]` / `[DataMember]` e construtor posicional.
- **Exceções:** use os tipos do `LeadSoft.Common.Library` (`BadRequestAppException`, `UnauthorizedAppException`, `ForbiddenAppException`) para erros de domínio.
- **Versionamento:** siga semântico — breaking change → major, nova feature → minor, fix → patch.

---

## Pacotes descontinuados

> [!WARNING]
> O pacote [`LeadSoft.Adapter.Google`](https://www.nuget.org/packages/LeadSoft.Adapter.Google/) está **descontinuado** e não receberá mais atualizações.  
> Migre para `LeadSoft.Adapter.Google.ReCaptcha` conforme a documentação do pacote.

---

## Licença

Distribuído sob a licença **MIT**. Consulte o arquivo [LICENSE](LICENSE) para detalhes.

---

## Autoria

Desenvolvido pelo time da **LeadSoft® Soluções Web**.

| Colaborador | Papel |
|-------------|-------|
| [Lucas Resende Tavares](https://www.linkedin.com/in/lucasrtavares/) | Autor principal, arquitetura e manutenção |
| ~~Frederico Ferreira Bitencourt~~ | Contribuidor original (inativo) |
| ~~Pedro Foresti Leão~~ | Contribuidor original (inativo) |

**LeadSoft Soluções Web Ltda** — CNPJ 38.043.762/0001-48

### Como nos encontrar

| Canal | Link |
|-------|------|
| Site | [leadsoft.inf.br](https://www.leadsoft.inf.br) |
| GitHub | [github.com/LeadSoft-Solucoes-Web](https://github.com/LeadSoft-Solucoes-Web) |
| LinkedIn | [linkedin.com/company/leadsoft-solucoes-web](https://www.linkedin.com/company/leadsoft-solucoes-web) |
| E-mail | [developers@leadsoft.inf.br](mailto:developers@leadsoft.inf.br) |
| YouTube | [@LeadsoftSolucoesWeb](https://www.youtube.com/@LeadsoftSolucoesWeb) |
| Instagram | [@leadsoft.inf](https://www.instagram.com/leadsoft.inf/) |
