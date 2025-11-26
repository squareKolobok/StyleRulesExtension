using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace StyleRulesExtensions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LocalVariableCamelCaseAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "local_variables_camel_case_naming";
        private const string Category = "Naming";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.LocalVariableCamelCaseTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.LocalVariableCamelCaseMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.LocalVariableCamelCaseDescription), Resources.ResourceManager, typeof(Resources));

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
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.LocalDeclarationStatement);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;
            var variable = localDeclaration.Declaration.Variables.FirstOrDefault();
            var name = variable.Identifier.Text;

            if (localDeclaration.IsConst)
                return;

            if (string.IsNullOrEmpty(name))
                return;

            if (char.IsLower(name.First()))
                return;

            var diagnostic = Diagnostic.Create(Rule, variable.Identifier.GetLocation(), name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
