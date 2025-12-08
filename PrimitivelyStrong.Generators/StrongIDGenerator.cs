using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace PrimitivelyStrong.Generators;

[Generator]
public class StrongIDGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var _strongIDsToGenerate = context.SyntaxProvider
           .ForAttributeWithMetadataName(
               @"PrimitivelyStrong.Generators.Attributes.StrongIDsAttribute",
               predicate: static (s, _) => true,
               transform: static (ctx, _) => GetEnumSource(ctx.SemanticModel, ctx.TargetNode))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(_strongIDsToGenerate,
            static (spc, strongKeysToGenerate) => DoGenerate(strongKeysToGenerate, spc));
    }

    private static StrongIDsToGenerate? GetEnumSource(SemanticModel semanticModel, SyntaxNode node)
    {
        if (semanticModel.GetDeclaredSymbol(node) is not INamedTypeSymbol _enumSymbol
            || (_enumSymbol.TypeKind != TypeKind.Enum)
            || (node is not EnumDeclarationSyntax _enumDeclaration))
        {
            return null;
        }

        var _nameSpace = StrongKeyGenerator.GetNamespace(node);
        var _enumName = _enumSymbol.Name;

        string _semType =
            (_enumDeclaration.BaseList?.Types.FirstOrDefault()?.Type is TypeSyntax _enumBase
            ? semanticModel.GetTypeInfo(_enumBase).Type?.Name
            : null)
            ?? @"int";

        var _enumMembers = _enumSymbol.GetMembers();
        var _keys = new List<string>(_enumMembers.Length);

        foreach (var _member in _enumMembers)
        {
            if (_member is IFieldSymbol _field
                && _field.ConstantValue is not null)
            {
                _keys.Add(_member.Name);
            }
        }

        return new StrongIDsToGenerate(_nameSpace, _enumName, _semType, _keys);
    }

    private static void DoGenerate(StrongIDsToGenerate? strongIDsToGenerate, SourceProductionContext context)
    {
        if (strongIDsToGenerate is { } _sitg)
        {
            // generate the source code and add it to the output
            var _code = GenerateCode(_sitg);
            context.AddSource($@"StrongIDs.{_sitg.Name}.g.cs", SourceText.From(_code, Encoding.UTF8));
        }
    }

    private static string GenerateCode(StrongIDsToGenerate strongIDs)
    {
        var sb = new StringBuilder();
        sb.AppendLine(@"using PrimitivelyStrong.Support;");
        sb.AppendLine(@"using Microsoft.Extensions.DependencyInjection;");
        sb.Append(@"namespace ");
        sb.AppendLine(strongIDs.NameSpace);
        sb.AppendLine(@"{");

        foreach (var _id in strongIDs.IDs)
        {
            sb.Append(@"    /// <remarks> BaseType = ");
            sb.Append(strongIDs.IntegralTypeName);
            sb.AppendLine(@"</remarks>");
            sb.Append(@"    public readonly partial record struct ");
            sb.Append(_id);
            sb.Append(@"(");
            sb.Append(strongIDs.IntegralTypeName);
            sb.AppendLine(@" ID)");
            sb.AppendLine(@"    {");
            sb.Append(@"        private readonly ");
            sb.Append(strongIDs.IntegralTypeName);
            sb.AppendLine(@" _ID = ID;");
            sb.Append(@"        public ");
            sb.Append(strongIDs.IntegralTypeName);
            sb.AppendLine(@" ID { readonly get => _ID; init => _ID = value; }");
            sb.AppendLine(@"        public override string ToString() => @$""{ID}"";");
            sb.AppendLine(@"    }");
        }

        foreach (var _id in strongIDs.IDs)
        {
            sb.Append(@"    public class ");
            sb.Append(_id);
            sb.Append(@"Configurator() : ConfigureIDBase<");
            sb.Append(_id);
            sb.Append(@", ");
            sb.Append(strongIDs.IntegralTypeName);
            sb.Append(@">(v => v.ID, v => new ");
            sb.Append(_id);
            sb.AppendLine(@"(v));");
        }

        sb.Append(@"    public static class ");
        sb.Append(strongIDs.Name);
        sb.AppendLine(@"Dependencies");
        sb.AppendLine(@"    {");
        sb.Append(@"        public static IServiceCollection Add");
        sb.Append(strongIDs.Name);
        sb.AppendLine(@"Configurators(this IServiceCollection services)");
        sb.AppendLine(@"        {");
        sb.Append(@"            if (!services.Any(_sv => _sv.ImplementationType == typeof(");
        sb.Append(strongIDs.IDs.FirstOrDefault());
        sb.AppendLine(@"Configurator)))");
        sb.AppendLine(@"            {");
        sb.Append(@"                foreach (var _conv in Get");
        sb.Append(strongIDs.Name);
        sb.AppendLine(@"Configurators())");
        sb.AppendLine(@"                {");
        sb.AppendLine(@"                    services.AddSingleton(_conv);");
        sb.AppendLine(@"                }");
        sb.AppendLine(@"            }");
        sb.AppendLine(@"            return services;");
        sb.AppendLine(@"        }");

        sb.Append(@"        public static IEnumerable<IPropertyConfigurator> Get");
        sb.Append(strongIDs.Name);
        sb.AppendLine(@"Configurators()");
        sb.AppendLine(@"        {");
        foreach (var _id in strongIDs.IDs)
        {
            sb.Append(@"            yield return new ");
            sb.Append(_id);
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
}
