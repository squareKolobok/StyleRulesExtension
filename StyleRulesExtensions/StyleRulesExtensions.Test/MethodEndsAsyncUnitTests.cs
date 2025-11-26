using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = StyleRulesExtensions.Test.CSharpCodeFixVerifier<
    StyleRulesExtensions.MethodEndsAsyncAnalyzer,
    StyleRulesExtensions.MethodEndsAsyncCodeFixProvider>;

namespace StyleRulesExtensions.Test
{
    [TestClass]
    public class MethodEndsAsyncUnitTestsUnitTest
    {
        [TestMethod]
        public async Task AsyncMethodEndsWithoutAsync_FixAnythere()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{
				class Program
				{
					public static void Main()
					{
						var program = new Program();
						program.Test();
					}

					Task [|Test|]()
					{
						return Task.CompletedTask;
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
						var program = new Program();
						program.TestAsync();
					}

					Task TestAsync()
					{
						return Task.CompletedTask;
					}
				}
			}";

            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }

        [TestMethod]
        public async Task AsyncMethodEndsWithoutAsync_Diagnostic()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{
				class Program
				{
					Task [|Test|]()
					{
						return Task.CompletedTask;
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task ParameterizationAsyncMethodEndsWithoutAsync_Diagnostic()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{
				class Program
				{
					Task<int> [|Test|]()
					{
						return Task.FromResult(1);
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task PublicMethodControllerEndsWithoutAsync_NoDiagnostic()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{ 
				class ProgramController
				{
					public Task Test()
					{
						return Task.FromResult(1);
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
