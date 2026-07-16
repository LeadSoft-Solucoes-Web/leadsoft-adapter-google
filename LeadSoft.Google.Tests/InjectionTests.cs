using System.Linq;

namespace LeadSoft.Google.Tests;

public class InjectionTests
{
    [Fact]
    public void AddReCAPTCHAApi_RegistersIReCAPTCHAAsScoped()
    {
        var services = new ServiceCollection();

        services.AddReCAPTCHAApi();

        var descriptor = services.Single(d => d.ServiceType == typeof(IReCAPTCHA));
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.Equal(typeof(ReCAPTCHA), descriptor.ImplementationType);
    }

    [Fact]
    public void AddReCAPTCHAApi_WithSingleton_RegistersAsSingleton()
    {
        var services = new ServiceCollection();

        services.AddReCAPTCHAApi(useSingleton: true);

        var descriptor = services.Single(d => d.ServiceType == typeof(IReCAPTCHA));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddReCAPTCHAEnterpriseApi_RegistersIReCAPTCHAEnterpriseAsScoped()
    {
        var services = new ServiceCollection();

        services.AddReCAPTCHAEnterpriseApi("my-project");

        var descriptor = services.Single(d => d.ServiceType == typeof(IReCAPTCHAEnterprise));
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddReCAPTCHAEnterpriseApi_WithSingleton_RegistersAsSingleton()
    {
        var services = new ServiceCollection();

        services.AddReCAPTCHAEnterpriseApi("my-project", useSingleton: true);

        var descriptor = services.Single(d => d.ServiceType == typeof(IReCAPTCHAEnterprise));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddReCAPTCHAEnterpriseApi_ResolvedInstance_IsTypeReCAPTCHAEnterprise()
    {
        var services = new ServiceCollection();
        services.AddReCAPTCHAEnterpriseApi("my-project");
        using var provider = services.BuildServiceProvider();

        using var enterprise = provider.GetRequiredService<IReCAPTCHAEnterprise>();

        Assert.IsType<ReCAPTCHAEnterprise>(enterprise);
    }

    [Fact]
    public void AddReCAPTCHAApi_ResolvedInstance_IsTypeReCAPTCHA()
    {
        var services = new ServiceCollection();
        services.AddReCAPTCHAApi();
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        using var recaptcha = scope.ServiceProvider.GetRequiredService<IReCAPTCHA>();

        Assert.IsType<ReCAPTCHA>(recaptcha);
    }
}
