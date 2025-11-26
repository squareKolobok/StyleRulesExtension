using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace StyleRulesExtensions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public abstract class BasePascalCaseNamingAnalyzer<TSymbol> : DiagnosticAnalyzer where TSymbol : ISymbol
    {
        private const string Category = "Naming";
        private readonly DiagnosticDescriptor _rule;
        private readonly SymbolKind _symbolKind;

        public BasePascalCaseNamingAnalyzer(SymbolKind symbolKind, string diagnosticId, LocalizableString title, LocalizableString messageFormat, LocalizableString description)
        {
            _symbolKind = symbolKind;
            _rule = new DiagnosticDescriptor(
                diagnosticId,
                title,
                messageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: description);
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol, _symbolKind);
        }

        private void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var namedTypeSymbol = (TSymbol)context.Symbol;
            var name = namedTypeSymbol.Name;

            if (string.IsNullOrEmpty(name))
                return;

            if (char.IsUpper(name.First()))
                return;

            var diagnostic = Diagnostic.Create(_rule, namedTypeSymbol.Locations[0], name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
