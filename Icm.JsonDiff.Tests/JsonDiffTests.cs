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
    public class JsonDiffTests
    {
        private class TestClass
        {
            [SuppressMessage("ReSharper", "UnusedMember.Local",
                Justification = "Serialized by JSON library")]
            public string Prop { get; set; }
        }

        [TestMethod]
        public void Diff_PropertyNotInSource()
        {
            TestDifference(
                new {},
                new {prop = 345.44},
                new DifferencePropertyNotInSource(string.Empty, "prop"));
        }

        [TestMethod]
        public void Diff_PropertyNotInDestination()
        {
            TestDifference(
                new {prop = 345.44},
                new {},
                new DifferencePropertyNotInDestination(string.Empty, "prop"));
        }

        [TestMethod]
        public void Diff_InType()
        {
            TestDifference(
                new {prop = new {}},
                new {prop = 345.44},
                new DifferenceInType("prop", "Object", "Float"));
        }

        [TestMethod]
        public void Diff_Value()
        {
            TestDifference(
                new {Prop = 34},
                new {Prop = 345.44},
                new DifferenceValue("Prop", "34", "345.44"));
        }

        [TestMethod]
        public void Diff_ValueWithNull()
        {
            TestDifference(
                new TestClass(),
                new {Prop = 345.44},
                new DifferenceValue("Prop", "null", "345.44"));
        }

        [TestMethod]
        public void Diff_ArrayCount()
        {
            TestDifference(
                new {prop = new object[] {"a", 4}},
                new {prop = new object[] {0}},
                new DifferenceArrayCount("prop", 2, 1),
                new DifferenceValue("prop[0]", "'a'", "0"));
        }

        [TestMethod]
        public void Diff_ValueInArray()
        {
            TestDifference(
                new {prop = new object[] {"a", 4}},
                new {prop = new[] {"a", "b"}},
                new DifferenceValue("prop[1]", "4", "'b'"));
        }

        [TestMethod]
        public void Diff_Multiple()
        {
            TestDifference(
                new
                {
                    prop = new
                    {
                        arr = new object[] {"a", 4},
                        arr2 = new object[] {"a", 4},
                        arr3 = new object[] {"a", 4},
                        arr4 = new object[] { "x", 4, 45 },
                        val = "asdf"
                    },
                    prop1 = 67,
                    prop3 = new { }
                },
                new
                {
                    prop = new
                    {
                        arr = new object[] {"a"},
                        arr2 = new object[] {"a", 5},
                        arr3 = new object[] {"b", 4},
                        arr4 = new object[] { "y" },
                        val = 33
                    },
                    prop2 = 44,
                    prop3 = new object[] { }
                },
                new DifferencePropertyNotInDestination(string.Empty, "prop1"),
                new DifferencePropertyNotInSource(string.Empty, "prop2"),
                new DifferenceArrayCount("prop.arr", 2, 1),
                new DifferenceValue("prop.arr2[1]", "4", "null"),
                new DifferenceValue("prop.arr3[0]", "'a'", "'b'"),
                new DifferenceArrayCount("prop.arr4", 3, 1),
                new DifferenceValue("prop.arr4[0]", "'x'", "'y'"),
                new DifferenceValue("prop.val", "'asdf'", "33"),
                new DifferenceInType("prop3", "Object", "Array"));
        }

        private static void TestDifference(object object1, object object2, params Difference[] expectedDifferences)
        {
            var json1 = JObject.FromObject(object1);
            var json2 = JObject.FromObject(object2);

            IEnumerable<Difference> differences = JsonDiff.Diff(json1, json2).ToList();
            differences.Should().BeEquivalentTo(expectedDifferences.ToList());
        }
    }
}