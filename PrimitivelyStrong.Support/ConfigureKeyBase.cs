using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace PrimitivelyStrong.Support;

/// <summary>
/// Provides a base class for configuring value conversion and property settings for strongly typed key structs in
/// Entity Framework models.
/// </summary>
/// <remarks>This class is intended for use with Entity Framework Core model configuration, enabling consistent
/// conversion and property constraints for custom key types. The configuration enforces NVARCHAR/VARCHAR storage and a
/// maximum length for the key column.</remarks>
/// <typeparam name="TStrong">The struct type representing the strongly typed key to be configured.</typeparam>
/// <param name="convertToProviderExpression">An expression that defines how to convert the strongly typed key to its provider representation as a string.</param>
/// <param name="convertFromProviderExpression">An expression that defines how to convert the provider representation from a string back to the strongly typed key.</param>
/// <param name="maxLength">The maximum allowed length, in characters, for the string representation of the key.</param>
public abstract class ConfigureKeyBase<TStrong>(
    Expression<Func<TStrong, string>> convertToProviderExpression,
    Expression<Func<string, TStrong>> convertFromProviderExpression,
    int maxLength
    ) : ValueConverter<TStrong, string>(
        convertToProviderExpression,
        convertFromProviderExpression
        ), IPropertyCollation
    where TStrong : struct
{
    public virtual bool IsCaseSensitive => false;
    public virtual bool IsUnicode => false;

    public string Collation { get; set; } = string.Empty;

    public PropertiesConfigurationBuilder ConfigureProperties(ModelConfigurationBuilder builder)
        => string.IsNullOrWhiteSpace(Collation)
        ? builder
        .Properties<TStrong>()
        .AreUnicode(IsUnicode)
        .HaveMaxLength(maxLength)
        .HaveConversion(GetType())
        : builder
        .Properties<TStrong>()
        .AreUnicode(IsUnicode)
        .HaveMaxLength(maxLength)
        .HaveConversion(GetType())
        .HaveAnnotation(RelationalAnnotationNames.Collation, Collation);
}
