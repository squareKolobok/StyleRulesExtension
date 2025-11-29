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
        public const string DiagnosticId = "private_field_naming";
        private const string Category = "Naming";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.PrivateFieldNamingTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.PrivateFieldNamingMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.PrivateFieldNamingDescription), Resources.ResourceManager, typeof(Resources));
        private static readonly Regex nameRegex = new Regex("^_[a-z][a-zA-Z0-9]*$");

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

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

            if (nameRegex.IsMatch(name))
                return;

            var diagnostic = Diagnostic.Create(Rule, variable.Identifier.GetLocation(), name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
