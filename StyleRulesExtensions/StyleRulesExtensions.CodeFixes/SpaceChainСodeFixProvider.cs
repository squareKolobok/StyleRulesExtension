using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Data;
using System.Collections.Generic;

namespace StyleRulesExtensions
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SpaceChainСodeFixProvider)), Shared]
    public class SpaceChainСodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(SpaceChainAnalyzer.DiagnosticId); }
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
            var properties = diagnostic.Properties;

            var localDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().FirstOrDefault();
            var ifDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<IfStatementSyntax>().FirstOrDefault();
            var expressionDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ExpressionStatementSyntax>().FirstOrDefault();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.SpaceChainTitle,
                    createChangedDocument: cancellationToken => ChangeSpacesMethodAsync(context.Document,
                        properties,
                        localDeclaration,
                        ifDeclaration,
                        expressionDeclaration,
                        cancellationToken),
                    equivalenceKey: nameof(CodeFixResources.SpaceChainTitle)),
                diagnostic);
        }

        private async Task<Document> ChangeSpacesMethodAsync(
            Document document,
            ImmutableDictionary<string, string> properties,
            LocalDeclarationStatementSyntax localDeclaration,
            IfStatementSyntax ifStatement,
            ExpressionStatementSyntax expressionStatement,
            CancellationToken cancellationToken)
        {
            if (!properties.TryGetValue(SpaceChainAnalyzer.NAME_TAB_SIZE, out var tabSizeString))
                return document;

            var tabSize = int.Parse(tabSizeString);

            var expression = GetExpression(localDeclaration, ifStatement, expressionStatement);

            if (expression == null)
                return document;

            SyntaxNode syntaxNode = localDeclaration;
            syntaxNode = syntaxNode ?? ifStatement;
            syntaxNode = syntaxNode ?? expressionStatement;

            var declarationSpaceSize = GetSpaceSize(syntaxNode);
            var expectedTabSize = declarationSpaceSize + tabSize;
            var tokens = expression.DescendantTokens();
            var tokensDictionary = new Dictionary<SyntaxToken, int>();
            var location = expression.GetLocation();
            var lineSpan = location.GetLineSpan();
            var line = lineSpan.StartLinePosition.Line;

            if (expressionStatement != null)
                tokens = tokens.Skip(1);

            foreach (var token in tokens)
            {
                location = token.GetLocation();
                lineSpan = location.GetLineSpan();
                var tokenLine = lineSpan.StartLinePosition.Line;
                var tokenKind = token.Kind();

                if ((tokenKind == SyntaxKind.CloseParenToken ||
                        tokenKind == SyntaxKind.CloseBracketToken ||
                        tokenKind == SyntaxKind.CloseBraceToken) &&
                    tokenLine == line)
                {
                    expectedTabSize -= tabSize;
                }

                if ((tokenKind == SyntaxKind.OpenParenToken ||
                    tokenKind == SyntaxKind.OpenBracketToken ||
                    tokenKind == SyntaxKind.OpenBraceToken) &&
                    tokenLine == line)
                {
                    expectedTabSize += tabSize;
                }

                if ((tokenKind == SyntaxKind.OpenBraceToken || tokenKind == SyntaxKind.CloseBraceToken) && tokenLine != line)
                    expectedTabSize -= tabSize;

                if (token.HasLeadingTrivia)
                {
                    var spaceSize = GetSpaceSize(token);
                    var differenceTabs = Math.Abs(spaceSize - expectedTabSize);

                    if (differenceTabs != 0)
                        tokensDictionary.Add(token, expectedTabSize);
                }

                if ((tokenKind == SyntaxKind.OpenBraceToken || tokenKind == SyntaxKind.CloseBraceToken) && tokenLine != line)
                    expectedTabSize += tabSize;

                line = tokenLine;
            }

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var newRoot = oldRoot.ReplaceTokens(tokensDictionary.Keys, (token1, token2) =>
            {
                var size = tokensDictionary[token1];

                var trivia = SyntaxFactory.Whitespace(new string(' ', size));
                var newToken = token1.WithLeadingTrivia(trivia);

                return newToken;
            });

            var str = newRoot.ToString();

            return document.WithSyntaxRoot(newRoot);
        }

        private ExpressionSyntax GetExpression(
            LocalDeclarationStatementSyntax localDeclaration,
            IfStatementSyntax ifStatement,
            ExpressionStatementSyntax expressionStatement)
        {
            if (localDeclaration != null)
            {
                var variables = localDeclaration.Declaration.Variables;

                if (variables.Count > 1 || variables.Count == 0)
                    return null;

                return variables[0].Initializer.Value;
            }

            if (ifStatement != null)
            {
                var condition = ifStatement.Condition;

                return condition;
            }

            return expressionStatement?.Expression;
        }

        private int GetSpaceSize(SyntaxNode node)
        {
            if (!node.HasLeadingTrivia)
                return 0;

            var trivial = node.GetLeadingTrivia();
            var spaceTrivia = trivial.FirstOrDefault(x => x.IsKind(SyntaxKind.WhitespaceTrivia));

            if (spaceTrivia != null)
            {
                var span = spaceTrivia.Span;
                return span.Length;
            }

            return 0;
        }

        private int GetSpaceSize(SyntaxToken tocken)
        {
            var spaceTrivia = GetTrivia(tocken);

            if (spaceTrivia != null)
            {
                var span = spaceTrivia.Span;
                return span.Length;
            }

            return 0;
        }

        private SyntaxTrivia GetTrivia(SyntaxToken tocken)
        {
            var trivial = tocken.LeadingTrivia;
            var spaceTrivia = trivial.FirstOrDefault(x => x.IsKind(SyntaxKind.WhitespaceTrivia));

            return spaceTrivia;
        }
    }
}
