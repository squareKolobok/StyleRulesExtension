using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace StyleRulesExtensions
{
    public abstract class BasePascalCaseNamingAnalyzer<TSymbol> : DiagnosticAnalyzer where TSymbol : ISymbol
    {
        private const string CATEGORY = "Naming";
        private readonly DiagnosticDescriptor _rule;
        private readonly SymbolKind _symbolKind;
        private static readonly Regex _nameRegex = new Regex("^@?[A-Z][a-zA-Z0-9]*$");

        public BasePascalCaseNamingAnalyzer(SymbolKind symbolKind, string diagnosticId, LocalizableString title, LocalizableString messageFormat, LocalizableString description)
        {
            _symbolKind = symbolKind;
            _rule = new DiagnosticDescriptor(
                diagnosticId,
                title,
                messageFormat,
                CATEGORY,
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

        protected virtual bool NeedEndDiagnistic(TSymbol symbol)
        {
            return false;
        }

        private void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var namedTypeSymbol = (TSymbol)context.Symbol;
            var name = namedTypeSymbol.Name;

            if (NeedEndDiagnistic(namedTypeSymbol))
                return;

            if (string.IsNullOrEmpty(name))
                return;

            if (_nameRegex.IsMatch(name))
                return;

            var diagnostic = Diagnostic.Create(_rule, namedTypeSymbol.Locations[0], name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
