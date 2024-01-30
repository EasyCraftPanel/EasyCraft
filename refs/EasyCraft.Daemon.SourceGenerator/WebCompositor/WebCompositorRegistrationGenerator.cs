using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace EasyCraft.Daemon.SourceGenerator.WebCompositor
{
    [Generator(LanguageNames.CSharp)]
    public sealed class WebCompositorRegistrationGenerator : IIncrementalGenerator
    {
        private const string FileName = "WebCompositorRegister.g.cs";

        private const string FileContent =
            """
            using Microsoft.AspNetCore.Builder;
            namespace EasyCraft.Daemon.Extensions {
                public static class WebCompositorRegisterExtensions
                {
                    public static void AddControllerServices(this WebApplicationBuilder builder)
                    {
                        #TBDS#
                    }
                    
                    public static void UseControllerApp(this WebApplication app)
                    {
                        #TBDA#
                    }
                }
            }
            """;

        private const string AttributeFileContent = 
            """
            using System;
            
            namespace EasyCraft.Daemon.SourceGenerator.WebCompositor;
            
            [AttributeUsage(AttributeTargets.Class)]
            public class WebCompositorAttribute : Attribute
            {
                public WebCompositorAttribute(int order)
                {
                    
                }
                public WebCompositorAttribute()
                {
                    
                }                                   
            }
            """;

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // get all classes implements IController
            var provider = context.SyntaxProvider
                .CreateSyntaxProvider((node, _) => node is ClassDeclarationSyntax, CheckImplements)
                .Where(t => t.order != -1);
            context.RegisterPostInitializationOutput(t=>t.AddSource("WebCompositorAttribute.g.cs", SourceText.From(AttributeFileContent, Encoding.UTF8)));
            context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()), GenerateCode);
        }

        private void GenerateCode(SourceProductionContext context,
            (Compilation Left, ImmutableArray<(ClassDeclarationSyntax syntax, int order)> Right) valueTuple)
        {
            var builder = new StringBuilder();
            var builder2 = new StringBuilder();
            var syntaxes = valueTuple.Right.ToList().GroupBy(t => t.order).OrderBy(t=>t.Key).ToList();
            foreach (var syntaxGroup in syntaxes)
            {
                builder.AppendLine($"// Layer {syntaxGroup.Key}");
                builder2.AppendLine($"// Layer {syntaxGroup.Key}");
                foreach (var (syntax, order) in syntaxGroup)
                {
                    if (syntax.Parent is BaseNamespaceDeclarationSyntax bnds)
                    {
                        var parentName = bnds.Name.ToString();
                        builder.AppendLine(
                            $"{parentName}.{syntax.Identifier.ToString()}.ConfigureBuilder(builder);");
                        builder2.AppendLine($"{parentName}.{syntax.Identifier.ToString()}.ConfigureApp(app);");
                    }
                }
            }

            var content = FileContent.Replace("#TBDS#", builder.ToString())
                .Replace("#TBDA#", builder2.ToString());

            context.AddSource(FileName, SourceText.From(content, System.Text.Encoding.UTF8));
        }


        private (ClassDeclarationSyntax syntax, int order) CheckImplements(GeneratorSyntaxContext context,
            CancellationToken cancellationToken)
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
            if (classDeclarationSyntax.BaseList is null) return (classDeclarationSyntax, -1);
            foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
            {
                foreach (var attributeSyntax in attributeListSyntax.Attributes)
                {
                    var info = context.SemanticModel.GetSymbolInfo(attributeSyntax);
                    if (info.Symbol is not IMethodSymbol methodSymbol) continue;
                    string attributeName = methodSymbol.ContainingType.ToDisplayString();
                    if (attributeName == "EasyCraft.Daemon.SourceGenerator.WebCompositor.WebCompositorAttribute")
                    {
                        if (methodSymbol.Parameters.Length == 0) return (classDeclarationSyntax, 5);

                        if (int.TryParse(attributeSyntax.ArgumentList!.Arguments[0].Expression.ToString(), out int order))
                        {
                            return (classDeclarationSyntax, order);
                        }
                    }
                }
            }


            return (classDeclarationSyntax, -1);
        }
    }
}