using PrimitivelyStrong.Generators.Attributes;

namespace SampleUse;

// declare partial structs to simplify rename refactoring (if desired)
// you can work without these, but renaming the constants in SampleIDs
// ...will require finding and renaming the generated struct references
// ...without refactor support

public readonly partial record struct EntityID;
public readonly partial record struct ReferenceID;

/// <summary>
/// Specifies the set of sample identifier types used to distinguish entities, transactions, and references within the
/// system.
/// </summary>
/// <remarks>This enumeration is typically used in conjunction with the <see cref="StrongIDsAttribute"/> to
/// provide type-safe identifiers for various domain objects. Each value represents a distinct category of identifier,
/// enabling clearer intent and improved compile-time safety when working with strongly-typed IDs.</remarks>
[StrongIDs]
public enum SampleIDs
{
    EntityID,
    ReferenceID
}
