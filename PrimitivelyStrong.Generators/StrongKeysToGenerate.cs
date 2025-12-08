namespace PrimitivelyStrong.Generators;
public readonly record struct StrongKeysToGenerate
{
    public readonly string NameSpace;
    public readonly string Name;
    public readonly bool IsUnicodeStorage;
    public readonly bool IsCaseSensitive;
    public readonly EquatableArray<(string, int)> Keys;

    public StrongKeysToGenerate(
        string nameSpace,
        string name,
        bool isUnicodeStorage,
        bool isCaseSensitive,
        List<(string, int)> keys)
    {
        NameSpace = nameSpace;
        Name = name;
        IsUnicodeStorage = isUnicodeStorage;
        IsCaseSensitive = isCaseSensitive;
        Keys = new([.. keys]);
    }
}
