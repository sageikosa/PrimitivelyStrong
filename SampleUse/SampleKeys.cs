using PrimitivelyStrong.Generators.Attributes;

namespace SampleUse;

// declare partial structs to simplify rename refactoring (if desired)
// you can work without these, but renaming the constants in SampleKeys
// ...will require finding and renaming the generated struct references
// ...without refactor support

public readonly partial struct SampleKey;

/// <summary>
/// Provides sample key constants used for string length definitions in VARCHAR storage scenarios.
/// </summary>
/// <remarks>
/// <para>
/// These constants are intended for use in scenarios where standardized string length limits are
/// required, such as database field definitions or validation logic.
/// </para>
/// <para>The keys are not case-sensitive and do not use Unicode storage, as indicated by the associated attributes.</para>
/// </remarks>
[StrongKeys(IsUnicodeStorage = false, IsCaseSensitive = false)]
public static class SampleKeys
{
    public const int SampleKey = 16;
}
