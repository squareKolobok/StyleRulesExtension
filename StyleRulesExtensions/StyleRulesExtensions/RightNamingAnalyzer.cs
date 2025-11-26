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
    public class RightNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "right_naming";
        private const string Category = "Naming";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.RightNamingAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.RightNamingAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.RightNamingAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private static readonly Regex nameRegex = new Regex("^@?[a-zA-Z_][a-zA-Z_0-9]*$");

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol,
                SymbolKind.Namespace,
                SymbolKind.NamedType,
                SymbolKind.Field,
                SymbolKind.Method,
                SymbolKind.Parameter,
                SymbolKind.Property);
            context.RegisterSyntaxNodeAction(AnalyzeLocalName, SyntaxKind.LocalDeclarationStatement);
        }

        private void AnalyzeLocalName(SyntaxNodeAnalysisContext context)
        {
            var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;
            var variables = localDeclaration.Declaration.Variables;
            var badVariable = variables.Where(x => !string.IsNullOrEmpty(x.Identifier.Text))
                .FirstOrDefault(x => !nameRegex.IsMatch(x.Identifier.Text));

            if (badVariable == null)
                return;

            var name = badVariable.Identifier.Text;

            var diagnostic = Diagnostic.Create(Rule, badVariable.Identifier.GetLocation(), name);
            context.ReportDiagnostic(diagnostic);
        }

        private void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var symbol = context.Symbol;
            var name = symbol.Name;

            if (nameRegex.IsMatch(name))
                return;

            var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
