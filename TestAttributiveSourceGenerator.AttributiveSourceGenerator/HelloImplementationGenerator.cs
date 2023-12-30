using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading;

namespace TestAttributiveSourceGenerator.AttributiveSourceGenerator;

[Generator(LanguageNames.CSharp)]
public class HelloImplementationGenerator : IIncrementalGenerator
{
    const string ExpectedToAffectOn = "TestAttributiveSourceGenerator.HelloImplementationAttribute";

    struct ImplementationContext
    {
        public string Namespace { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string HelloContent { get; set; }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var implementationContexts = context.SyntaxProvider.CreateSyntaxProvider(
            FetchMethods,
            FilterMethods
        ).Where(x => x is not null).Select((x, _) => (ImplementationContext)x);

        context.RegisterSourceOutput(implementationContexts, Generate);
    }

    static bool FetchMethods(SyntaxNode node, CancellationToken _) => node is MethodDeclarationSyntax;

    static ImplementationContext? FilterMethods(GeneratorSyntaxContext context, CancellationToken _)
    {
        var methodDeclaration = context.SemanticModel.GetDeclaredSymbol(context.Node) as IMethodSymbol;
        foreach (var attributeData in methodDeclaration.GetAttributes())
        {
            if (MatchesAttribute(attributeData))
            {
                var helloContent = attributeData.ConstructorArguments[0].Value as string;
                return new()
                {
                    Namespace = methodDeclaration.ContainingNamespace.Name,
                    Class = methodDeclaration.ContainingType.Name,
                    Method = methodDeclaration.Name,
                    HelloContent = helloContent
                };
            }
        }
        return null;
    }

    static void Generate(SourceProductionContext context, ImplementationContext implementationContext)
    {
        context.AddSource($"{implementationContext.Namespace}.{implementationContext.Class}.g.cs", $@"
            namespace {implementationContext.Namespace};
            
            public partial class {implementationContext.Class}
            {{
                public partial void {implementationContext.Method}()
                {{
                    Console.WriteLine(""{implementationContext.HelloContent}"");
                }}
            }}
        ");
    }

    static bool MatchesAttribute(AttributeData attributeData)
    {
        var attributeTypeSymbol = attributeData.AttributeClass;
        var namespaces = attributeTypeSymbol.ContainingNamespace.ToDisplayString();

        return $"{namespaces}.{attributeTypeSymbol.Name}" == ExpectedToAffectOn;
    }
}
