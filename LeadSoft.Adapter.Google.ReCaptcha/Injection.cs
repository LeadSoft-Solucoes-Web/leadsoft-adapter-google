using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
    public static void AddReCAPTCHA(this IServiceCollection services, bool useSingleton = false)
    {
        if (useSingleton)
            services.AddSingleton<IReCAPTCHA>(sp =>
                new ReCAPTCHA(sp.GetService<ILogger<ReCAPTCHA>>()));
        else
            services.AddScoped<IReCAPTCHA>(sp =>
                new ReCAPTCHA(sp.GetService<ILogger<ReCAPTCHA>>()));
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
    public static void AddReCAPTCHAEnterprise(this IServiceCollection services, string projectId, bool useSingleton = false)
    {
        if (useSingleton)
            services.AddSingleton<IReCAPTCHAEnterprise>(sp =>
                new ReCAPTCHAEnterprise(projectId, sp.GetService<ILogger<ReCAPTCHAEnterprise>>()));
        else
            services.AddScoped<IReCAPTCHAEnterprise>(sp =>
                new ReCAPTCHAEnterprise(projectId, sp.GetService<ILogger<ReCAPTCHAEnterprise>>()));
    }
}
