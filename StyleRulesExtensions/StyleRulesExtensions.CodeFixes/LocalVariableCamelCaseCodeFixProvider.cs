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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalVariableCamelCaseCodeFixProvider)), Shared]
    public class LocalVariableCamelCaseCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(LocalVariableCamelCaseAnalyzer.DiagnosticId); }
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

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.LocalVariableCamelCaseTitle,
                    createChangedSolution: cancellationToken => RenameLocalVariable(context.Document, declaration, cancellationToken),
                    equivalenceKey: nameof(CodeFixResources.LocalVariableCamelCaseTitle)),
                diagnostic);
        }

        private async Task<Solution> RenameLocalVariable(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken)
        {
            var solution = document.Project.Solution;
            var optionSet = solution.Workspace.Options;

            var variable = localDeclaration.Declaration.Variables.FirstOrDefault();
            var name = variable.Identifier.Text;
            var newNameArray = new List<char>();

            for (var index = 0; index < name.Length; index++)
            {
                var charSymbol = name[index];

                if (char.IsDigit(charSymbol))
                {
                    newNameArray.Add(charSymbol);
                    continue;
                }

                if (char.IsUpper(charSymbol))
                {
                    if (newNameArray.Any())
                        newNameArray.Add(charSymbol);
                    else
                        newNameArray.Add(char.ToLower(charSymbol));
                        
                    continue;
                }

                if (!char.IsLower(charSymbol))
                    continue;

                if (index != 0 && name[index - 1] == '_' && ContainsLetterOrDigit(name, index - 1))
                    newNameArray.Add(char.ToUpper(charSymbol));
                else
                    newNameArray.Add(charSymbol);
            }

            var newName = new string(newNameArray.ToArray());
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var symbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);

            var newSolution = await Renamer.RenameSymbolAsync(solution, symbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

            return newSolution;
        }

        private bool ContainsLetterOrDigit(string name, int length)
        {
            for (var index = length; index >= 0; --index)
                if (char.IsLetterOrDigit(name[index]))
                    return true;

            return false;
        }
    }
}
