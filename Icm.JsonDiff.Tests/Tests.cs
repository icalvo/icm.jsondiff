using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using Icm.JsonDiff.Differences;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Icm.JsonDiff.Tests
{
    [TestClass]
    public class Tests
    {
        private class TestClass
        {
            [SuppressMessage("ReSharper", "UnusedMember.Local",
                Justification = "Serialized by JSON library")]
            public string Prop { get; set; }
        }

        [TestMethod]
        public void DiffObject1()
        {
            TestDifference(
                new {}, 
                new { prop = 345.44 },
                (jobj1, jobj2) => new DifferencePropertyNotInSource(jobj1, jobj2, jobj2.Property("prop")));
        }

        [TestMethod]
        public void DiffObject2()
        {
            TestDifference(
                new { prop = 345.44 },
                new { },
                (jobj1, jobj2) => new DifferencePropertyNotInDestination(jobj1, jobj2, jobj1.Property("prop")));
        }

        [TestMethod]
        public void DiffType()
        {
            TestDifference(
                new { prop = new {} },
                new { prop = 345.44 },
                (jobj1, jobj2) => new DifferenceInType(jobj1["prop"], jobj2["prop"]));
        }

        [TestMethod]
        public void DiffValue1()
        {
            TestDifference(
                new { Prop = 34 },
                new { Prop = 345.44 },
                (jobj1, jobj2) => new DifferenceValue(jobj1["Prop"], jobj2["Prop"]));
        }

        [TestMethod]
        public void DiffValue2()
        {
            TestDifference(
                new TestClass(), 
                new { Prop = 345.44 },
                (jobj1, jobj2) => new DifferenceValue(jobj1["Prop"], jobj2["Prop"]));
        }

        [TestMethod]
        public void DiffArray1()
        {
            TestDifference(
                new { prop = new object[] { "a", 4} },
                new { prop = new[] { 0 } },
                (jobj1, jobj2) => new DifferenceArrayCount(jobj1["prop"], jobj2["prop"]));
        }

        [TestMethod]
        public void DiffArray2()
        {
            TestDifference(
                new { prop = new object[] { "a", 4 } },
                new { prop = new[] { "a", "b" } },
                (jobj1, jobj2) => new DifferenceValue(jobj1["prop"][1], jobj2["prop"][1]));
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
                (jobj1, jobj2) => new DifferencePropertyNotInDestination(jobj1, jobj2, jobj1.Property("prop1")),
                (jobj1, jobj2) => new DifferencePropertyNotInSource(jobj1, jobj2, jobj2.Property("prop2")),
                (jobj1, jobj2) => new DifferenceArrayCount(jobj1["prop"]["arr"], jobj2["prop"]["arr"]),
                (jobj1, jobj2) => new DifferenceValue(jobj1["prop"]["arr2"][1], jobj2["prop"]["arr2"][1]),
                (jobj1, jobj2) => new DifferenceValue(jobj1["prop"]["arr3"][0], jobj2["prop"]["arr3"][0]),
                (jobj1, jobj2) => new DifferenceValue(jobj1["prop"]["val"], jobj2["prop"]["val"])
                );
        }

        [TestMethod]
        public void Output()
        {
            TestOutput(
                new
                {
                    prop = new
                    {
                        arr = new object[] { "a", 4 },
                        arr2 = new object[] { "a", 4 },
                        arr3 = new object[] { "a", 4 },
                        val = "asdf"
                    },
                    prop1 = 67,
                    prop3 = new { }
                },
                new
                {
                    prop = new
                    {
                        arr = new object[] { "a" },
                        arr2 = new object[] { "a", null },
                        arr3 = new object[] { "b", 4 },
                        val = 33
                    },
                    prop2 = 44,
                    prop3 = new object[0]
                },
                "In root object, property prop1 is in source but not in destination",
                "In root object, property prop2 is in destination but not in source",
                "In prop.arr, arrays don't have same number of elements",
                "In prop.arr2[1], value 4 != null",
                "In prop.arr3[0], value 'a' != 'b'",
                "In prop.val, value 'asdf' != 33",
                "In prop3, there is a type difference"
                );
        }

        private static void TestDifference(object object1, object object2, params Func<JObject, JObject, Difference>[] expectedDifferences)
        {
            var json1 = JObject.FromObject(object1);
            var json2 = JObject.FromObject(object2);

            IEnumerable<Difference> differences = JsonDiff.Diff(json1, json2).ToList();
            differences.Should().BeEquivalentTo(expectedDifferences.Select(x => x.Invoke(json1, json2)));
        }

        private static void TestOutput(object object1, object object2, params string[] expectedOutput)
        {
            var json1 = JObject.FromObject(object1);
            var json2 = JObject.FromObject(object2);

            IEnumerable<Difference> differences = JsonDiff.Diff(json1, json2).ToList();

            foreach (var difference in differences)
            {
                difference.Token1.Should().NotBeNull();
                difference.Token2.Should().NotBeNull();
            }

            var output = differences.Select(x => x.ToString());
            output.Should().BeEquivalentTo(expectedOutput);
        }
    }
}