using Microsoft.EntityFrameworkCore.Design;

namespace SampleUse;

/// <summary>
/// Provides a design-time factory for creating instances of <see cref="SampleDbContext"/> for use with Entity Framework
/// Core tooling.
/// </summary>
/// <remarks>This class is typically used by Entity Framework Core tools such as migrations and scaffolding to
/// instantiate <see cref="SampleDbContext"/> when the application's runtime configuration is not available. It
/// implements <see cref="IDesignTimeDbContextFactory{TContext}"/> to supply a context instance during design-time
/// operations. For most applications, you do not need to use this class directly.</remarks>
public class SampleDbContextFactory 
    : IDesignTimeDbContextFactory<SampleDbContext>
{
    public SampleDbContext CreateDbContext(string[] args) 
        => new(
            CommonUnicodeKeysDependencies.GetCommonUnicodeKeysConfigurators()
            .Concat(SampleKeysDependencies.GetSampleKeysConfigurators())
            .Concat(SampleIDsDependencies.GetSampleIDsConfigurators())
            .Concat(SampleLongIDsDependencies.GetSampleLongIDsConfigurators()),
            @"SQL_Latin1_General_CP1_CS_AS",
            @"SQL_Latin1_General_CP1_CI_AS"
            );
}
