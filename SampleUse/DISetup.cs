using Microsoft.Extensions.DependencyInjection;

namespace SampleUse;

public static class DISetup
{
    /// <summary>
    /// Registers all available configurator services for sample IDs, long IDs, common Unicode keys, and sample keys
    /// into the specified service collection.
    /// </summary>
    /// <remarks>This method is intended to simplify the registration of multiple related configurator
    /// services in a single call. It can be used during application startup to ensure all relevant configurators are
    /// available for dependency injection.</remarks>
    /// <param name="services">The service collection to which the configurator services will be added. Cannot be null.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance with the configurator services registered.</returns>
    public static IServiceCollection AddAllConfigurators(this IServiceCollection services)
    {
        services.AddSampleIDsConfigurators();
        services.AddSampleLongIDsConfigurators();
        services.AddCommonUnicodeKeysConfigurators();
        services.AddSampleKeysConfigurators();
        return services;
    }
}
