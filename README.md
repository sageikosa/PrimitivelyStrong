Primitively.Strong
------------------
Getting rid of primitive obsession in your C# code.

Once I discovered the concept of "primitive obsession" in code, I realized I had it myself. 
Primitive obsession refers to the overuse of basic data types 
(like strings, integers, and booleans) to represent complex concepts in your code.  The 
primitive types float around in data structures and function signatures, 
complicating validation, behavior, and understanding.  The compiler cannot help check
that the provenance of the value used at a call-site is intended as an identity or key for the domain.

I had worked around this in the _before-times_ with strongly-typed structs and classes, 
using id and key fields carried in the structs to represent the identity 
of the object.  I allowed the records to be mostly empty except for the key and id fields under 
ciscumstances where the object was just a carrier for the identity.

I researched several bits of postings on the interwebs, and worked my way through a few
iterations of my own framework to help me get rid of primitive obsession in my code.

What is presented in this repository is a distillation of that research and experimentation
in sample code form that hopefully will be useful to others.

The solution file has 3 mostly important projects, and one sample project to demonstrate use.

- [PrimitiveyStrong.Generators.Attributes](https://github.com/sageikosa/PrimitivelyStrong/tree/main/PrimitivelyStrong.Generators.Attributes)
- [PrimitiveyStrong.Generators](https://github.com/sageikosa/PrimitivelyStrong/tree/main/PrimitivelyStrong.Generators)
- [PrimitiveyStrong.Support](https://github.com/sageikosa/PrimitivelyStrong/tree/main/PrimitivelyStrong.Support)
- [SampleUse](https://github.com/sageikosa/PrimitivelyStrong/tree/main/SampleUse)

*PrimitiveyStrong.Generators.Attributes* and *PrimitiveyStrong.Generators* need to be built as 
netstandard2.0 projects, as they are source generator or source attribute projects.

*PrimitiveyStrong.Support* is a net10.0 project that contains supporting code referenced in the
generated code, so needs to be referenced in the projects that use the generated code.

*SampleUse* is a net10.0 project that demonstrates how to use the framework.

StrongKeys
----------
When this attribute is placed on a static class, the names of all const int fields in 
that class are used as names for partial struct strong key types using strings as the 
underlying storage.

There are optional parameters for defining the characteristics of the generated code controlling unicode storage
when used with Entity Framework, and case sensitivity comparisons.

Currently, the generated code throw exceptions when the length of the string exceeds the defined 
maximum length when called at runtime in a constructor or init accessor, I've toyed with making
a static analyzer to inspect and throw compile-time errors, but it's just a bit more than
I want to tackle at the moment.  I'm willing to entertain pull requests if anyone is interested,
and gives me some heads-up ahead of time.

Also, there is an implicit conversion to string, but no conversion from string to the strong key type,
this is to avoid accidental conversions from string to the strong key type, which might negate 
some of the compile-time type safety of the strong keys.  You need to work to get strong,
but can slack back into strings easily for logging, console output and display data-binding.

```csharp
[StrongKeys(IsUnicodeStorage = false, IsCaseSensitive = false)]
public static class SampleKeys
{
    public const int SampleKey = 16;
}
```

yields a generated partial struct:
```csharp
    /// <remarks>
    /// <para>MaxLength = 16</para>
    /// <para>Case-insensitive</para>
    /// <para>VARCHAR storage</para>
    /// </remarks>
    public readonly partial struct SampleKey : IEquatable<SampleKey>
    {
        private readonly string _KeyVal;

        public SampleKey(string keyVal)
        {
             _KeyVal = ((keyVal ?? string.Empty).Length <= 16) ? (keyVal ?? string.Empty) : throw new InvalidOperationException($@"length cannot exceed 16");
        }

        /// <summary>Default = string.Empty.  Allow initialization.</summary>
        public string KeyVal
        {
            readonly get => _KeyVal ?? string.Empty;
            init => _KeyVal = ((value ?? string.Empty).Length <= 16) ? (value ?? string.Empty) : throw new InvalidOperationException($@"length cannot exceed 16");
        }

        public bool Equals(SampleKey other)
            => string.Equals(KeyVal, other.KeyVal, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode()
            => StringComparer.OrdinalIgnoreCase.GetHashCode(KeyVal);

        public static implicit operator string(SampleKey strong)
            => strong.KeyVal;

        public override bool Equals(object? obj)
            => obj is SampleKey _val && Equals(_val);

        public static bool operator ==(SampleKey left, SampleKey right)
            => left.Equals(right);

        public static bool operator !=(SampleKey left, SampleKey right)
            => !(left == right);

        public override string ToString()
            => $@"{{ {nameof(KeyVal)} = ""{KeyVal}"" }}";
    }
```

Additionally, a derived *ConfigureKeyBase* class is generated to allow for easy 
Entity Framework configuration of the key type.
```csharp
    public class SampleKeyConfigurator() : ConfigureKeyBase<SampleKey>(v => v.KeyVal, v => new SampleKey(v), 16);
```

Also, extension methods are generated to help with dependency injection setup.
```csharp
    public static class SampleKeysDependencies
    {
        public static IServiceCollection AddSampleKeysConfigurators(this IServiceCollection services)
        {
            if (!services.Any(_sv => _sv.ImplementationType == typeof(SampleKeyConfigurator)))
            {
                foreach (var _conv in GetSampleKeysConfigurators())
                {
                    services.AddSingleton(_conv);
                }
            }
            return services;
        }
        public static IEnumerable<IPropertyConfigurator> GetSampleKeysConfigurators()
        {
            yield return new SampleKeyConfigurator();
            yield break;
        }
    }
```

All these structs and classes are in the same namespace as the class on which the 
StrongKeys attribute is placed.

Putting more than one const int field in the static class will generate multiple
structs and configurators, all of which will be referenced in the generated 
dependency injection helpers.

```csharp
[StrongKeys(IsUnicodeStorage = true)]
public static class CommonUnicodeKeys
{
    // simply renaming or changing these constants
    // does not change the references in referencing code
    // declare the partial structs above greatly improves refactor support
    public const int NameString = 64;
    public const int DescriptionString = 256;
}
```

In all the sample code, there are "stubs" defined above the static classes to 
help with refactoring should you decide to change the names 
of the generated structs in referenced code, as changing the names in the 
static class does not become a rename-refactor function in Visual Studio.

```csharp
// declare partial structs to simplify rename refactoring (if desired)
// you can work without these, but renaming the constants in CommonUnicodeKeys
// ...will require finding and renaming the generated struct references
// ...without refactor support

public readonly partial struct NameString;
public readonly partial struct DescriptionString;
```

StrongIDs
---------
When this attribute is placed on an enum, the names of all enum fields
are used to generate readonly partial record struct types for integral
data fields as the underlying storage.  The integral type used is the base
type of the enum (assumed to be int if not specified).
```csharp
[StrongIDs]
public enum SampleIDs
{
    EntityID,
    ReferenceID
}
```

The generated structs rely on the "record" feature of C# to provide most of the
implementation (unlike StrongKeys which needed custom implementations for equality 
and ensuring Empty.string instead of null in the underyling storage).  As records,
the implementation is much more concise.
```csharp
    /// <remarks> BaseType = int</remarks>
    public readonly partial record struct EntityID(int ID)
    {
        private readonly int _ID = ID;
        public int ID { readonly get => _ID; init => _ID = value; }
        public override string ToString() => @$"{ID}";
    }
```

As with StrongKeys, there are Entity Framework configuration classes generated, and 
dependency injection extension methods generated.
```csharp
    public class EntityIDConfigurator() : ConfigureIDBase<EntityID, int>(v => v.ID, v => new EntityID(v));
    public class ReferenceIDConfigurator() : ConfigureIDBase<ReferenceID, int>(v => v.ID, v => new ReferenceID(v));
    public static class SampleIDsDependencies
    {
        public static IServiceCollection AddSampleIDsConfigurators(this IServiceCollection services)
        {
            if (!services.Any(_sv => _sv.ImplementationType == typeof(EntityIDConfigurator)))
            {
                foreach (var _conv in GetSampleIDsConfigurators())
                {
                    services.AddSingleton(_conv);
                }
            }
            return services;
        }
        public static IEnumerable<IPropertyConfigurator> GetSampleIDsConfigurators()
        {
            yield return new EntityIDConfigurator();
            yield return new ReferenceIDConfigurator();
            yield break;
        }
    }
```

As with StrongKeys, all generated code for StrongIDs is in the same namespace as the 
enum.

Also, as with StrongKeys, there are "stubs" defined above the enum in the SampleIDs file
to help with refactoring should you decide to change the names.

Entity Framework Integration
----------------------------
The samples code has both a DbContext and a DbContextFactory to demonstrate how to
use the generated strong types with Entity Framework Core using the conventions features
and with migrations accessed from external tools like "dotnet ef".

To use at "runtime" make sure the Add...Configurators extension methods have been called
in application setup.  When using with the dotnet ef tool, make sure any DbContextFactory calls the 
...Dependencies.Get...Configurators methods (concatenated, as in the sample code).