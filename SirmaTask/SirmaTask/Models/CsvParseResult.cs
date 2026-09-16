namespace SirmaTask.Models
{
    public class CsvParseResult
    {
        public List<EmployeeProjectRecord> Records { get; set; } = new();

        public List<CsvParseError> Errors { get; set; } = new();

        public bool IsValid => Errors.Count == 0;
    }
}