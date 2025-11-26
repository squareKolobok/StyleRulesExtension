using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;

namespace StyleRulesExtensions
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClassNamingCodeFixProvider)), Shared]
    public class ClassNamingCodeFixProvider : BasePascalCaseNamingCodeFixProvider<TypeDeclarationSyntax>
    {
        public ClassNamingCodeFixProvider() : base(CodeFixResources.ClassNamingTitle, nameof(CodeFixResources.ClassNamingTitle))
        {
        }

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ClassNamingAnalyzer.DiagnosticId); }
        }

        protected sealed override string GetName(TypeDeclarationSyntax declaration)
        {
            return declaration.Identifier.Text;
        }
    }
}
