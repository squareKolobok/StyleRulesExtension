using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace StyleRulesExtensions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClassNamingAnalyzer : BasePascalCaseNamingAnalyzer<INamedTypeSymbol>
    {
        public const string DIAGNOSTIC_ID = "class_naming";

        private static readonly SymbolKind _symbolKind = SymbolKind.NamedType;
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(Resources.ClassNaminAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(Resources.ClassNaminAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(Resources.ClassNaminAnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        public ClassNamingAnalyzer() : base(_symbolKind, DIAGNOSTIC_ID, _title, _messageFormat, _description)
        { }
    }
}
