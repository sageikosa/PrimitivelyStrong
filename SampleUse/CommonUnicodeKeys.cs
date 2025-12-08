using PrimitivelyStrong.Generators.Attributes;

namespace SampleUse;

// declare partial structs to simplify rename refactoring (if desired)
// you can work without these, but renaming the constants in CommonUnicodeKeys
// ...will require finding and renaming the generated struct references
// ...without refactor support

public readonly partial struct NameString;
public readonly partial struct DescriptionString;

/// <summary>
/// Provides common key constants used for string length definitions in NVARCHAR storage scenarios.
/// </summary>
/// <remarks>
/// <para>
/// These constants are intended for use in scenarios where standardized string length limits are
/// required, such as database field definitions or validation logic.</para>
/// <para>
/// Changing the values of these constants does not automatically update references in dependent code; 
/// refactoring support is improved by declaring related partial
/// structs.
/// </para>
/// </remarks>
[StrongKeys(IsUnicodeStorage = true)]
public static class CommonUnicodeKeys
{
    // simply renaming or changing these constants
    // does not change the references in referencing code
    // declare the partial structs above greatly improves refactor support
    public const int NameString = 64;
    public const int DescriptionString = 256;
}