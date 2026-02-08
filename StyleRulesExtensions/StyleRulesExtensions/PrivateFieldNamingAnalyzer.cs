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
    public class PrivateFieldNamingAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "private_field_naming";
        private const string CATEGORY = "Naming";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(Resources.PrivateFieldNamingTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(Resources.PrivateFieldNamingMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(Resources.PrivateFieldNamingDescription), Resources.ResourceManager, typeof(Resources));
        private static readonly Regex _nameRegex = new Regex("^_[a-z][a-zA-Z0-9]*$");

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(
            DIAGNOSTIC_ID,
            _title,
            _messageFormat,
            CATEGORY,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: _description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.FieldDeclaration);
        }

        private void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;
            var isPrivate = fieldDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword);
            var isConst = fieldDeclaration.Modifiers.Any(SyntaxKind.ConstKeyword);
            var variable = fieldDeclaration.Declaration.Variables.FirstOrDefault();
            var name = variable?.Identifier.Text;

            if (string.IsNullOrEmpty(name) || !isPrivate || isConst)
                return;

            if (_nameRegex.IsMatch(name))
                return;

            var diagnostic = Diagnostic.Create(_rule, variable.Identifier.GetLocation(), name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
