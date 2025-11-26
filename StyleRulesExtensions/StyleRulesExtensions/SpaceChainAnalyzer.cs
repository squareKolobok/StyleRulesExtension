using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace StyleRulesExtensions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SpaceChainAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "space_chain";
        public const int DEFAULT_TAB_SIZE = 4;
        public const string NAME_TAB_SIZE = "TabSize";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.SpaceChainTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.SpaceChainMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.SpaceChainDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Styling";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.LocalDeclarationStatement, SyntaxKind.ExpressionStatement, SyntaxKind.IfStatement);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var statement = context.Node;
            var location = statement.GetLocation();
            var lineSpan = location.GetLineSpan();

            if (lineSpan.StartLinePosition.Line == lineSpan.EndLinePosition.Line)
                return;

            var expression = GetExpression(statement);
            
            if (expression == null)
                return;

            location = expression.GetLocation();
            lineSpan = location.GetLineSpan();

            if (lineSpan.StartLinePosition.Line == lineSpan.EndLinePosition.Line)
                return;

            var tabSize = GetOneTabSize(context);
            var declarationSpaceSize = GetSpaceSize(statement);
            var expectedTabSize = declarationSpaceSize + tabSize;
            var tokens = expression.DescendantTokens();

            foreach (var token in tokens)
            {
                if (!token.HasLeadingTrivia)
                    continue;

                var spaceSize = GetSpaceSize(token);
                var differenceTabs = Math.Abs(spaceSize - expectedTabSize);

                if (differenceTabs != tabSize && differenceTabs != 0)
                {
                    var dictionary = new Dictionary<string, string>
                    {
                        { NAME_TAB_SIZE, tabSize.ToString() }
                    };
                    var diagnostic = Diagnostic.Create(Rule, location, dictionary.ToImmutableDictionary(), differenceTabs, tabSize);
                    context.ReportDiagnostic(diagnostic);

                    return;
                }

                expectedTabSize = spaceSize;
            }
        }

        private ExpressionSyntax GetExpression(SyntaxNode node)
        {
            var localDeclaration = node as LocalDeclarationStatementSyntax;

            if (localDeclaration != null)
            {
                var variables = localDeclaration.Declaration.Variables;

                if (variables.Count > 1 || variables.Count == 0)
                    return null;

                return variables[0].Initializer.Value;
            }

            var ifStatement = node as IfStatementSyntax;

            if (ifStatement != null)
            {
                var condition = ifStatement.Condition;

                return condition;
            }

            var expressionSyntax = node as ExpressionStatementSyntax;

            return expressionSyntax?.Expression;
        }

        private int GetOneTabSize(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            var preveousNode = node;
            var typeNode = node as TypeDeclarationSyntax;

            while (node != null && typeNode == null)
            {
                preveousNode = node;
                node = node.Parent;
                typeNode = node as TypeDeclarationSyntax;
            }

            if (node == null && typeNode == null)
                return DEFAULT_TAB_SIZE;

            var typeSize = GetSpaceSize(typeNode);
            var preveousNodeSize = GetSpaceSize(preveousNode);
            var tabSize = Math.Max(0, preveousNodeSize - typeSize);

            return tabSize;
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
            if (!tocken.HasLeadingTrivia)
                return 0;

            var trivial = tocken.LeadingTrivia;
            var spaceTrivia = trivial.FirstOrDefault(x => x.IsKind(SyntaxKind.WhitespaceTrivia));

            if (spaceTrivia != null)
            {
                var span = spaceTrivia.Span;
                return span.Length;
            }

            return 0;
        }
    }
}
