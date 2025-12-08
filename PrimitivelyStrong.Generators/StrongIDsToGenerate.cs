namespace PrimitivelyStrong.Generators;
public readonly record struct StrongIDsToGenerate
{
    public readonly string NameSpace;
    public readonly string Name;
    public readonly string IntegralTypeName;
    public readonly EquatableArray<string> IDs;

    public StrongIDsToGenerate(
        string nameSpace,
        string name,
        string integralTypeName,
        List<string> ids)
    {
        NameSpace = nameSpace;
        Name = name;
        IntegralTypeName = integralTypeName;
        IDs = new([.. ids]);
    }
}
