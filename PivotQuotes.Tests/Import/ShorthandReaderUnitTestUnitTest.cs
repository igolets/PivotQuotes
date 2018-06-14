using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PivotQuotes.Import;
using Shouldly;

namespace PivotQuotes.Tests.Import
{
    [TestClass]
    public class ShorthandReaderUnitTestUnitTest
    {
        [TestMethod]
        public void ShorthandWrapperUnitTest_CorrectQ1()
        {
            var test = ShorthandReader.Read("Q1_14");
            test.From.ShouldBe(new DateTime(2014, 01, 01));
            test.To.ShouldBe(new DateTime(2014, 03, 31));
        }

        [TestMethod]
        public void ShorthandWrapperUnitTest_CorrectQ2()
        {
            var test = ShorthandReader.Read("Q2_12");
            test.From.ShouldBe(new DateTime(2012, 04, 01));
            test.To.ShouldBe(new DateTime(2012, 06, 30));
        }

        [TestMethod]
        public void ShorthandWrapperUnitTest_CorrectQ3()
        {
            var test = ShorthandReader.Read("Q3_10");
            test.From.ShouldBe(new DateTime(2010, 07, 01));
            test.To.ShouldBe(new DateTime(2010, 09, 30));
        }

        [TestMethod]
        public void ShorthandWrapperUnitTest_CorrectQ4()
        {
            var test = ShorthandReader.Read("Q4_11");
            test.From.ShouldBe(new DateTime(2011, 10, 01));
            test.To.ShouldBe(new DateTime(2011, 12, 31));
        }

        [TestMethod]
        public void ShorthandWrapperUnitTest_IsNullOrWhiteSpace()
        {
            Should.Throw<Exception>(()=>ShorthandReader.Read(null));
        }

        [TestMethod]
        public void ShorthandWrapperUnitTest_Split()
        {
            Should.Throw<Exception>(()=>ShorthandReader.Read("Q"));
            Should.Throw<Exception>(()=>ShorthandReader.Read("Q209"));
        }

        [TestMethod]
        public void ShorthandWrapperUnitTest_NoYear()
        {
            Should.Throw<Exception>(()=>ShorthandReader.Read("Q2_U9"));
        }

        [TestMethod]
        public void ShorthandWrapperUnitTest_WrongYear()
        {
            Should.Throw<Exception>(()=>ShorthandReader.Read("Q3_100"));
            Should.Throw<Exception>(()=>ShorthandReader.Read("Q3_-10"));
        }

        [TestMethod]
        public void ShorthandWrapperUnitTest_CenturyBorder2000()
        {
            var test = ShorthandReader.Read("Q4_59");
            test.From.ShouldBe(new DateTime(2059, 10, 01));
            test.To.ShouldBe(new DateTime(2059, 12, 31));
        }

        [TestMethod]
        public void ShorthandWrapperUnitTest_CenturyBorder1900()
        {
            var test = ShorthandReader.Read("Q4_60");
            test.From.ShouldBe(new DateTime(1960, 10, 01));
            test.To.ShouldBe(new DateTime(1960, 12, 31));
        }

        [TestMethod]
        public void ShorthandWrapperUnitTest_InCorrectQuarter()
        {
            Should.Throw<Exception>(()=>ShorthandReader.Read("Q0_10"));
            Should.Throw<Exception>(()=>ShorthandReader.Read("Q5_10"));
        }
    }
}
