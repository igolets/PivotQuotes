namespace PivotQuotes.Import
{
    /// <summary>
    /// Common constants
    /// </summary>
    internal static class Settings
    {
        /// <summary>
        /// If year is greater than or equal to the value then it is past century
        /// </summary>
        public const int CenturyBorder = 60;

        /// <summary>
        /// Minimal year possible with this logic
        /// </summary>
        public const int MinYear = CenturyBorder + 1900;
    }
}