using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = StyleRulesExtensions.Test.CSharpCodeFixVerifier<
    StyleRulesExtensions.PropertyNamingAnalyzer,
    StyleRulesExtensions.PropertyNamingCodeFixProvider>;

namespace StyleRulesExtensions.Test
{
    [TestClass]
    public class PropertyNamingUnitTest
    {
        [TestMethod]
        public async Task PropertyNaming_NoDiagmostic()
        {
            var test = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            using System.Diagnostics;

            namespace ConsoleApplication1
            {
                class typeName
                {
                    public int TestProperty { get; set; }
                    public void test() {}
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task PropertyNaming_FixName()
        {
            var test = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            using System.Diagnostics;

            namespace ConsoleApplication1
            {
                class TypeName
                {
                    public int [|testProperty|] { get; set; }
                }
            }";

            var fixtest = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            using System.Diagnostics;

            namespace ConsoleApplication1
            {
                class TypeName
                {
                    public int TestProperty { get; set; }
                }
            }";

            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }
    }
}
