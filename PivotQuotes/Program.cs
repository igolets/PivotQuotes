using System;
using System.Globalization;
using System.IO;
using System.Linq;
using FileHelpers;
using PivotQuotes.Import;

namespace PivotQuotes
{
    internal static class Program
    {
        #region Consts

        private const String ParameterHelpOption = "/?";

        #endregion

        static void Main(string[] args)
        {
            if ( args.Length != 2 || args [ 0 ] == ParameterHelpOption )
            {
                Console.WriteLine( @"Run the app with parameters:
PivotQuotes.exe <src> <dst>
Where:
src - source csv file name (tabular)
dst - destination file name (grid)

Press ENTER to finish.");
                Console.ReadLine();
                return;
            }

            // change locale format settings. Works starting from .Net 4.5
            // be careful when copying this to ASP.NET, thought

            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.DateTimeFormat.DateSeparator = "/";
            ci.NumberFormat.NumberDecimalSeparator = ".";
            CultureInfo.DefaultThreadCurrentCulture = ci;

            // read source data

            Console.WriteLine("Reading source file...");

            var csvEngine = new FileHelperEngine<FileFormatModel>();
            csvEngine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;

            var srcData = csvEngine.ReadFile(args[0]);

            if (csvEngine.ErrorManager.HasErrors)
            {
                Console.WriteLine("Following lines with errors were skipped:");
                foreach (var error in csvEngine.ErrorManager.Errors)
                {
                    Console.WriteLine( $"Line {error.LineNumber}, {error.ExceptionInfo.Message}");
                }

                Console.Write("Do you want to continue with wrong data skipped? [y/N]:");
                if (!WaitForContinueConfirmation())
                {
                    return;
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Validating source data...");

            var validator = new FileFormatValidator(srcData);
            var fixedData = validator.ValidateAndFix();

            if (validator.ValidationErrors != null)
            {
                Console.WriteLine("There are unrecoverable validation errors:");
                foreach (var error in validator.ValidationErrors)
                {
                    Console.WriteLine(error);
                }
                Console.Write(@"Please fix the data and run the tool again.

Do you want to continue with wrong data skipped? [y/N]:");
                if (!WaitForContinueConfirmation())
                {
                    return;
                }
            }

            Console.WriteLine();
            Console.WriteLine();

            if (validator.ValidationWarnings != null)
            {
                Console.WriteLine("FYI: there were some data fixed by program, please review:");
                foreach (var warning in validator.ValidationWarnings)
                {
                    Console.WriteLine(warning);
                }
                Console.WriteLine(@"If something is wrongly fixed, please fix the data manually and run the tool again.");
            }

            // pivot creation

            Console.WriteLine();
            Console.WriteLine("Creating pivot grid...");

            // pivor headers in correct order ('Q3_07' is _before_ 'Q1_10')
            var pivotHeaders = fixedData
                .Select(x => x.Shorthand)
                .Distinct()
                .Select(x => new ShorthandWrapper(x))
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Quarter)
                .ToArray();

            var pivotLines = fixedData
                .GroupBy(x=>x.ObservationDate.GetValueOrDefault())
                .OrderBy(grp=>grp.Key);

            using (var file = new StreamWriter(args[1]))
            {
                // write headers
                foreach (var header in pivotHeaders)
                {
                    file.Write(",");
                    file.Write(header.SrcValue);
                }
                file.WriteLine();

                // write lines
                foreach (var line in pivotLines)
                {
                    var dateStr = line.Key.ToString("dd/MM/yyyy");
                    file.Write($"{dateStr},");

                    // write cells
                    foreach (var header in pivotHeaders)
                    {
                        var cellvalue = line.Where(x => x.Shorthand == header.SrcValue).ToArray();
                        if (cellvalue.Length > 0)
                        {
                            file.Write(cellvalue[0].Price.GetValueOrDefault());
                        }
                        if (cellvalue.Length > 1)
                        {
                            Console.WriteLine($"Found more than one price value for ObservationDate={dateStr} and Shorthand={header.SrcValue}");
                        }
                        file.Write(",");
                    }

                    file.WriteLine();
                }
            }

            Console.WriteLine($@"Resulting data written to {args[1]}.

Press ENTER to finish.");
            Console.ReadLine();
        }

        private static bool WaitForContinueConfirmation()
        {
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.N)
                {
                    return false;
                }

                if(key.Key == ConsoleKey.Y)
                {
                    return true;
                }
            }
        }
    }
}
