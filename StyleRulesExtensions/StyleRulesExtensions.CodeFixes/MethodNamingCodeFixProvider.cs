using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;

namespace StyleRulesExtensions
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodNamingCodeFixProvider)), Shared]
    public class MethodNamingCodeFixProvider : BasePascalCaseNamingCodeFixProvider<MethodDeclarationSyntax>
    {
        public MethodNamingCodeFixProvider() : base(CodeFixResources.MethodNamingTitle, nameof(CodeFixResources.MethodNamingTitle))
        {
        }

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(MethodNamingAnalyzer.DiagnosticId); }
        }

        protected sealed override string GetName(MethodDeclarationSyntax declaration)
        {
            return declaration.Identifier.Text;
        }
    }
}
