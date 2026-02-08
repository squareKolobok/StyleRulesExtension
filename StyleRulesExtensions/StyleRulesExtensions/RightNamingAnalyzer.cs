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
        public const string DIAGNOSTIC_ID = "right_naming";
        private const string CATEGORY = "Naming";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(Resources.RightNamingAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(Resources.RightNamingAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(Resources.RightNamingAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private static readonly Regex _nameRegex = new Regex("^@?[a-zA-Z_][a-zA-Z_0-9]*$");

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
            DIAGNOSTIC_ID,
            _title,
            _messageFormat,
            CATEGORY,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: _description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

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
                .FirstOrDefault(x => !_nameRegex.IsMatch(x.Identifier.Text));

            if (badVariable == null)
                return;

            var name = badVariable.Identifier.Text;

            var diagnostic = Diagnostic.Create(_rule, badVariable.Identifier.GetLocation(), name);
            context.ReportDiagnostic(diagnostic);
        }

        private void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var symbol = context.Symbol;
            var name = symbol.Name;

            if (_nameRegex.IsMatch(name))
                return;

            if (symbol.Kind == SymbolKind.Method)
            {
                var methodSymbol = symbol as IMethodSymbol;

                if (methodSymbol.MethodKind == MethodKind.PropertyGet ||
                    methodSymbol.MethodKind == MethodKind.PropertySet ||
                    methodSymbol.MethodKind != MethodKind.Ordinary)
                {
                    return;
                }
            }

            var diagnostic = Diagnostic.Create(_rule, symbol.Locations[0], name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
