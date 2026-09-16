namespace SirmaTask.Enums
{
    public enum ErrorCode
    {
        None = 0,

        FileNotSelected,
        InvalidFileType,
        EmptyFile,
        InvalidColumnCount,

        InvalidEmployeeId,
        InvalidProjectId,

        InvalidDateFrom,
        InvalidDateTo,
        DateFromAfterDateTo,

        NoValidRecords,
        NoEmployeePairFound,
        FileReadError
    }
}