using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = StyleRulesExtensions.Test.CSharpCodeFixVerifier<
    StyleRulesExtensions.ClassNamingAnalyzer,
    StyleRulesExtensions.ClassNamingCodeFixProvider>;

namespace StyleRulesExtensions.Test
{
    [TestClass]
    public class ClassNamingUnitTest
    {
        [TestMethod]
        public async Task ClassNaiming_NoDiagmostic()
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
                    public void Test() {}
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task ClassNaiming_FixName()
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
                class [|typeName|]
                {   
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
                }
            }";

            //var expected = VerifyCS.Diagnostic("StyleRulesExtensions").WithLocation(0).WithArguments("TypeName");
            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }
    }
}
