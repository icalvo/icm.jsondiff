using FluentAssertions;
using Icm.JsonDiff.Differences;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Icm.JsonDiff.Tests
{
    [TestClass]
    public class DifferenceTests
    {
        [TestMethod]
        public void DifferencePropertyNotInDestination_ToString()
        {
            new DifferencePropertyNotInDestination(string.Empty, "prop1").ToString()
                .Should().Be("In root object, property prop1 is in source but not in destination");
        }

        [TestMethod]
        public void DifferencePropertyNotInSource_ToString()
        {
            new DifferencePropertyNotInSource(string.Empty, "prop2").ToString()
                .Should().Be("In root object, property prop2 is in destination but not in source");
        }

        [TestMethod]
        public void DifferenceArrayCount_ToString()
        {
            new DifferenceArrayCount("prop.arr", 3, 5).ToString()
                .Should().Be("In prop.arr, arrays have 3 elements in source and 5 elements in destination");
        }

        [TestMethod]
        public void DifferenceValue_ToString()
        {
            new DifferenceValue("prop.arr", "rep1", "rep2").ToString()
                .Should().Be("In prop.arr, value rep1 != rep2");
        }

        [TestMethod]
        public void DifferenceInType_ToString()
        {
            new DifferenceInType("prop.arr", "Object", "Float").ToString()
                .Should().Be("In prop.arr, it is an Object in source and a Float in destination");
        }
    }
}