namespace PrimitivelyStrong.Generators.Attributes;

/// <summary>
/// Will read class values and generate strongly-typed keys with max length based on the values of the fields.
/// </summary>
/// <remarks>
/// Will also use the name of the class to name the generated dependency injection classes used for EF Core conversions.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class StrongKeysAttribute : Attribute
{
    public bool IsUnicodeStorage { get; set; } = false;
    public bool IsCaseSensitive { get; set; } = false;
}
