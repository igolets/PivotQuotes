using System;

namespace PivotQuotes.Import
{
    /// <summary>
    /// Class can read "Shorthand" field and calculate quarter date borders
    /// </summary>
    public class ShorthandReader
    {
        /// <summary>
        /// Read Shorthand and parse From and To dates from it
        /// </summary>
        /// <param name="shorthand"></param>
        /// <remarks>
        /// Method was made intentionally — code throws exception,
        /// but throwing exception from constructor is a bad pattern.
        ///
        /// Made it static as I don't see a reason to mock such simple code
        /// </remarks>
        public static ShorthandReader Read(string shorthand)
        {
            // validation

            if (string.IsNullOrWhiteSpace(shorthand))
            {
                throw new ArgumentException("Can not parse empty value", nameof(shorthand));
            }

            var parts = shorthand.Split('_');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid format, please use 'Q#_##'", nameof(shorthand));
            }

            if (!int.TryParse(parts[1], out var year))
            {
                throw new ArgumentException("Incorrect 'year' part", nameof(shorthand));
            }

            if (year > 99 || year < 0)
            {
                throw new ArgumentException("Incorrect 'year' value", nameof(shorthand));
            }

            // calculate year
            if (year >= Settings.CenturyBorder)
            {
                year += 1900;
            }
            else
            {
                year += 2000;
            }

            // parse quarter
            switch (parts[0].ToUpper())
            {
                case "Q1":
                    return new ShorthandReader
                    {
                        From = new DateTime(year, 01, 01),
                        To = new DateTime(year, 03, 31)
                    };
                case "Q2":
                    return new ShorthandReader
                    {
                        From = new DateTime(year, 04, 01),
                        To = new DateTime(year, 06, 30)
                    };
                case "Q3":
                    return new ShorthandReader
                    {
                        From = new DateTime(year, 07, 01),
                        To = new DateTime(year, 09, 30)
                    };
                case "Q4":
                    return new ShorthandReader
                    {
                        From = new DateTime(year, 10, 01),
                        To = new DateTime(year, 12, 31)
                    };
                default:
                    throw new ArgumentException("Incorrect 'quarter' part", nameof(shorthand));
            }
        }

        public DateTime From
        {
            get;
            set;
        }

        public DateTime To
        {
            get;
            set;
        }
    }
}