using SirmaTask.Models;

namespace SirmaTask.Services.Interfaces
{
    public interface IEmployeePairAnalyzer
    {
        EmployeePairAnalysisResult FindLongestWorkingPair(IEnumerable<EmployeeProjectRecord> records);
    }
}