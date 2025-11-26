using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace StyleRulesExtensions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClassNamingAnalyzer : BasePascalCaseNamingAnalyzer<INamedTypeSymbol>
    {
        public const string DiagnosticId = "class_naming";

        private static readonly SymbolKind symbolKind = SymbolKind.NamedType;
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.ClassNaminAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.ClassNaminAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.ClassNaminAnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        public ClassNamingAnalyzer() : base(symbolKind, DiagnosticId, Title, MessageFormat, Description)
        { }
    }
}
