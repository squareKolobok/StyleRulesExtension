using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StyleRulesExtensions
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PrivateFieldNamingCodeFixProvider)), Shared]
    public class PrivateFieldNamingCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(PrivateFieldNamingAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.PrivateFieldNamingTitle,
                    createChangedSolution: cancellationToken => RenameFieldAsync(context.Document, declaration, cancellationToken),
                    equivalenceKey: nameof(CodeFixResources.PrivateFieldNamingTitle)),
                diagnostic);
        }

        private async Task<Solution> RenameFieldAsync(
            Document document,
            FieldDeclarationSyntax fieldDeclaration,
            CancellationToken cancellationToken)
        {
            var solution = document.Project.Solution;
            var optionSet = solution.Workspace.Options;

            var variable = fieldDeclaration.Declaration.Variables.First();
            var name = variable.Identifier.Text;
            var isFirsCharCorrect = name[0] == '_';
            var firstCharPosition = isFirsCharCorrect ? 1 : 0;
            var otherNamePath = name.Substring(isFirsCharCorrect ? 2 : 1);
            var newName = $"_{char.ToLower(name[firstCharPosition])}{otherNamePath}";

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var symbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);

            var newSolution = await Renamer.RenameSymbolAsync(solution, symbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            return newSolution;
        }
    }
}
