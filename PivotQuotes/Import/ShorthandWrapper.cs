namespace PivotQuotes.Import
{
    public class ShorthandWrapper
    {
        public ShorthandWrapper(string shorthand)
        {
            Quarter = "";
            Year = -1;
            SrcValue = shorthand;

            if (string.IsNullOrWhiteSpace(shorthand))
            {
                return;
            }

            var parts = shorthand.Split('_');
            if (parts.Length != 2)
            {
                return;
            }

            if (!int.TryParse(parts[1], out var year))
            {
                return;
            }

            if (year > 99 || year < 0)
            {
                return;
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

            Quarter = parts[0];
            Year = year;
        }

        public string SrcValue
        {
            get;
            set;
        }

        public int Year
        {
            get;
            set;
        }

        public string Quarter
        {
            get;
            set;
        }
    }
}