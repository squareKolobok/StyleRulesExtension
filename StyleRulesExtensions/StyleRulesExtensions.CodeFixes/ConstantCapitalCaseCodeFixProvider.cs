using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StyleRulesExtensions
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConstantCapitalCaseCodeFixProvider)), Shared]
    public class ConstantCapitalCaseCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ConstantCapitalCaseAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declarationField = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<FieldDeclarationSyntax>().FirstOrDefault();
            var declarationLocal = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().FirstOrDefault();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.PrivateFieldNamingTitle,
                    createChangedSolution: cancellationToken => RenameFieldAsync(context.Document, declarationField, declarationLocal, cancellationToken),
                    equivalenceKey: nameof(CodeFixResources.PrivateFieldNamingTitle)),
                diagnostic);
        }

        private async Task<Solution> RenameFieldAsync(
            Document document,
            FieldDeclarationSyntax fieldDeclaration,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken)
        {
            var solution = document.Project.Solution;
            var optionSet = solution.Workspace.Options;

            var variable = fieldDeclaration?.Declaration.Variables.First();

            if (variable == null)
                variable = localDeclaration.Declaration.Variables.First();

            var name = variable.Identifier.Text;
            var newNameChars = new List<char>();

            foreach (var charSymbol in name)
            {
                if (charSymbol == '_' && newNameChars.LastOrDefault() != '_')
                    newNameChars.Add(charSymbol);

                if (char.IsDigit(charSymbol) || char.IsUpper(charSymbol))
                    newNameChars.Add(charSymbol);

                if (char.IsLower(charSymbol))
                    newNameChars.Add(char.ToUpper(charSymbol));
            }

            var newName = new string(newNameChars.ToArray());
            newName = newName.Trim('_');

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var symbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);

            var newSolution = await Renamer.RenameSymbolAsync(solution, symbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            return newSolution;
        }
    }
}
