using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StyleRulesExtensions
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnnecssaryIfBracesCodeFixProvider)), Shared]
    public class UnnecssaryIfBracesCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(UnnecssaryIfBracesAnalyzer.DiagnosticId); }
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

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.UnnecssaryIfBracesTitle,
                    createChangedDocument: cancellationToken => RemoveBrackets(context.Document, declaration, cancellationToken),
                    equivalenceKey: nameof(CodeFixResources.UnnecssaryIfBracesTitle)),
                diagnostic);
        }

        private async Task<Document> RemoveBrackets(Document document, IfStatementSyntax ifStatement, CancellationToken cancellationToken)
        {
            var ifBlock = ifStatement.Statement as BlockSyntax;
            var elseBlock = ifStatement.Else?.Statement as BlockSyntax;
            var newIfBlock = GetNewBlockStatement(ifBlock);
            var newElseBlock = GetNewBlockStatement(elseBlock);
            IfStatementSyntax newIfStatement = ifStatement;

            if (newIfBlock != null)
                newIfStatement = ifStatement.WithStatement(newIfBlock);

            if (newElseBlock != null)
            {
                var elseClause = ifStatement.Else;
                var newElseClause = elseClause.WithStatement(newElseBlock);
                newIfStatement = newIfStatement.WithElse(newElseClause);
            }

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = oldRoot.ReplaceNode(ifStatement, newIfStatement);

            return document.WithSyntaxRoot(newRoot);
        }

        private StatementSyntax GetNewBlockStatement(BlockSyntax block)
        {
            if (block == null || block.Statements.Count != 1)
                return block;

            var leadingTrivia = block.Statements[0].GetLeadingTrivia();

            return block.Statements[0]
                .WithLeadingTrivia(block.OpenBraceToken.LeadingTrivia)
                .WithTrailingTrivia(block.CloseBraceToken.TrailingTrivia)
                .WithLeadingTrivia(leadingTrivia);
        }
    }
}
