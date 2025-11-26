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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalVariableCamelCaseCodeFixProvider)), Shared]
    public class LocalVariableCamelCaseCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(LocalVariableCamelCaseAnalyzer.DiagnosticId); }
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

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.MethodEndsAsyncTitle,
                    createChangedSolution: cancellationToken => RenameMethodAsync(context.Document, declaration, cancellationToken),
                    equivalenceKey: nameof(CodeFixResources.MethodEndsAsyncTitle)),
                diagnostic);
        }

        private async Task<Solution> RenameMethodAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken)
        {
            var solution = document.Project.Solution;
            var optionSet = solution.Workspace.Options;

            var variable = localDeclaration.Declaration.Variables.FirstOrDefault();
            var name = variable.Identifier.Text;
            var newName = $"{char.ToLower(name[0])}{name.Substring(1)}";

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var symbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);

            var newSolution = await Renamer.RenameSymbolAsync(solution, symbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            return newSolution;
        }
    }
}
