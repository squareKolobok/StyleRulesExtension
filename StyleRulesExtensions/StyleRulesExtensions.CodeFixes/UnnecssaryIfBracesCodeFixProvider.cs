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
            var isExistElse = ifStatement.Else != null;
            var hasOneStatement = ifBlock != null && ifBlock.Statements.Count == 1;
            var isExistInnerIf = hasOneStatement && ifBlock.Statements.First() is IfStatementSyntax;
            var isNotExistInnerElse = isExistInnerIf && (ifBlock.Statements.First() as IfStatementSyntax).Else == null;
            var elseBlock = ifStatement.Else?.Statement as BlockSyntax;
            var ifHasSingleStatementManyLines = SingleStatementHasManyLines(ifBlock);
            var elseHasSingleStatementManyLines = SingleStatementHasManyLines(elseBlock);
            var newIfBlock = GetNewBlockStatement(ifBlock);
            var newElseBlock = GetNewBlockStatement(elseBlock);
            IfStatementSyntax newIfStatement = ifStatement;

            if (newIfBlock != null &&
                hasOneStatement &&
                !(isExistElse && isExistInnerIf && isNotExistInnerElse) &&
                !ifHasSingleStatementManyLines)
            {
                newIfStatement = ifStatement.WithStatement(newIfBlock);
            }

            if (newElseBlock != null && !elseHasSingleStatementManyLines)
            {
                var elseClause = ifStatement.Else;
                var newElseClause = elseClause.WithStatement(newElseBlock);
                newIfStatement = newIfStatement.WithElse(newElseClause);
            }

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = oldRoot.ReplaceNode(ifStatement, newIfStatement);

            return document.WithSyntaxRoot(newRoot);
        }

        private bool SingleStatementHasManyLines(BlockSyntax block)
        {
            if (block == null || block.Statements.Count != 1)
                return false;

            var statement = block.Statements.First();
            var positionSpan = statement.GetLocation().GetLineSpan();
            var startLine = positionSpan.StartLinePosition.Line;
            var endLine = positionSpan.EndLinePosition.Line;
            var countLines = endLine - startLine + 1;

            return countLines > 1;
        }

        private StatementSyntax GetNewBlockStatement(BlockSyntax block)
        {
            if (block == null || block.Statements.Count != 1 || SingleStatementHasManyLines(block))
                return block;

            var leadingTrivia = block.Statements[0].GetLeadingTrivia();

            return block.Statements[0]
                .WithLeadingTrivia(block.OpenBraceToken.LeadingTrivia)
                .WithTrailingTrivia(block.CloseBraceToken.TrailingTrivia)
                .WithLeadingTrivia(leadingTrivia);
        }
    }
}
