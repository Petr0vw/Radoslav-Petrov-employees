using System.Globalization;
using SirmaTask.Enums;
using SirmaTask.Models;
using SirmaTask.Services.Interfaces;

namespace SirmaTask.Services
{
    public class CsvParserService : ICsvParserService
    {
        private static readonly string[] SupportedDateFormats =
        {
            "yyyy-MM-dd", "dd-MM-yyyy",
            "dd/MM/yyyy", "MM/dd/yyyy",
            "dd.MM.yyyy", "yyyy/MM/dd"
        };

        private static readonly char[] SupportedSeparators =
        {
            ',', ';', '\t', '|'
        };

        private static readonly string[] ExpectedHeaders =
        {
            "EmpID", "ProjectID", "DateFrom", "DateTo"
        };

        public CsvParseResult Parse(Stream stream)
        {
            var result = new CsvParseResult();

            if (stream == null || !stream.CanRead)
            {
                AddError(result, 0, ErrorCode.FileReadError);
                return result;
            }

            try
            {
                using var reader = new StreamReader(stream);

                int lineNumber = 0;
                bool firstContentLine = true;
                bool hasContent = false;
                char? separator = null;

                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    lineNumber++;

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    hasContent = true;

                    if (separator == null)
                    {
                        separator = DetectSeparator(line);

                        if (separator == null)
                        {
                            AddError(result, lineNumber, ErrorCode.InvalidColumnCount);
                            return result;
                        }
                    }

                    string[] values = line.Split(separator.Value);

                    if (firstContentLine)
                    {
                        firstContentLine = false;

                        if (IsHeader(values))
                        {
                            continue;
                        }
                    }

                    if (values.Length != ExpectedHeaders.Length)
                    {
                        AddError(result, lineNumber, ErrorCode.InvalidColumnCount);
                        continue;
                    }

                    string employeeValue = values[0].Trim();
                    string projectValue = values[1].Trim();
                    string dateFromValue = values[2].Trim();
                    string dateToValue = values[3].Trim();

                    if (!int.TryParse(employeeValue, out int employeeId))
                    {
                        AddError(result, lineNumber, ErrorCode.InvalidEmployeeId);
                        continue;
                    }

                    if (!int.TryParse(projectValue, out int projectId))
                    {
                        AddError(result, lineNumber, ErrorCode.InvalidProjectId);
                        continue;
                    }

                    if (!TryParseDate(dateFromValue, out DateTime dateFrom))
                    {
                        AddError(result, lineNumber, ErrorCode.InvalidDateFrom);
                        continue;
                    }

                    DateTime dateTo;

                    if (dateToValue.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                    {
                        dateTo = DateTime.Today;
                    }
                    else if (!TryParseDate(dateToValue, out dateTo))
                    {
                        AddError(result, lineNumber, ErrorCode.InvalidDateTo);
                        continue;
                    }

                    if (dateFrom > dateTo)
                    {
                        AddError(result, lineNumber, ErrorCode.DateFromAfterDateTo);
                        continue;
                    }

                    var record = new EmployeeProjectRecord();

                    record.EmployeeId = employeeId;
                    record.ProjectId = projectId;
                    record.DateFrom = dateFrom;
                    record.DateTo = dateTo;

                    result.Records.Add(record);
                }

                if (!hasContent)
                {
                    AddError(result, 0, ErrorCode.EmptyFile);
                }
                else if (result.Records.Count == 0 && result.Errors.Count == 0)
                {
                    AddError(result, 0, ErrorCode.NoValidRecords);
                }
            }
            catch (IOException)
            {
                AddError(result, 0, ErrorCode.FileReadError);
            }

            return result;
        }

        private static char? DetectSeparator(string line)
        {
            foreach (char separator in SupportedSeparators)
            {
                if (line.Split(separator).Length == ExpectedHeaders.Length)
                {
                    return separator;
                }
            }

            return null;
        }

        private static bool TryParseDate(string value, out DateTime date)
        {
            return DateTime.TryParseExact(value, SupportedDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        private static bool IsHeader(string[] values)
        {
            if (values.Length != ExpectedHeaders.Length)
            {
                return false;
            }

            string[] trimmedValues = values.Select(value => value.Trim()).ToArray();

            return trimmedValues.SequenceEqual(ExpectedHeaders, StringComparer.OrdinalIgnoreCase);
        }

        private static void AddError(CsvParseResult result, int lineNumber, ErrorCode errorCode)
        {
            var error = new CsvParseError();

            error.LineNumber = lineNumber;
            error.ErrorCode = errorCode;

            result.Errors.Add(error);
        }
    }
}