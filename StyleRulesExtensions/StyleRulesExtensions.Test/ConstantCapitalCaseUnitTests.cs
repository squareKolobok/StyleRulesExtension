using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = StyleRulesExtensions.Test.CSharpCodeFixVerifier<
    StyleRulesExtensions.ConstantCapitalCaseAnalyzer,
    StyleRulesExtensions.ConstantCapitalCaseCodeFixProvider>;

namespace StyleRulesExtensions.Test
{
    [TestClass]
    public class ConstantCapitalCaseUnitTests
    {
        [TestMethod]
        public async Task NoConstant_NoDiagnostic()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{
				class Program
				{
					protected int Field1 = 1;
					public int Field2 = 2;
					internal int Field3 = 3;
					int Field4 = 4;

					public static void Main()
					{
						var local1 = 1;
						var local2 = 2;
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task ConstantsCapitalCase_NoDiagnostic()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{
				class Program
				{
					protected const int CONSTANT_NAME1 = 1;
					public const int CONSTANT_NAME2 = 2;
					internal const int CONSTANT_NAME3 = 3;
					const int CONSTANT_NAME4 = 4;

					public static void Main()
					{
						const int LOCAL_CONSTANT = 1;
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task ClassConstantsAreNotCapitalCase_Diagnostic()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{
				class Program
				{
					private const int [|const_name1|] = 1;
					public const int [|const_name2|] = 2;
					protected const int [|const_name3|] = 3;
					const int [|const_name4|] = 4;

					public static void Main()
					{
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task MethodConstantIsNotCapitalCase_Diagnostic()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{
				class Program
				{
					public static void Main()
					{
						const int [|myConst|] = 1;
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task PrivateFieldsNamingAreNotCorrect_FixAnythere()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{ 
				class Program
				{
					private const int [|const_name1|] = 1;
					public const int [|const_name2|] = 2;
					protected const int [|const_name3|] = 3;
					internal const int [|const_name4|] = 4;
					const int CONST_NAME5 = 5;

					public static void Main()
					{
						const int [|myConst|] = 1;
						const int MY_CONST2 = 2;
					}
				}
			}";

            var fixtest = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{ 
				class Program
				{
					private const int CONST_NAME1 = 1;
					public const int CONST_NAME2 = 2;
					protected const int CONST_NAME3 = 3;
					internal const int CONST_NAME4 = 4;
					const int CONST_NAME5 = 5;

					public static void Main()
					{
						const int MYCONST = 1;
						const int MY_CONST2 = 2;
					}
				}
			}";

            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }
    }
}
