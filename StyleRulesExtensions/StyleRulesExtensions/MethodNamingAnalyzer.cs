using Microsoft.CodeAnalysis;

namespace StyleRulesExtensions
{
    public class MethodNamingAnalyzer : BasePascalCaseNamingAnalyzer<IMethodSymbol>
    {
        public const string DiagnosticId = "method_naming";

        private static readonly SymbolKind symbolKind = SymbolKind.Method;
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.MethodNaminAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.MethodNaminAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.MethodNaminAnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        public MethodNamingAnalyzer() : base(symbolKind, DiagnosticId, Title, MessageFormat, Description)
        { }
    }
}
