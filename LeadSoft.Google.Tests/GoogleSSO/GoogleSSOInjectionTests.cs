namespace LeadSoft.Google.Tests.GoogleSSO;

public class GoogleSSOInjectionTests
{
    [Fact]
    public void AddGoogleSSOApi_RegistersIGoogleSSOAsScoped()
    {
        var services = new ServiceCollection();

        services.AddGoogleSSO();

        var descriptor = services.Single(d => d.ServiceType == typeof(IGoogleSSO));
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.Equal(typeof(Adapter.Google.Workspace.GoogleSSO), descriptor.ImplementationType);
    }

    [Fact]
    public void AddGoogleSSOApi_WithSingleton_RegistersAsSingleton()
    {
        var services = new ServiceCollection();

        services.AddGoogleSSO(useSingleton: true);

        var descriptor = services.Single(d => d.ServiceType == typeof(IGoogleSSO));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddGoogleSSOApi_ResolvedInstance_IsTypeGoogleSSO()
    {
        var services = new ServiceCollection();
        services.AddGoogleSSO();
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        using var sso = scope.ServiceProvider.GetRequiredService<IGoogleSSO>();

        Assert.IsType<Adapter.Google.Workspace.GoogleSSO>(sso);
    }

    [Fact]
    public void AddGoogleSSOApi_SingletonResolvedInstance_IsTypeGoogleSSO()
    {
        var services = new ServiceCollection();
        services.AddGoogleSSO(useSingleton: true);
        using var provider = services.BuildServiceProvider();

        using var sso = provider.GetRequiredService<IGoogleSSO>();

        Assert.IsType<Adapter.Google.Workspace.GoogleSSO>(sso);
    }

    [Fact]
    public void AddGoogleSSOApi_CalledTwice_RegistersTwoDescriptors()
    {
        // Comportamento esperado de IServiceCollection: não deduplica por padrão
        var services = new ServiceCollection();

        services.AddGoogleSSO();
        services.AddGoogleSSO();

        Assert.Equal(2, services.Count(d => d.ServiceType == typeof(IGoogleSSO)));
    }

    [Fact]
    public void AddGoogleSSOApi_ScopedInstance_NotSameAcrossScopes()
    {
        var services = new ServiceCollection();
        services.AddGoogleSSO();
        using var provider = services.BuildServiceProvider();

        IGoogleSSO sso1, sso2;
        using (var scope1 = provider.CreateScope())
            sso1 = scope1.ServiceProvider.GetRequiredService<IGoogleSSO>();

        using (var scope2 = provider.CreateScope())
            sso2 = scope2.ServiceProvider.GetRequiredService<IGoogleSSO>();

        Assert.NotSame(sso1, sso2);
    }

    [Fact]
    public void AddGoogleSSOApi_SingletonInstance_SameAcrossResolutions()
    {
        var services = new ServiceCollection();
        services.AddGoogleSSO(useSingleton: true);
        using var provider = services.BuildServiceProvider();

        var sso1 = provider.GetRequiredService<IGoogleSSO>();
        var sso2 = provider.GetRequiredService<IGoogleSSO>();

        Assert.Same(sso1, sso2);
    }
}
