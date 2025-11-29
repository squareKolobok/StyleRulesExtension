using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = StyleRulesExtensions.Test.CSharpCodeFixVerifier<
    StyleRulesExtensions.LocalVariableCamelCaseAnalyzer,
    StyleRulesExtensions.LocalVariableCamelCaseCodeFixProvider>;

namespace StyleRulesExtensions.Test
{
    [TestClass]
    public class LocalVariableCamelCaseUnitTests
    {
        [TestMethod]
        public async Task LocalVariablePascalCase_FixAnythere()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{
				class Program
				{
					public static void Main()
					{
						const int MY_CONSTANT = 100;
						var [|LocalVariable|] = 2;
						var [|local_variable2|] = 3;
						var [|_Local_variable3|] = 3;
						var [|Class|] = 4;
						LocalVariable = LocalVariable * MY_CONSTANT;
					}
				}
			}";

            var fixtest = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{
				class Program
				{
					public static void Main()
					{
						const int MY_CONSTANT = 100;
						var localVariable = 2;
						var localVariable2 = 3;
						var localVariable3 = 3;
						var @class = 4;
						localVariable = localVariable * MY_CONSTANT;
					}
				}
			}";

            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }

        [TestMethod]
        public async Task LocalVariablePascalCase_Diagnostic()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{
				class Program
				{
					public static void Main()
					{
						const int MY_CONSTANT = 100;
						var [|LocalVariables|] = 2;
						var [|local_variable2|] = 3;
						var localVariables2 = LocalVariables * MY_CONSTANT;
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task LocalVariableCamelCase_NoDiagnostic()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{
				class Program
				{
					public static void Main()
					{
						const int MY_CONSTANT = 100;
						var localVariables = 2;
						localVariables = localVariables * MY_CONSTANT;
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
