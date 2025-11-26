using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace StyleRulesExtensions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PropertyNamingAnalyzer : BasePascalCaseNamingAnalyzer<IPropertySymbol>
    {
        public const string DiagnosticId = "property_naming";

        private static readonly SymbolKind symbolKind = SymbolKind.Property;
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.PropertyNaminAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.PropertyNaminAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.PropertyNaminAnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        public PropertyNamingAnalyzer() : base(symbolKind, DiagnosticId, Title, MessageFormat, Description)
        { }
    }
}
