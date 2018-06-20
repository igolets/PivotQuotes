using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PivotQuotes.Import
{
    /// <summary>
    /// Class is responsible for validating and fixing source data if possible
    /// </summary>
    public class FileFormatValidator
    {
        private readonly FileFormatModel[] _input;

        public FileFormatValidator(FileFormatModel[] input)
        {
            _input = input;
        }

        /// <summary>
        /// List of validation errors. 'Error' means non recoverable data. No errors if null.
        /// </summary>
        public string[] ValidationErrors
        {
            get;
            set;
        }

        /// <summary>
        /// List of validation warnings. 'Warning' means some data were recovered. No warnings if null.
        /// </summary>
        public string[] ValidationWarnings
        {
            get;
            set;
        }

        /// <summary>
        /// Validation and fixing logic
        /// </summary>
        /// <returns>Source data with recoverable data fixed and invalid records filtered out</returns>
        public FileFormatModel[] ValidateAndFix()
        {
            Debug.Assert(_input != null);

            var validationErrors = new List<string>();
            var validationWarnings = new List<string>();
            var fixedData = new List<FileFormatModel>();

            foreach (var model in _input)
            {
                var modelCopy = (FileFormatModel)model.Clone();

                if (!modelCopy.From.HasValue
                    && !modelCopy.To.HasValue
                    && !modelCopy.ObservationDate.HasValue
                    && !modelCopy.Price.HasValue
                    && string.IsNullOrWhiteSpace(modelCopy.Shorthand))
                {
                    validationWarnings.Add($"Line {modelCopy.LineNumber} is empty");
                    continue; // skip empty lines
                }

                if (!modelCopy.Price.HasValue || modelCopy.Price == 0)
                {
                    validationErrors.Add($"Line {modelCopy.LineNumber} has empty price, can not recover");
                    continue; // skip line with error
                }

                if (!modelCopy.ObservationDate.HasValue)
                {
                    validationErrors.Add($"Line {modelCopy.LineNumber} has empty ObservationDate, can not recover");
                    continue; // skip line with error
                }

                if (modelCopy.ObservationDate.Value.Year < Settings.MinYear)
                {
                    validationWarnings.Add($"Line {modelCopy.LineNumber} has suspicious ObservationDate '{modelCopy.ObservationDate:dd/MM/yyyy}'");
                }

                if (string.IsNullOrEmpty(modelCopy.Shorthand))
                {
                    validationErrors.Add($"Line {modelCopy.LineNumber} has empty Shorthand, can not recover");
                    continue; // skip line with error
                }

                // Validate Shorthand field
                ShorthandReader shRd;
                try
                {
                    shRd = ShorthandReader.Read(modelCopy.Shorthand);
                }
                catch (Exception)
                {
                    validationErrors.Add($"Line {modelCopy.LineNumber} has incorrect Shorthand");
                    continue; // skip line with error
                }

                // Try to recover data from Shorthand — From and To are not required by task,
                // so they just produce warnings. But this code verify Shorthand for correctness.

                if (!modelCopy.From.HasValue)
                {
                    modelCopy.From = shRd.From;
                    validationWarnings.Add($"Line {modelCopy.LineNumber} recovered From using Shorthand");
                }
                else if (modelCopy.From != shRd.From)
                {
                    validationWarnings.Add($"Line {modelCopy.LineNumber} has suspicious From '{modelCopy.From:dd/MM/yyyy}' for Shorthand '{modelCopy.Shorthand}'");
                }
                
                if (!modelCopy.To.HasValue)
                {
                    modelCopy.To = shRd.To;
                    validationWarnings.Add($"Line {modelCopy.LineNumber} recovered To using Shorthand");
                }
                else if (modelCopy.To != shRd.To)
                {
                    validationWarnings.Add($"Line {modelCopy.LineNumber} has suspicious To '{modelCopy.To:dd/MM/yyyy}' for Shorthand '{modelCopy.Shorthand}'");
                }

                fixedData.Add(modelCopy);
            }

            ValidationErrors = validationErrors.Any() ? validationErrors.ToArray() : null;
            ValidationWarnings = validationWarnings.Any() ? validationWarnings.ToArray() : null;

            return fixedData.ToArray();
        }
    }
}
