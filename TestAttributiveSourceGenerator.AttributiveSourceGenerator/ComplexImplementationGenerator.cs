using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Text;
using System.Threading;

namespace TestAttributiveSourceGenerator.AttributiveSourceGenerator;

[Generator]
public class ComplexImplementationGenerator : IIncrementalGenerator
{
    const string ExpectedToAffectOn = "TestAttributiveSourceGenerator.ComplexImplementationAttribute";

    struct ComplexImplementationContext
    {
        public string Namespace;
        public string Class;
        public string[] Usings { get; set; }
        public string Pattern { get; set; }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //Debugger.Launch();

        var classDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
            FetchClassDeclarations,
            FilterClassDeclarations
        ).Where(x => x is not null).Select((x, _) => (ComplexImplementationContext)x);

        context.RegisterSourceOutput(classDeclarations, Generate);
    }

    static bool FetchClassDeclarations(SyntaxNode node, CancellationToken _)
        => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };

    static ComplexImplementationContext? FilterClassDeclarations(GeneratorSyntaxContext context, CancellationToken _)
    {
        //Debugger.Break();
        var declarationSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node) as ITypeSymbol;
        foreach (var attributeData in declarationSymbol.GetAttributes())
        {
            if (MatchesAttribute(attributeData))
            {
                var implementationContext = new ComplexImplementationContext()
                {
                    Namespace = declarationSymbol.ContainingNamespace.ToDisplayString(),
                    Class = declarationSymbol.Name,
                };
                foreach (var namedArgument in attributeData.NamedArguments)
                {
                    switch (namedArgument.Key)
                    {
                        case nameof(ComplexImplementationContext.Usings):
                            var immutableArray = namedArgument.Value.Values;
                            implementationContext.Usings = new string[immutableArray.Length];
                            for (var i = 0; i < immutableArray.Length; i++)
                            {
                                implementationContext.Usings[i] = immutableArray[i].Value as string;
                            }
                            break;
                        case nameof(ComplexImplementationContext.Pattern):
                            implementationContext.Pattern = namedArgument.Value.Value as string;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                return implementationContext;
            }
        }
        return null;
    }

    static void Generate(SourceProductionContext sourceProductionContext, ComplexImplementationContext implementationContext)
    {
        var builder = new StringBuilder();
        foreach (var usingItem in implementationContext.Usings)
        {
            builder.AppendLine($"using {usingItem};");
        }
        builder.AppendLine()
            .AppendLine($"namespace {implementationContext.Namespace};")
            .AppendLine();
        builder.Append($@"
public partial class {implementationContext.Class}
{{
    {implementationContext.Pattern}
}}
        ");
        sourceProductionContext.AddSource(
            $"{implementationContext.Namespace}.{implementationContext.Class}.g.cs", builder.ToString()
        );
    }

    static bool MatchesAttribute(AttributeData attributeData)
    {
        var attributeClass = attributeData.AttributeClass;
        return $"{attributeClass.ContainingNamespace.ToDisplayString()}.{attributeClass.Name}" == ExpectedToAffectOn;
    }
}
