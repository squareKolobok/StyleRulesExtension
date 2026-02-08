using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace StyleRulesExtensions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodNamingAnalyzer : BasePascalCaseNamingAnalyzer<IMethodSymbol>
    {
        public const string DIAGNOSTIC_ID = "method_naming";

        private static readonly SymbolKind _symbolKind = SymbolKind.Method;
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(Resources.MethodNaminAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(Resources.MethodNaminAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(Resources.MethodNaminAnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        public MethodNamingAnalyzer() : base(_symbolKind, DIAGNOSTIC_ID, _title, _messageFormat, _description)
        { }

        protected override bool NeedEndDiagnistic(IMethodSymbol symbol)
        {
            if (symbol.IsOverride ||
                symbol.MethodKind == MethodKind.PropertyGet ||
                symbol.MethodKind == MethodKind.PropertySet)
            {
                return true;
            }

            return symbol.MethodKind != MethodKind.Ordinary;
        }
    }
}
