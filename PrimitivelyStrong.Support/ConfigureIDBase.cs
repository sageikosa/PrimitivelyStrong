using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace PrimitivelyStrong.Support;

/// <summary>
/// Provides a base class for configuring value conversion between a strongly typed struct and its underlying value type
/// for use with Entity Framework Core property mapping.
/// </summary>
/// <remarks>This class is intended to be used as a base for implementing custom value converters for strongly
/// typed IDs or value objects in Entity Framework Core. It enables type-safe property configuration and conversion
/// logic, facilitating consistent mapping between domain types and their persisted representations.</remarks>
/// <typeparam name="TStrong">The strongly typed struct representing the domain-specific value to be converted.</typeparam>
/// <typeparam name="TValue">The underlying value type used for persistence in the database.</typeparam>
/// <param name="convertToProviderExpression">An expression that defines how to convert the strongly typed struct to its underlying value type for storage in the
/// database. Cannot be null.</param>
/// <param name="convertFromProviderExpression">An expression that defines how to convert the underlying value type from the database back to the strongly typed
/// struct. Cannot be null.</param>
public abstract class ConfigureIDBase<TStrong, TValue>(
    Expression<Func<TStrong, TValue>> convertToProviderExpression,
    Expression<Func<TValue, TStrong>> convertFromProviderExpression
    )
    : ValueConverter<TStrong, TValue>(
        convertToProviderExpression,
        convertFromProviderExpression
        ), IPropertyConfigurator
    where TStrong : struct
    where TValue : struct
{
    public Type ConvertType => typeof(TStrong);
    public Type Converter => GetType();

    public PropertiesConfigurationBuilder ConfigureProperties(ModelConfigurationBuilder builder)
        => builder
        .Properties<TStrong>()
        .HaveConversion(GetType());
}
