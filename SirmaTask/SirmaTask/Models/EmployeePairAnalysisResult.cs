namespace SirmaTask.Models
{
    public class EmployeePairAnalysisResult
    {
        public List<EmployeePairResult> Results { get; set; } = new List<EmployeePairResult>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}