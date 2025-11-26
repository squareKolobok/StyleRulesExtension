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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodEndsAsyncCodeFixProvider)), Shared]
    public class MethodEndsAsyncCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(MethodEndsAsyncAnalyzer.DiagnosticId); }
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

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.MethodEndsAsyncTitle,
                    createChangedSolution: cancellationToken => RenameMethodAsync(context.Document, declaration, cancellationToken),
                    equivalenceKey: nameof(CodeFixResources.MethodEndsAsyncTitle)),
                diagnostic);
        }

        private async Task<Solution> RenameMethodAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken)
        {
            var solution = document.Project.Solution;
            var optionSet = solution.Workspace.Options;

            var identifierToken = methodDeclaration.Identifier;
            var newName = identifierToken.Text + "Async";

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var symbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

            var newSolution = await Renamer.RenameSymbolAsync(solution, symbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            return newSolution;
        }
    }
}
