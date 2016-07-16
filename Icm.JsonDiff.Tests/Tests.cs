using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Icm.JsonDiff.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void DiffObject1()
        {
            TestDifference(
                new {}, 
                new { prop = 345.44 },
                (jobj1, jobj2) => new Difference("Property prop in destination but not in source", jobj1, jobj2, "ROOT"));
        }

        [TestMethod]
        public void DiffObject2()
        {
            TestDifference(
                new { prop = 345.44 },
                new { },
                (jobj1, jobj2) => new Difference("Property prop in source but not in destination", jobj1, jobj2, "ROOT"));
        }

        [TestMethod]
        public void DiffValue()
        {
            TestDifference(
                new { prop = 34 },
                new { prop = 345.44 },
                (jobj1, jobj2) => new Difference("Value 34 != 345.44", jobj1["prop"], jobj2["prop"], "ROOT/prop"));
        }

        [TestMethod]
        public void DiffArray1()
        {
            TestDifference(
                new { prop = new object[] { "a", 4} },
                new { prop = new[] { 0 } },
                (jobj1, jobj2) => new Difference("Arrays don't have same number of elements", jobj1["prop"], jobj2["prop"], "ROOT/prop"));
        }

        [TestMethod]
        public void DiffArray2()
        {
            TestDifference(
                new { prop = new object[] { "a", 4 } },
                new { prop = new[] { "a", "b" } },
                (jobj1, jobj2) => new Difference("Value 4 != b", jobj1["prop"][1], jobj2["prop"][1], "ROOT/prop[1]"));
        }

        [TestMethod]
        public void DiffMultiple1()
        {
            TestDifference(
                new
                {
                    prop = new
                    {
                        arr = new object[] { "a", 4 },
                        arr2 = new object[] { "a", 4 },
                        arr3 = new object[] { "a", 4 },
                        val = "asdf"
                    },
                    prop1 = 67
                },
                new
                {
                    prop = new
                    {
                        arr = new object[] { "a" },
                        arr2 = new object[] { "a", 5 },
                        arr3 = new object[] { "b", 4 },
                        val = 33
                    },
                    prop2 = 44
                },
                (jobj1, jobj2) => new Difference("Property prop1 in source but not in destination", jobj1, jobj2, "ROOT"),
                (jobj1, jobj2) => new Difference("Property prop2 in destination but not in source", jobj1, jobj2, "ROOT"),
                (jobj1, jobj2) => new Difference("Arrays don't have same number of elements", jobj1["prop"]["arr"], jobj2["prop"]["arr"], "ROOT/prop/arr"),
                (jobj1, jobj2) => new Difference("Value 4 != 5", jobj1["prop"]["arr2"][1], jobj2["prop"]["arr2"][1], "ROOT/prop/arr2[1]"),
                (jobj1, jobj2) => new Difference("Value a != b", jobj1["prop"]["arr3"][0], jobj2["prop"]["arr3"][0], "ROOT/prop/arr3[0]"),
                (jobj1, jobj2) => new Difference("Value asdf != 33", jobj1["prop"]["val"], jobj2["prop"]["val"], "ROOT/prop/val")
                );
        }

        private void TestDifference(object object1, object object2, params Func<JObject, JObject, Difference>[] expectedDifferences)
        {
            var json1 = JObject.FromObject(object1);
            var json2 = JObject.FromObject(object2);

            IEnumerable<Difference> differences = JsonDiff.Diff(json1, json2, "ROOT").ToList();
            differences.Should().BeEquivalentTo(expectedDifferences.Select(x => x.Invoke(json1, json2)).ToArray());
        }
    }
}


//Test Name:	TestMethod2
//Test FullName:	Icm.JsonDiff.Tests.Tests.TestMethod2
//Test Source:	C:\Repos\icm.jsondiff\Icm.JsonDiff.Tests\Tests.cs : line 24
//Test Outcome:	Failed
//Test Duration:	0:00:00,3003476

//Result StackTrace:	
//en FluentAssertions.Execution.LateBoundTestFramework.Throw(String message) en C:\projects\fluentassertions-vf06b\Src\Shared\Execution\LateBoundTestFramework.cs:línea 31
//   en FluentAssertions.Execution.TestFrameworkProvider.Throw(String message) en C:\projects\fluentassertions-vf06b\Src\FluentAssertions.Net40\Execution\TestFrameworkProvider.cs:línea 42
//   en FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message) en C:\projects\fluentassertions-vf06b\Src\Core\Execution\DefaultAssertionStrategy.cs:línea 25
//   en FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args) en C:\projects\fluentassertions-vf06b\Src\Core\Execution\AssertionScope.cs:línea 197
//   en FluentAssertions.Collections.CollectionAssertions`2.BeEquivalentTo[T](IEnumerable`1 expected, String because, Object[] becauseArgs) en C:\projects\fluentassertions-vf06b\Src\Core\Collections\CollectionAssertions.cs:línea 335
//   en FluentAssertions.Collections.CollectionAssertions`2.BeEquivalentTo(Object[] elements) en C:\projects\fluentassertions-vf06b\Src\Core\Collections\CollectionAssertions.cs:línea 284
//   en Icm.JsonDiff.Tests.Tests.TestDifference(Object object1, Object object2, Func`3[] expectedDifferences) en C:\Repos\icm.jsondiff\Icm.JsonDiff.Tests\Tests.cs:línea 38
//   en Icm.JsonDiff.Tests.Tests.TestMethod2() en C:\Repos\icm.jsondiff\Icm.JsonDiff.Tests\Tests.cs:línea 25
//Result Message:	
//Expected collection {'Value 34 != 345.44' 34 345,44 'ROOT/prop'}
//to be equivalent to
//{'Property prop in destination but not in source' {
//        "prop": 34
//} {
//        "prop": 345.44
//} 'ROOT'}, but it misses {'Property prop in destination but not in source' {
//  "prop": 34
//} {
//  "prop": 345.44
//} 'ROOT'}.

