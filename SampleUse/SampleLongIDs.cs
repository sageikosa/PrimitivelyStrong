using PrimitivelyStrong.Generators.Attributes;

namespace SampleUse;

// declare partial structs to simplify rename refactoring (if desired)
// you can work without these, but renaming the constants in SampleLongIDs
// ...will require finding and renaming the generated struct references
// ...without refactor support

public readonly partial record struct LoggingID;
public readonly partial record struct TransactionID;

/// <summary>
/// Defines strongly-typed identifiers for logging and transaction operations, represented as 64-bit integer values.
/// </summary>
/// <remarks>This enumeration is intended for use with APIs or systems that require distinct long-based
/// identifiers for logging and transaction contexts. The <see cref="StrongIDsAttribute"/> indicates that these values
/// are used as strong IDs, which can help prevent accidental misuse or mixing of unrelated identifiers.</remarks>
[StrongIDs]
public enum SampleLongIDs : long
{
    LoggingID,
    TransactionID
}
