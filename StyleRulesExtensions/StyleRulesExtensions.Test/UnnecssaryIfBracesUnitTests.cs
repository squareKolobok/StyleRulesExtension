using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = StyleRulesExtensions.Test.CSharpCodeFixVerifier<
    StyleRulesExtensions.UnnecssaryIfBracesAnalyzer,
    StyleRulesExtensions.UnnecssaryIfBracesCodeFixProvider>;

namespace StyleRulesExtensions.Test
{
    [TestClass]
    public class UnnecssaryIfBracesUnitTest
    {
        [TestMethod]
        public async Task RemoveBracketWithOneExression()
        {
            var test = @"
			namespace ConsoleApp
			{ 
				class Program
				{
					static void Main(string[] args)
					{
						var x = 1;
			
						[|if (x == 2)
						{
							x = 3;
						}|]
					}
				}
			}";

            var fixtest = @"
			namespace ConsoleApp
			{ 
				class Program
				{
					static void Main(string[] args)
					{
						var x = 1;
			
						if (x == 2)
							x = 3;
					}
				}
			}";

            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }

        [TestMethod]
        public async Task UnremoveBracketWithManyExressions_NoDiagnostic()
        {
            var test = @"
			namespace ConsoleApp
			{ 
				class Program
				{
					static void Main(string[] args)
					{
						var x = 1;
			
						if (x == 2)
						{
							x = 3;
							x = 3;
						}
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task UnremoveBracketWithoutExression_NoDiagnostic()
        {
            var test = @"
			namespace ConsoleApp
			{ 
				class Program
				{
					static void Main(string[] args)
					{
						var x = 1;
			
						if (x == 2)
						{
						}
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task UnremoveBracketWithOneExressionAndMultiLineCondition_NoDiagnostic()
        {
            var test = @"
			namespace ConsoleApp
			{ 
				class Program
				{
					static void Main(string[] args)
					{
						var x = 1;
			
						if (x == 2 ||
							x == 1)
						{
							x = 3;
						}
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task UnremoveBracketWithManyExressionsAndMultiLineCondition_NoDiagnostic()
        {
            var test = @"
			namespace ConsoleApp
			{ 
				class Program
				{
					static void Main(string[] args)
					{
						var x = 1;
			
						if (x == 2 ||
							x == 1)
						{
							x = 3;
							x = 3;
						}
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
