using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;

namespace StyleRulesExtensions
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyNamingCodeFixProvider)), Shared]
    public class PropertyNamingCodeFixProvider : BasePascalCaseNamingCodeFixProvider<PropertyDeclarationSyntax>
    {
        public PropertyNamingCodeFixProvider() : base(CodeFixResources.PropertyNamingTitle, nameof(CodeFixResources.PropertyNamingTitle))
        {
        }

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(PropertyNamingAnalyzer.DiagnosticId); }
        }

        protected sealed override string GetName(PropertyDeclarationSyntax declaration)
        {
            return declaration.Identifier.Text;
        }
    }
}
