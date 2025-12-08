using Microsoft.EntityFrameworkCore;
using PrimitivelyStrong.Support;

namespace SampleUse;

/// <summary>
/// Represents a database context that applies property configuration conventions using the specified configurators.
/// </summary>
/// <remarks>This context applies each configurator to the model configuration builder when conventions are
/// configured. Use this class to centralize and extend property configuration logic for entity models.</remarks>
/// <param name="configurators">A collection of property configurators used to customize model property conventions during context initialization.
/// Cannot be null.</param>
public class SampleDbContext(
    IEnumerable<IPropertyConfigurator> configurators
    ) : DbContext
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        foreach (var _conf in configurators)
        {
            _conf.ConfigureProperties(configurationBuilder);
        }
        base.ConfigureConventions(configurationBuilder);
    }
}
