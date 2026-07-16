using LeadSoft.Adapter.Google.ReCaptchaService;
using Microsoft.Extensions.DependencyInjection;

namespace LeadSoft.Adapter.Google.ReCaptcha;

/// <summary>
/// Extensões de injeção de dependência para os serviços do Google reCAPTCHA.
/// </summary>
public static class Injection
{
    /// <summary>
    /// Registra o serviço do Google reCAPTCHA v3 no <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">A coleção de serviços onde o adapter será registrado.</param>
    /// <param name="useSingleton">
    /// <see langword="true"/> para registrar como singleton (compartilhado em toda a aplicação);
    /// <see langword="false"/> para registrar como scoped (por requisição). Padrão: <see langword="false"/>.
    /// </param>
    public static void AddReCAPTCHAApi(this IServiceCollection services, bool useSingleton = false)
    {
        if (useSingleton)
            services.AddSingleton<IReCAPTCHA, ReCAPTCHA>();
        else
            services.AddScoped<IReCAPTCHA, ReCAPTCHA>();
    }

    /// <summary>
    /// Registra o serviço do Google reCAPTCHA Enterprise no <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">A coleção de serviços onde o adapter será registrado.</param>
    /// <param name="projectId">O ID do projeto no Google Cloud associado ao reCAPTCHA Enterprise.</param>
    /// <param name="useSingleton">
    /// <see langword="true"/> para registrar como singleton (compartilhado em toda a aplicação);
    /// <see langword="false"/> para registrar como scoped (por requisição). Padrão: <see langword="false"/>.
    /// </param>
    public static void AddReCAPTCHAEnterpriseApi(this IServiceCollection services, string projectId, bool useSingleton = false)
    {
        if (useSingleton)
            services.AddSingleton<IReCAPTCHAEnterprise>(_ => new ReCAPTCHAEnterprise(projectId));
        else
            services.AddScoped<IReCAPTCHAEnterprise>(_ => new ReCAPTCHAEnterprise(projectId));
    }
}
