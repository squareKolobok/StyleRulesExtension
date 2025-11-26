using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = StyleRulesExtensions.Test.CSharpCodeFixVerifier<
    StyleRulesExtensions.PrivateFieldNamingAnalyzer,
    StyleRulesExtensions.PrivateFieldNamingCodeFixProvider>;

namespace StyleRulesExtensions.Test
{
    [TestClass]
    public class PrivateFieldNamingUnitTests
    {
        [TestMethod]
        public async Task NotPrivateFieldsNamingInPacalCase_NoDiagnostic()
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
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task PrivateFieldNamingInPacalCase_Diagnostic()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{
				class Program
				{
					private int [|Field1|] = 1;

					public static void Main()
					{
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task PrivateFieldNamingInCamelCaseWithoutUnderscore_Diagnostic()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{
				class Program
				{
					private int [|field1|] = 1;

					public static void Main()
					{
					}
				}
			}";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task PrivateFieldNamingInCamelCaseWithUnderscore_NoDiagnostic()
        {
            var test = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{ 
				class Program
				{
					private int _field1 = 1;

					public static void Main()
					{
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
					private int [|Field1|] = 1;
					private int [|field2|] = 2;
					private int [|_Field3|] = 3;
					private int _field4 = 4;

					public static void Main()
					{
						var program = new Program();
						program.Field1 = 1;
						program.field2 = 2;
						program._Field3 = 3;
						program._field4 = 4;
					}
				}
			}";

            var fixtest = @"
			using System.Threading.Tasks;

			namespace ConsoleApp
			{ 
				class Program
				{
					private int _field1 = 1;
					private int _field2 = 2;
					private int _field3 = 3;
					private int _field4 = 4;

					public static void Main()
					{
						var program = new Program();
						program._field1 = 1;
						program._field2 = 2;
						program._field3 = 3;
						program._field4 = 4;
					}
				}
			}";

            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }
    }
}
