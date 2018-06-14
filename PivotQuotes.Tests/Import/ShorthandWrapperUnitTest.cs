using Microsoft.VisualStudio.TestTools.UnitTesting;
using PivotQuotes.Import;
using Shouldly;

namespace PivotQuotes.Tests.Import
{
    [TestClass]
    public class ShorthandWrapperUnitTest
    {
        [TestMethod]
        public void ShorthandWrapper_Correct()
        {
            var test = new ShorthandWrapper("Q2_09");
            test.SrcValue.ShouldBe("Q2_09");
            test.Quarter.ShouldBe("Q2");
            test.Year.ShouldBe(2009);
        }

        [TestMethod]
        public void ShorthandWrapper_IsNullOrWhiteSpace()
        {
            var test = new ShorthandWrapper("");
            test.SrcValue.ShouldBe("");
            test.Quarter.ShouldBe("");
            test.Year.ShouldBe(-1);
        }

        [TestMethod]
        public void ShorthandWrapper_Split()
        {
            var test = new ShorthandWrapper("Q209");
            test.SrcValue.ShouldBe("Q209");
            test.Quarter.ShouldBe("");
            test.Year.ShouldBe(-1);
        }
        
        [TestMethod]
        public void ShorthandWrapper_NoYear()
        {
            var test = new ShorthandWrapper("Q2_O9");
            test.SrcValue.ShouldBe("Q2_O9");
            test.Quarter.ShouldBe("");
            test.Year.ShouldBe(-1);
        }
        
        [TestMethod]
        public void ShorthandWrapper_WrongYear()
        {
            var test = new ShorthandWrapper("Q2_109");
            test.SrcValue.ShouldBe("Q2_109");
            test.Quarter.ShouldBe("");
            test.Year.ShouldBe(-1);

            test = new ShorthandWrapper("Q2_-9");
            test.SrcValue.ShouldBe("Q2_-9");
            test.Quarter.ShouldBe("");
            test.Year.ShouldBe(-1);
        }
        
        [TestMethod]
        public void ShorthandWrapper_CenturyBorder()
        {
            var test = new ShorthandWrapper("Q1_59");
            test.SrcValue.ShouldBe("Q1_59");
            test.Quarter.ShouldBe("Q1");
            test.Year.ShouldBe(2059);
            test = new ShorthandWrapper("Q1_60");
            test.SrcValue.ShouldBe("Q1_60");
            test.Quarter.ShouldBe("Q1");
            test.Year.ShouldBe(1960);
        }
    }
}
