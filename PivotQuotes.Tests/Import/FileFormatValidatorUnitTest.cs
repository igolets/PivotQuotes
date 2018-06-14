using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PivotQuotes.Import;
using Shouldly;

namespace PivotQuotes.Tests.Import
{
    [TestClass]
    public class FileFormatValidatorUnitTest
    {
        [TestMethod]
        public void FileFormatValidator_Correct()
        {
            var src = new[]
            {
                new FileFormatModel
                {
                    ObservationDate = new DateTime(2009,01,02),
                    Shorthand = "Q2_09",
                    From = new DateTime(2009,04,01),
                    To = new DateTime(2009,06,30),
                    Price = 0.46925m
                }
            };

            var test = new FileFormatValidator(src);
            var rez = test.ValidateAndFix();

            test.ValidationWarnings.ShouldBeNull();
            test.ValidationErrors.ShouldBeNull();
            rez.Length.ShouldBe(1);
            rez[0].ObservationDate.ShouldBe(src[0].ObservationDate.GetValueOrDefault());
            rez[0].Shorthand.ShouldBe(src[0].Shorthand);
            rez[0].From.ShouldBe(src[0].From.GetValueOrDefault());
            rez[0].To.ShouldBe(src[0].To.GetValueOrDefault());
            rez[0].Price.ShouldBe(src[0].Price.GetValueOrDefault());
        }

        [TestMethod]
        public void FileFormatValidator_ErrNoShorthand()
        {
            var src = new[]
            {
                new FileFormatModel
                {
                    ObservationDate = new DateTime(2009,02,25),
                    Shorthand = null,
                    From = new DateTime(2012,10,01),
                    To = new DateTime(2012,10,31),
                    Price = 0.606m
                }
            };

            var test = new FileFormatValidator(src);
            var rez = test.ValidateAndFix();

            rez.Length.ShouldBe(0);
            test.ValidationWarnings.ShouldBeNull();
            test.ValidationErrors.Length.ShouldBe(1);
        }

        [TestMethod]
        public void FileFormatValidator_ErrWrongShorthand()
        {
            var src = new[]
            {
                new FileFormatModel
                {
                    ObservationDate = new DateTime(2009,02,25),
                    Shorthand = null,
                    From = new DateTime(2014,04,01),
                    To = new DateTime(2014,06,30),
                    Price = 0.578m
                }
            };

            var test = new FileFormatValidator(src);
            var rez = test.ValidateAndFix();

            rez.Length.ShouldBe(0);
            test.ValidationWarnings.ShouldBeNull();
            test.ValidationErrors.Length.ShouldBe(1);
        }

        [TestMethod]
        public void FileFormatValidator_ErrNoObservationDate()
        {
            var src = new[]
            {
                new FileFormatModel
                {
                    ObservationDate = null,
                    Shorthand = "Q1_13",
                    From = new DateTime(2013,01,01),
                    To = new DateTime(2013,03,31),
                    Price = 0.685m
                }
            };

            var test = new FileFormatValidator(src);
            var rez = test.ValidateAndFix();

            rez.Length.ShouldBe(0);
            test.ValidationWarnings.ShouldBeNull();
            test.ValidationErrors.Length.ShouldBe(1);
        }

        [TestMethod]
        public void FileFormatValidator_ErrNoPrice()
        {
            var src = new[]
            {
                new FileFormatModel
                {
                    ObservationDate = new DateTime(2010,01,05),
                    Shorthand = "Q1_13",
                    From = new DateTime(2013,01,01),
                    To = new DateTime(2013,03,31),
                    Price = 0m
                }
            };

            var test = new FileFormatValidator(src);
            var rez = test.ValidateAndFix();

            rez.Length.ShouldBe(0);
            test.ValidationWarnings.ShouldBeNull();
            test.ValidationErrors.Length.ShouldBe(1);
        }

        [TestMethod]
        public void FileFormatValidator_WarnSuspiciousObservationDate()
        {
            var src = new[]
            {
                new FileFormatModel
                {
                    ObservationDate = new DateTime(1900,01,14),
                    Shorthand = "Q3_14",
                    From = new DateTime(2014,07,01),
                    To = new DateTime(2014,09,30),
                    Price = 0.575m
                }
            };

            var test = new FileFormatValidator(src);
            var rez = test.ValidateAndFix();

            test.ValidationWarnings.Length.ShouldBe(1);
            test.ValidationErrors.ShouldBeNull();
            rez.Length.ShouldBe(1);
            rez[0].ObservationDate.ShouldBe(src[0].ObservationDate.GetValueOrDefault());
            rez[0].Shorthand.ShouldBe(src[0].Shorthand);
            rez[0].From.ShouldBe(src[0].From.GetValueOrDefault());
            rez[0].To.ShouldBe(src[0].To.GetValueOrDefault());
            rez[0].Price.ShouldBe(src[0].Price.GetValueOrDefault());
        }

