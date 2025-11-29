using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace StyleRulesExtensions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConstantCapitalCaseAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "constant_capital_case_naming";
        private const string Category = "Naming";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.ConstantCapitalCaseTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.ConstantCapitalCaseMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.ConstantCapitalCaseDescription), Resources.ResourceManager, typeof(Resources));
        private static readonly Regex nameRegex = new Regex("^[A-Z][A-Z0-9_]*[A-Z0-9]$");
        
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
            context.RegisterSyntaxNodeAction(AnalyzeFieldSymbol, SyntaxKind.FieldDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeLocalSymbol, SyntaxKind.LocalDeclarationStatement);
        }

        private void AnalyzeFieldSymbol(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;
            var isConst = fieldDeclaration.Modifiers.Any(SyntaxKind.ConstKeyword);
            var variable = fieldDeclaration.Declaration.Variables.FirstOrDefault();
            var name = variable?.Identifier.Text;

            if (string.IsNullOrEmpty(name) || !isConst)
                return;

            Analyze(context, variable, name);
        }

        private void AnalyzeLocalSymbol(SyntaxNodeAnalysisContext context)
        {
            var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;
            var isConst = localDeclaration.IsConst;
            var variable = localDeclaration.Declaration.Variables.FirstOrDefault();
            var name = variable?.Identifier.Text;

            if (string.IsNullOrEmpty(name) || !isConst)
                return;

            Analyze(context, variable, name);
        }

        private void Analyze(SyntaxNodeAnalysisContext context, VariableDeclaratorSyntax variable, string name)
        {
            if (nameRegex.IsMatch(name))
                return;

            var diagnostic = Diagnostic.Create(Rule, variable.Identifier.GetLocation(), name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
