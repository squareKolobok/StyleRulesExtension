using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace StyleRulesExtensions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodEndsAsyncAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "method_ends_async";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.MethodEndsAsyncAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.MethodEndsAsyncAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.MethodEndsAsyncAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

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
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var name = method.Identifier.ValueText;
            var identifierReturnType = method.ReturnType as IdentifierNameSyntax;
            var genericReturnType = method.ReturnType as GenericNameSyntax;

            if (identifierReturnType == null && genericReturnType == null)
                return;

            var identifier = identifierReturnType?.Identifier.ValueText ?? genericReturnType.Identifier.ValueText;
            var isOverrided = method.Modifiers.Any(SyntaxKind.OverrideKeyword);

            if (isOverrided || identifier != "Task" || name.EndsWith("Async"))
                return;

            var isPublic = method.Modifiers.Any(SyntaxKind.PublicKeyword);
            var parent = context.Node.Parent as ClassDeclarationSyntax;
            var parentName = parent?.Identifier.ValueText;
            var isController = parentName?.EndsWith("Controller") == true;

            if (isPublic && isController)
                return;

            context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), name));
        }
    }
}
