using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StyleRulesExtensions
{
    public abstract class BasePascalCaseNamingCodeFixProvider<TDeclarationSyntax> : CodeFixProvider where TDeclarationSyntax : MemberDeclarationSyntax
    {
        private readonly string _title;
        private readonly string _equivalenceKey;

        public BasePascalCaseNamingCodeFixProvider(string title, string equivalenceKey)
        {
            _title = title;
            _equivalenceKey = equivalenceKey;
        }

        public override ImmutableArray<string> FixableDiagnosticIds => throw new NotImplementedException();

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: _title,
                    createChangedSolution: cancellationToken => RenameMethodAsync(context.Document, declaration, cancellationToken),
                    equivalenceKey: _equivalenceKey),
                diagnostic);
        }

        private async Task<Solution> RenameMethodAsync(
            Document document,
            TDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken)
        {
            var solution = document.Project.Solution;
            var optionSet = solution.Workspace.Options;

            var name = GetName(methodDeclaration);
            var newName = char.ToUpper(name[0]) + name.Substring(1);

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var symbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

            var newSolution = await Renamer.RenameSymbolAsync(solution, symbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            return newSolution;
        }

        protected abstract string GetName(TDeclarationSyntax declaration);
    }
}
