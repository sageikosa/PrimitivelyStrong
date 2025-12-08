using PrimitivelyStrong.Generators.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace PrimitivelyStrong.Generators;

[Generator]
public class StrongKeyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var _strongKeysToGenerate = context.SyntaxProvider
           .ForAttributeWithMetadataName(
               @"PrimitivelyStrong.Generators.Attributes.StrongKeysAttribute",
               predicate: static (s, _) => true,
               transform: static (ctx, _) => GetClassSource(ctx.SemanticModel, ctx.TargetNode, ctx.Attributes))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(_strongKeysToGenerate,
            static (spc, strongKeysToGenerate) => DoGenerate(strongKeysToGenerate, spc));
    }

    private static StrongKeysToGenerate? GetClassSource(
        SemanticModel semanticModel,
        SyntaxNode node,
        ImmutableArray<AttributeData> attributes)
    {
        if (semanticModel.GetDeclaredSymbol(node) is not INamedTypeSymbol _classSymbol
            || (_classSymbol.TypeKind != TypeKind.Class))
        {
            return null;
        }

        var _attribute = attributes.FirstOrDefault(_a => _a.AttributeClass!.Name == nameof(StrongKeysAttribute));
        var _unicode = (bool)((_attribute?.NamedArguments
            .FirstOrDefault(_n => _n.Key == nameof(StrongKeysAttribute.IsUnicodeStorage))
            .Value.Value) ?? false);
        var _cased = (bool)((_attribute?.NamedArguments
            .FirstOrDefault(_n => _n.Key == nameof(StrongKeysAttribute.IsCaseSensitive))
            .Value.Value) ?? false);

        var _nameSpace = GetNamespace(node);
        var _className = _classSymbol.Name;
        var _classMembers = _classSymbol.GetMembers();
        var _keys = new List<(string, int)>(_classMembers.Length);

        foreach (var _member in _classMembers)
        {
            if (_member is IFieldSymbol _field
                && _field.ConstantValue is not null)
            {
                _keys.Add((_member.Name, (int)_field.ConstantValue));
            }
        }

        return new StrongKeysToGenerate(_nameSpace, _className, _unicode, _cased, _keys);
    }

    private static void DoGenerate(StrongKeysToGenerate? strongKeysToGenerate, SourceProductionContext context)
    {
        if (strongKeysToGenerate is { } _sktg)
        {
            // generate the source code and add it to the output
            var _code = GenerateCode(_sktg);
            context.AddSource($@"StrongKeys.{_sktg.Name}.g.cs", SourceText.From(_code, Encoding.UTF8));
        }
    }

    private static void AddStrongString(string name, int length, bool isUnicode, bool isCaseSensitive, StringBuilder builder)
    {
        builder.AppendLine(@"    /// <remarks>");
        builder.Append(@"    /// <para>MaxLength = ");
        builder.Append(length);
        builder.AppendLine(@"</para>");
        if (isCaseSensitive)
        {
            builder.AppendLine(@"    /// <para>Case-sensitive</para>");
        }
        else
        {
            builder.AppendLine(@"    /// <para>Case-insensitive</para>");
        }
        if (isUnicode)
        {
            builder.AppendLine(@"    /// <para>NVARCHAR storage</para>");
        }
        else
        {
            builder.AppendLine(@"    /// <para>VARCHAR storage</para>");
        }
        builder.AppendLine(@"    /// </remarks>");
        builder.Append(@"    public readonly partial struct ");
        builder.Append(name);
        //builder.Append(@"(string keyVal) : IEquatable<");
        builder.Append(@" : IEquatable<");
        builder.Append(name);
        builder.AppendLine(@">");
        builder.AppendLine(@"    {");
        //builder.AppendLine(@"        private readonly string _KeyVal = keyVal;");
        builder.AppendLine(@"        private readonly string _KeyVal;");
        builder.AppendLine();
        builder.Append(@"        public ");
        builder.Append(name);
        builder.AppendLine(@"(string keyVal)");
        builder.AppendLine(@"        {");
        builder.Append(@"             _KeyVal = ((keyVal ?? string.Empty).Length <= ");
        builder.Append(length);
        builder.Append(@") ? (keyVal ?? string.Empty) : throw new InvalidOperationException($@""length cannot exceed ");
        builder.Append(length);
        builder.AppendLine(@""");");
        builder.AppendLine(@"        }");
        builder.AppendLine();
        builder.AppendLine(@"        /// <summary>Default = string.Empty.  Allow initialization.</summary>");
        builder.AppendLine(@"        public string KeyVal");
        builder.AppendLine(@"        {");
        builder.AppendLine(@"            readonly get => _KeyVal ?? string.Empty;");
        builder.Append(@"            init => _KeyVal = ((value ?? string.Empty).Length <= ");
        builder.Append(length);
        builder.Append(@") ? (value ?? string.Empty) : throw new InvalidOperationException($@""length cannot exceed ");
        builder.Append(length);
        builder.AppendLine(@""");");
        builder.AppendLine(@"        }");
        builder.AppendLine();
        builder.Append(@"        public bool Equals(");
        builder.Append(name);
        builder.AppendLine(@" other)");
        if (isCaseSensitive)
        {
            builder.AppendLine(@"            => string.Equals(KeyVal, other.KeyVal, StringComparison.Ordinal);");
        }
        else
        {
            builder.AppendLine(@"            => string.Equals(KeyVal, other.KeyVal, StringComparison.OrdinalIgnoreCase);");
        }
        builder.AppendLine();
        builder.AppendLine(@"        public override int GetHashCode()");
        if (isCaseSensitive)
        {
            builder.AppendLine(@"            => StringComparer.Ordinal.GetHashCode(KeyVal);");
        }
        else
        {
            builder.AppendLine(@"            => StringComparer.OrdinalIgnoreCase.GetHashCode(KeyVal);");
        }
        builder.AppendLine();
        builder.Append(@"        public static implicit operator string(");
        builder.Append(name);
        builder.AppendLine(@" strong)");
        builder.AppendLine(@"            => strong.KeyVal;");
        builder.AppendLine();
        builder.AppendLine(@"        public override bool Equals(object? obj)");
        builder.Append(@"            => obj is ");
        builder.Append(name);
        builder.AppendLine(@" _val && Equals(_val);");
        builder.AppendLine();
        builder.Append(@"        public static bool operator ==(");
        builder.Append(name);
        builder.Append(@" left, ");
        builder.Append(name);
        builder.AppendLine(@" right)");
        builder.AppendLine(@"            => left.Equals(right);");
        builder.AppendLine();
        builder.Append(@"        public static bool operator !=(");
        builder.Append(name);
        builder.Append(@" left, ");
        builder.Append(name);
        builder.AppendLine(@" right)");
        builder.AppendLine(@"            => !(left == right);");
        builder.AppendLine();
        builder.AppendLine(@"        public override string ToString()");
        builder.AppendLine(@"            => $@""{{ {nameof(KeyVal)} = """"{KeyVal}"""" }}"";");
        builder.AppendLine(@"    }");
    }

    private static string GenerateCode(StrongKeysToGenerate strongKeys)
    {
        var sb = new StringBuilder();
        sb.AppendLine(@"using PrimitivelyStrong.Support;");
        sb.AppendLine(@"using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine(@"#nullable enable");
        sb.Append(@"namespace ");
        sb.AppendLine(strongKeys.NameSpace);
        sb.AppendLine(@"{");

        foreach (var _key in strongKeys.Keys)
        {
            AddStrongString(_key.Item1, _key.Item2, strongKeys.IsUnicodeStorage, strongKeys.IsCaseSensitive, sb);
        }

        foreach (var _key in strongKeys.Keys)
        {
            sb.Append(@"    public class ");
            sb.Append(_key.Item1);
            sb.Append(@"Configurator() : ConfigureKeyBase<");
            sb.Append(_key.Item1);
            sb.Append(@">(v => v.KeyVal, v => new ");
            sb.Append(_key.Item1);
            sb.Append(@"(v), ");
            sb.Append(_key.Item2);
            if (strongKeys.IsUnicodeStorage)
            {
                sb.AppendLine(@"){ public override bool IsUnicode => true; }");
            }
            else
            {
                sb.AppendLine(@");");
            }
        }

        sb.Append(@"    public static class ");
        sb.Append(strongKeys.Name);
        sb.AppendLine(@"Dependencies");
        sb.AppendLine(@"    {");
        sb.Append(@"        public static IServiceCollection Add");
        sb.Append(strongKeys.Name);
        sb.AppendLine(@"Configurators(this IServiceCollection services)");
        sb.AppendLine(@"        {");
        sb.Append(@"            if (!services.Any(_sv => _sv.ImplementationType == typeof(");
        sb.Append(strongKeys.Keys.FirstOrDefault().Item1);
        sb.AppendLine(@"Configurator)))");
        sb.AppendLine(@"            {");
        sb.Append(@"                foreach (var _conv in Get");
        sb.Append(strongKeys.Name);
        sb.AppendLine(@"Configurators())");
        sb.AppendLine(@"                {");
        sb.AppendLine(@"                    services.AddSingleton(_conv);");
        sb.AppendLine(@"                }");
        sb.AppendLine(@"            }");
        sb.AppendLine(@"            return services;");
        sb.AppendLine(@"        }");

        sb.Append(@"        public static IEnumerable<IPropertyConfigurator> Get");
        sb.Append(strongKeys.Name);
        sb.AppendLine(@"Configurators()");
        sb.AppendLine(@"        {");
        foreach (var _key in strongKeys.Keys)
        {
            sb.Append(@"            yield return new ");
            sb.Append(_key.Item1);
            sb.AppendLine(@"Configurator();");
        }
        sb.AppendLine(@"            yield break;");
        sb.AppendLine(@"        }");

        // class
        sb.AppendLine(@"    }");

        // namespace
        sb.AppendLine(@"}");
        return sb.ToString();
    }

    internal static string GetNamespace(SyntaxNode syntax)
    {
        var _nameSpace = string.Empty;
        var _potentialParent = syntax.Parent;

        while ((_potentialParent != null)
            && (_potentialParent is not NamespaceDeclarationSyntax)
            && (_potentialParent is not FileScopedNamespaceDeclarationSyntax))
        {
            _potentialParent = _potentialParent.Parent;
        }

        if (_potentialParent is BaseNamespaceDeclarationSyntax _parent)
        {
            _nameSpace = _parent.Name.ToString();

            while (true)
            {
                if (_parent.Parent is not NamespaceDeclarationSyntax _nsParent)
                {
                    break;
                }

                // build it
                _nameSpace = $@"{_parent.Name}.{_nameSpace}";
                _parent = _nsParent;
            }
        }
        return _nameSpace;
    }
}
