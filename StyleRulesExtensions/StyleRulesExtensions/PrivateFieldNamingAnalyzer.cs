using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

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
            const int MIN_NAME_LENGTH = 2;

            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;
            var isPrivate = fieldDeclaration.Modifiers.Any(SyntaxKind.PrivateKeyword);
            var variable = fieldDeclaration.Declaration.Variables.FirstOrDefault();
            var name = variable?.Identifier.Text;

            if (string.IsNullOrEmpty(name) || !isPrivate)
                return;

            if (name.Length >= MIN_NAME_LENGTH && name.First() == '_' && char.IsLower(name[1]))
                return;

            var diagnostic = Diagnostic.Create(Rule, variable.Identifier.GetLocation(), name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
