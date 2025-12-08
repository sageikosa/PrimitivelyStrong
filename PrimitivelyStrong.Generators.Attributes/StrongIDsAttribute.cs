namespace PrimitivelyStrong.Generators.Attributes;

/// <summary>
/// When attached to an enum, will use the base numeric type of the enum to generate strongly typed IDs.
/// </summary>
/// <remarks>
/// Will also use the name of the class to name the generated dependency injection classes used for EF Core conversions.
/// </remarks>
[AttributeUsage(AttributeTargets.Enum)]
public class StrongIDsAttribute : Attribute
{
}
