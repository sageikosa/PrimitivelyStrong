using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PrimitivelyStrong.Support;

public interface IPropertyConfigurator
{
    PropertiesConfigurationBuilder ConfigureProperties(ModelConfigurationBuilder builder);
}
