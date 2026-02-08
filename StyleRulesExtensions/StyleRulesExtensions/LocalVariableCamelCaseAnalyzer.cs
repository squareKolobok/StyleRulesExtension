using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace StyleRulesExtensions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LocalVariableCamelCaseAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "local_variables_camel_case_naming";
        private const string CATEGORY = "Naming";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(Resources.LocalVariableCamelCaseTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(Resources.LocalVariableCamelCaseMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(Resources.LocalVariableCamelCaseDescription), Resources.ResourceManager, typeof(Resources));
        private static readonly Regex _nameRegex = new Regex("^@?[a-z][a-zA-Z0-9]*$");

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

            if (_nameRegex.IsMatch(name))
                return;

            var diagnostic = Diagnostic.Create(_rule, variable.Identifier.GetLocation(), name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
