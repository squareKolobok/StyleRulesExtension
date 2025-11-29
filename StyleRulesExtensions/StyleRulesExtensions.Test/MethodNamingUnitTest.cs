using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = StyleRulesExtensions.Test.CSharpCodeFixVerifier<
    StyleRulesExtensions.MethodNamingAnalyzer,
    StyleRulesExtensions.MethodNamingCodeFixProvider>;

namespace StyleRulesExtensions.Test
{
    [TestClass]
    public class MethodNamingUnitTest
    {
        [TestMethod]
        public async Task MethodNaiming_NoDiagmostic()
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
                class myType
                {
                    public void Test() {}
                    public void @Test2() {}
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task MethodNaimingOfConstructorAndProperties_NoDiagmostic()
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
                class myType
                {
                    public myType()
                    {}

                    public int MyProperty { get; set; }
                    public void Test() {}
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task MethodNaiming_FixName()
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
                class MyType
                {
                    public MyType()
                    {}
                    
                    public int MyProperty { get; set; }
                    public void ValidMethod() {}
                    
                    public void [|test|]() {}

                    public int [|getInt|]()
                    {
                        return 0;
                    }
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
                class MyType
                {
                    public MyType()
                    {}
                    
                    public int MyProperty { get; set; }
                    public void ValidMethod() {}
                    
                    public void Test() {}

                    public int GetInt()
                    {
                        return 0;
                    }
                }
            }";

            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }

        [TestMethod]
        public async Task MethodNaimingWithUndersore_FixName()
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
                class MyType
                {
                    public void [|test_method|]() {}
                    public void [|@test_method2|]() {}

                    public int [|get_int|]()
                    {
                        return 0;
                    }
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
                class MyType
                {
                    public void TestMethod() {}
                    public void TestMethod2() {}

                    public int GetInt()
                    {
                        return 0;
                    }
                }
            }";

            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }
    }
}
