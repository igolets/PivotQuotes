﻿using System;
using FileHelpers;

namespace PivotQuotes.Import
{
    /// <summary>
    /// data model class for use with FileHelpers
    /// </summary>
    [IgnoreFirst, DelimitedRecord(",")]
    public class FileFormatModel:ICloneable
    {
        [FieldHidden]
        public int LineNumber;

        [FieldConverter(ConverterKind.Date, "dd/MM/yyyy")]
        public DateTime? ObservationDate;

        public string Shorthand;

        [FieldConverter(ConverterKind.Date, "dd/MM/yyyy" )]
        public DateTime? From;

        [FieldConverter(ConverterKind.Date, "dd/MM/yyyy" )]
        public DateTime? To;

        [FieldConverter(ConverterKind.Decimal)]
        [FieldNullValue(typeof(decimal), "0")]
        public decimal? Price;

        #region Implementation of ICloneable

        public object Clone()
        {
            var copy = new FileFormatModel
            {
                LineNumber = LineNumber,
                ObservationDate = ObservationDate,
                Shorthand = Shorthand,
                From = From,
                To = To,
                Price = Price
            };

            return copy;
        }

        #endregion
    }
}
