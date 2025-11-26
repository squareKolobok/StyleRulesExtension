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
        public const string DiagnosticId = "unnecssary_if_braces";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.UnnecssaryIfBracesAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.UnnecssaryIfBracesAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.UnnecssaryIfBracesAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Usage";

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
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.IfStatement);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;
            var condition = ifStatement.Condition;
            var conditionSpan = condition.GetLocation().GetLineSpan();
            var startLine = conditionSpan.StartLinePosition.Line;
            var endLine = conditionSpan.EndLinePosition.Line;
            var conditionCountLines = endLine - startLine + 1;
            var block = ifStatement.Statement as BlockSyntax;

            if (conditionCountLines > 1 || block == null || block.Statements.Count != 1)
                return;

            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }
    }
}
