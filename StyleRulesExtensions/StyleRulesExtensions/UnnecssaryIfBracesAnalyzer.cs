using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace StyleRulesExtensions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnnecssaryIfBracesAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "unnecssary_if_braces";

        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(Resources.UnnecssaryIfBracesAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(Resources.UnnecssaryIfBracesAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(Resources.UnnecssaryIfBracesAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string CATEGORY = "Usage";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
            DIAGNOSTIC_ID,
            _title,
            _messageFormat,
            CATEGORY,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: _description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.IfStatement);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;
            var condition = ifStatement.Condition;
            var conditionSpan = condition.GetLocation().GetLineSpan();

            if (HasManyLines(conditionSpan))
                return;

            var elseBlock = ifStatement.Else?.Statement as BlockSyntax;

            if (IsExistUnnecessaryBracesIf(ifStatement) ||
                IsExistUnnecessaryBracesElse(elseBlock))
            {
                context.ReportDiagnostic(Diagnostic.Create(_rule, ifStatement.GetLocation()));
            }
        }

        private bool HasManyLines(FileLinePositionSpan positionSpan)
        {
            var startLine = positionSpan.StartLinePosition.Line;
            var endLine = positionSpan.EndLinePosition.Line;
            var countLines = endLine - startLine + 1;

            return countLines > 1;
        }

        private bool IsExistUnnecessaryBracesIf(IfStatementSyntax ifStatement)
        {
            var isExistElse = ifStatement.Else != null;
            var ifBlock = ifStatement.Statement as BlockSyntax;
            var hasOneStatement = ifBlock != null && ifBlock.Statements.Count == 1;
            var isExistInnerIf = hasOneStatement && ifBlock.Statements.First() is IfStatementSyntax;
            var isNotExistInnerElse = isExistInnerIf && (ifBlock.Statements.First() as IfStatementSyntax).Else == null;
            var hasManyLines = hasOneStatement && HasManyLines(ifBlock.Statements.First().GetLocation().GetLineSpan());

            return !hasManyLines && hasOneStatement && !(isExistElse && isExistInnerIf && isNotExistInnerElse);
        }

        private bool IsExistUnnecessaryBracesElse(BlockSyntax block)
        {
            var isExistBlock = block != null;
            var hasOneStatement = isExistBlock && block.Statements.Count == 1;
            var hasManyLines = hasOneStatement && HasManyLines(block.Statements.First().GetLocation().GetLineSpan());

            return block != null && block.Statements.Count == 1 && !hasManyLines;
        }
    }
}
