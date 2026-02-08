using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace StyleRulesExtensions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PropertyNamingAnalyzer : BasePascalCaseNamingAnalyzer<IPropertySymbol>
    {
        public const string DIAGNOSTIC_ID = "property_naming";

        private static readonly SymbolKind _symbolKind = SymbolKind.Property;
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(Resources.PropertyNaminAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(Resources.PropertyNaminAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(Resources.PropertyNaminAnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        public PropertyNamingAnalyzer() : base(_symbolKind, DIAGNOSTIC_ID, _title, _messageFormat, _description)
        { }
    }
}
