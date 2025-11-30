using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = StyleRulesExtensions.Test.CSharpAnalyzerVerifier<StyleRulesExtensions.RightNamingAnalyzer>;

namespace StyleRulesExtensions.Test
{
    [TestClass]
    public class RightNamingUnitTest
    {
        [TestMethod]
        public async Task RigthNames_NoDiagmostic()
        {
            var test = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            using System.Diagnostics;

            namespace ConsoleApplication
            {
                class TypeName
                {
                    public TypeName() { }
                    
                    public int TestProperty { get; set; }

                    public void Test(int param)
                    {
                        var local = 1;
                    }
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task BadNamespaceNames_Diagmostic()
        {
            var test = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            using System.Diagnostics;

            namespace [|ConsoleApplicationПлохое|]
            {
                class TypeName
                {
                    public int TestField = 1;
                    public int TestProperty { get; set; }

                    public void Test(int param)
                    {
                        var local = 1;
                    }
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task BadClassNames_Diagmostic()
        {
            var test = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            using System.Diagnostics;

            namespace ConsoleApplication
            {
                class [|TypeNameПлохое|]
                {
                    public int TestField = 1;
                    public int TestProperty { get; set; }

                    public void Test(int param)
                    {
                        var local = 1;
                    }
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task BadFieldNames_Diagmostic()
        {
            var test = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            using System.Diagnostics;

            namespace ConsoleApplication
            {
                class TypeName
                {
                    public int [|TestFieldПлохое|] = 1;
                    public int TestProperty { get; set; }

                    public void Test(int param)
                    {
                        var local = 1;
                    }
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task BadPropertyNames_Diagmostic()
        {
            var test = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            using System.Diagnostics;

            namespace ConsoleApplication
            {
                class TypeName
                {
                    public int TestField = 1;
                    public int [|TestPropertyПлохое|] { [|get|]; [|set|]; }

                    public void Test(int param)
                    {
                        var local = 1;
                    }
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task BadMethodNames_Diagmostic()
        {
            var test = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            using System.Diagnostics;

            namespace ConsoleApplication
            {
                class TypeName
                {
                    public int TestProperty { get; set; }

                    public void [|TestПлохое|](int param)
                    {
                        var local = 1;
                    }
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task BadLocalVariableNames_Diagmostic()
        {
            var test = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            using System.Diagnostics;

            namespace ConsoleApplication
            {
                class TypeName
                {
                    public int TestProperty { get; set; }

                    public void Test(int param)
                    {
                        var [|localПлохое|] = 1;
                    }
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task BadParameterVariableNames_Diagmostic()
        {
            var test = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Text;
            using System.Threading.Tasks;
            using System.Diagnostics;

            namespace ConsoleApplication
            {
                class TypeName
                {
                    public int TestProperty { get; set; }

                    public void Test(int [|paramПлохое|])
                    {
                        var local = 1;
                    }
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
