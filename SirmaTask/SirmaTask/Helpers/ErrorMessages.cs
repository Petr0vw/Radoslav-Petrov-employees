using SirmaTask.Enums;

namespace SirmaTask.Helpers
{
    public static class ErrorMessages
    {
        public static string Get(ErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ErrorCode.FileNotSelected:
                    return "Please select a CSV file.";

                case ErrorCode.InvalidFileType:
                    return "Only CSV files are supported.";

                case ErrorCode.EmptyFile:
                    return "The CSV file is empty.";

                case ErrorCode.InvalidColumnCount:
                    return "The CSV row must contain exactly 4 columns.";

                case ErrorCode.InvalidEmployeeId:
                    return "Invalid Employee ID.";

                case ErrorCode.InvalidProjectId:
                    return "Invalid Project ID.";

                case ErrorCode.InvalidDateFrom:
                    return "Invalid DateFrom value.";

                case ErrorCode.InvalidDateTo:
                    return "Invalid DateTo value.";

                case ErrorCode.DateFromAfterDateTo:
                    return "DateFrom cannot be later than DateTo.";

                case ErrorCode.NoValidRecords:
                    return "No valid employee records were found.";

                case ErrorCode.NoEmployeePairFound:
                    return "No employees have worked together on common projects.";

                case ErrorCode.FileReadError:
                    return "The file could not be read.";

                default:
                    return "An unexpected error occurred.";
            }
        }
    }
}