using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = StyleRulesExtensions.Test.CSharpCodeFixVerifier<
    StyleRulesExtensions.SpaceChainAnalyzer, 
    StyleRulesExtensions.SpaceChainСodeFixProvider>;

namespace StyleRulesExtensions.Test
{
    [TestClass]
    public class SpaceChainUnitTest
    {
        [TestMethod]
        public async Task NoSpaceChain_NoDiagnostic()
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

                    public void Test()
                    {
                        var local = 1;
                        var list = new List<int>();
                        list.Where(x => x > 1).Select(x => new { a = x, }).ToList();

            			var list2 = list.Where(x => x > 1).ToList();
                        var sum = 0 + 1 - 5 * 7;
                        var logical = sum == 1 || list.Count == 0 || list.Count == 1;
                    }
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task SpaceChainList_NoDiagnostic()
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

                    public void Test()
                    {
                        var local = 1;
                        var list = new List<int>();

                        list
                            .Where(x => x < 1)
                            .Select(x => new 
                            { 
                                a = x, 
                            })
                            .ToList();

                        var list2 = list
                            .Where(x => x > 1)
                            .Select(x => new 
                            { 
                                a = x, 
                            })
                            .ToList();

                        var sum = 0 + 1 - 
                            5 * 7;
                        var logical = sum == 1 || 
                            list.Count == 0 || 
                            list.Count == 1;

                        if (sum == 1 || list.Count == 0 || list.Count == 1) {}

                        if (sum == 1 || 
                            list.Count == 0 ||
                            list.Count == 1) 
                        {}
                    }
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task SpaceChainList_Diagnostic()
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

                    public void Test()
                    {
                        var list = new List<int>();

                        [|list
                             .Where(x => x < 1)
                            .Select(x => new 
                            { 
                                a = x, 
                            })
                            .ToList()|];

                        var list2 = [|list
                           .Where(x => x > 1)
                            .Select(x => new 
                            { 
                                a = x, 
                            })
                            .ToList()|];

                        var sum = [|0 + 1 -
                          5 * 7|];

                        var logical = [|sum == 1 || 
                          list.Count == 0 || 
                            list.Count == 1|];
                        
                        if ([|sum == 1 || 
                             list.Count == 0 ||
                            list.Count == 1|])
                        {}
                    }
                }
            }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task SpaceChainList_FixExpression()
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

                    public void Test()
                    {
                        var list = new List<int>();

                        var list2 = [|list
                           .Where(x => x > 1)
                            .Select(x => new
                             {
                              a = x,
                            })
                            .ToList()|];
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

            namespace ConsoleApplication
            {

                class TypeName
                {

                    public void Test()
                    {
                        var list = new List<int>();

                        var list2 = list
                            .Where(x => x > 1)
                            .Select(x => new
                            {
                                a = x,
                            })
                            .ToList();
                    }
                }
            }";

            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }

        [TestMethod]
        public async Task SpaceChainList_FixListDeclaration()
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

                    public void Test()
                    {
                        var list = new List<int>();

                        [|list
                             .Where(x => x < 1)
                            .Select(x => new 
                           { 
                                 a = x, 
                            })
                            .ToList()|];
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

            namespace ConsoleApplication
            {

                class TypeName
                {

                    public void Test()
                    {
                        var list = new List<int>();

                        list
                            .Where(x => x < 1)
                            .Select(x => new 
                            { 
                                a = x, 
                            })
                            .ToList();
                    }
                }
            }";

            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }

        [TestMethod]
        public async Task SpaceChainList_FixSimpleDeclaration()
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

                    public void Test()
                    {
                        var list = new List<int>();

                        var sum = [|0 + 1 -
                          5 * 7|];

                        var logical = [|sum == 1 || 
                          list.Count == 0 || 
                            list.Count == 1|];
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

            namespace ConsoleApplication
            {

                class TypeName
                {

                    public void Test()
                    {
                        var list = new List<int>();

                        var sum = 0 + 1 -
                            5 * 7;

                        var logical = sum == 1 || 
                            list.Count == 0 || 
                            list.Count == 1;
                    }
                }
            }";

            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }

        [TestMethod]
        public async Task SpaceChainList_FixIfCondition()
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

                    public void Test()
                    {
                        var list = new List<int>();
                        var sum = 12;

                        if ([|sum == 1 || 
                             list.Count == 0 ||
                            list.Count == 1|])
                        {}
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

            namespace ConsoleApplication
            {

                class TypeName
                {

                    public void Test()
                    {
                        var list = new List<int>();
                        var sum = 12;

                        if (sum == 1 || 
                            list.Count == 0 ||
                            list.Count == 1)
                        {}
                    }
                }
            }";

            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }

        [TestMethod]
        public async Task SpaceChainList_FixObjectDeclarations()
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

                class A
                {
                    public A A1 { get; set; }
                    public int Size { get; set; }
                    public A A2 { get; set; }
                }

                class TypeName
                {
                    List<int> GetMethod()
                    {
                        var list = new List<int>();

                        var newList1 = [|list
                           .Select(x => new A()
                                {
                                    Size = 1,
                                    A1 = new A()
                                    {
                                   A1 = new A()
                                        {
                                            Size = x,
                                A2 = new A(),
                                        }
                                    },
                                    A2 = new A()
                                    {
                                           Size = x,
                                        A2 = new A()
                                        {
                                    A1 = new A(),
                                            A2 = new A()
                                  }
                                 },
                                })
                               .ToList()|];

                        var newList2 = [|list.Select(x => new A()
                        {
                                         Size = 1,
                             A1 = new A()
                                                {
                               A1 = new A()
                                 {
                                     Size = x,
                                 A2 = new A(),
                                 }
                              },
                            A2 = new A()
                          {
                              Size = x,
                              A2 = new A()
                              {
        A1 = new A(),
                                  A2 = new A()
                              }
                          },
                        })
                                .ToList()|];

                        return list;
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

            namespace ConsoleApplication
            {

                class A
                {
                    public A A1 { get; set; }
                    public int Size { get; set; }
                    public A A2 { get; set; }
                }

                class TypeName
                {
                    List<int> GetMethod()
                    {
                        var list = new List<int>();

                        var newList1 = list
                            .Select(x => new A()
                            {
                                Size = 1,
                                A1 = new A()
                                {
                                    A1 = new A()
                                    {
                                        Size = x,
                                        A2 = new A(),
                                    }
                                },
                                A2 = new A()
                                {
                                    Size = x,
                                    A2 = new A()
                                    {
                                        A1 = new A(),
                                        A2 = new A()
                                    }
                                },
                            })
                           .ToList();

                        var newList2 = list.Select(x => new A()
                        {
                            Size = 1,
                            A1 = new A()
                            {
                                A1 = new A()
                                {
                                    Size = x,
                                    A2 = new A(),
                                }
                            },
                            A2 = new A()
                            {
                                Size = x,
                                A2 = new A()
                                {
                                    A1 = new A(),
                                    A2 = new A()
                                }
                            },
                        })
                        .ToList();

                        return list;
                    }
                }
            }";

            //await VerifyCS.VerifyAnalyzerAsync(test);
            await VerifyCS.VerifyCodeFixAsync(test, fixtest);
        }
    }
}