        [TestMethod]
        public void FileFormatValidator_WarnFixFrom()
        {
            var src = new[]
            {
                new FileFormatModel
                {
                    ObservationDate = new DateTime(2009,12,01),
                    Shorthand = "Q4_13",
                    From = null,
                    To = new DateTime(2013,12,31),
                    Price = 0.5645m
                }
            };

            var test = new FileFormatValidator(src);
            var rez = test.ValidateAndFix();

            test.ValidationWarnings.Length.ShouldBe(1);
            test.ValidationErrors.ShouldBeNull();
            rez.Length.ShouldBe(1);
            rez[0].ObservationDate.ShouldBe(src[0].ObservationDate.GetValueOrDefault());
            rez[0].Shorthand.ShouldBe(src[0].Shorthand);
            rez[0].From.ShouldBe(new DateTime(2013,10,01));
            rez[0].To.ShouldBe(src[0].To.GetValueOrDefault());
            rez[0].Price.ShouldBe(src[0].Price.GetValueOrDefault());
        }

        [TestMethod]
        public void FileFormatValidator_WarnFixTo()
        {
            var src = new[]
            {
                new FileFormatModel
                {
                    ObservationDate = new DateTime(2010,01,05),
                    Shorthand = "Q3_13",
                    From = new DateTime(2013,07,01),
                    To = null,
                    Price = 0.575m
                }
            };

            var test = new FileFormatValidator(src);
            var rez = test.ValidateAndFix();

            test.ValidationWarnings.Length.ShouldBe(1);
            test.ValidationErrors.ShouldBeNull();
            rez.Length.ShouldBe(1);
            rez[0].ObservationDate.ShouldBe(src[0].ObservationDate.GetValueOrDefault());
            rez[0].Shorthand.ShouldBe(src[0].Shorthand);
            rez[0].From.ShouldBe(src[0].From.GetValueOrDefault());
            rez[0].To.ShouldBe(new DateTime(2013,09,30));
            rez[0].Price.ShouldBe(src[0].Price.GetValueOrDefault());
        }

        [TestMethod]
        public void FileFormatValidator_WarnFixFromAndTo()
        {
            var src = new[]
            {
                new FileFormatModel
                {
                    ObservationDate = new DateTime(2010,01,05),
                    Shorthand = "Q3_13",
                    From = null,
                    To = null,
                    Price = 0.575m
                }
            };

            var test = new FileFormatValidator(src);
            var rez = test.ValidateAndFix();

            test.ValidationWarnings.Length.ShouldBe(2);
            test.ValidationErrors.ShouldBeNull();
            rez.Length.ShouldBe(1);
            rez[0].ObservationDate.ShouldBe(src[0].ObservationDate.GetValueOrDefault());
            rez[0].Shorthand.ShouldBe(src[0].Shorthand);
            rez[0].From.ShouldBe(new DateTime(2013,07,01));
            rez[0].To.ShouldBe(new DateTime(2013,09,30));
            rez[0].Price.ShouldBe(src[0].Price.GetValueOrDefault());
        }

        [TestMethod]
        public void FileFormatValidator_ErrorIsPriority()
        {
            var src = new[]
            {
                new FileFormatModel
                {
                    ObservationDate = new DateTime(2010,01,05),
                    Shorthand = "Q3_13",
                    From = null,
                    To = null,
                    Price = 0m
                }
            };

            var test = new FileFormatValidator(src);
            var rez = test.ValidateAndFix();

            test.ValidationWarnings.ShouldBeNull();
            test.ValidationErrors.Length.ShouldBe(1);
            rez.Length.ShouldBe(0);
        }

        [TestMethod]
        public void FileFormatValidator_WarnShorthandConflictsFrom()
        {
            var src = new[]
            {
                new FileFormatModel
                {
                    ObservationDate = new DateTime(2010,01,05),
                    Shorthand = "Q2_15",
                    From = new DateTime(2015,03,01),
                    To = new DateTime(2015,06,30),
                    Price = 0.598m
                }
            };

            var test = new FileFormatValidator(src);
            var rez = test.ValidateAndFix();

            test.ValidationWarnings.Length.ShouldBe(1);
            test.ValidationErrors.ShouldBeNull();
            rez.Length.ShouldBe(1);
            rez[0].ObservationDate.ShouldBe(src[0].ObservationDate.GetValueOrDefault());
            rez[0].Shorthand.ShouldBe(src[0].Shorthand);
            rez[0].From.ShouldBe(src[0].From.GetValueOrDefault());
            rez[0].To.ShouldBe(src[0].To.GetValueOrDefault());
            rez[0].Price.ShouldBe(src[0].Price.GetValueOrDefault());
        }

        [TestMethod]
        public void FileFormatValidator_WarnShorthandConflictsTo()
        {
            var src = new[]
            {
                new FileFormatModel
                {
                    ObservationDate = new DateTime(2010,01,05),
                    Shorthand = "Q2_15",
                    From = new DateTime(2015,04,01),
                    To = new DateTime(2015,06,29),
                    Price = 0.598m
                }
            };

            var test = new FileFormatValidator(src);
            var rez = test.ValidateAndFix();

            test.ValidationWarnings.Length.ShouldBe(1);
            test.ValidationErrors.ShouldBeNull();
            rez.Length.ShouldBe(1);
            rez[0].ObservationDate.ShouldBe(src[0].ObservationDate.GetValueOrDefault());
            rez[0].Shorthand.ShouldBe(src[0].Shorthand);
            rez[0].From.ShouldBe(src[0].From.GetValueOrDefault());
            rez[0].To.ShouldBe(src[0].To.GetValueOrDefault());
            rez[0].Price.ShouldBe(src[0].Price.GetValueOrDefault());
        }
    }
}
