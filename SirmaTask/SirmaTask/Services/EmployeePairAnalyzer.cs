using SirmaTask.Enums;
using SirmaTask.Helpers;
using SirmaTask.Models;
using SirmaTask.Services.Interfaces;

namespace SirmaTask.Services
{
    public class EmployeePairAnalyzer : IEmployeePairAnalyzer
    {
        public EmployeePairAnalysisResult FindLongestWorkingPair(IEnumerable<EmployeeProjectRecord> records)
        {
            var result = new EmployeePairAnalysisResult();

            if (records == null)
            {
                result.Errors.Add(ErrorMessages.Get(ErrorCode.NoValidRecords));
                return result;
            }

            List<EmployeeProjectRecord> recordsList = records.ToList();

            if (recordsList.Count == 0)
            {
                result.Errors.Add(ErrorMessages.Get(ErrorCode.NoValidRecords));
                return result;
            }

            var projectResults = new List<EmployeePairResult>();

            var projects = recordsList.GroupBy(record => record.ProjectId).ToList();

            foreach (var project in projects)
            {
                var employees = project.GroupBy(record => record.EmployeeId).OrderBy(employee => employee.Key).ToList();

                if (employees.Count < 2)
                {
                    continue;
                }

                for (int i = 0; i < employees.Count; i++)
                {
                    for (int j = i + 1; j < employees.Count; j++)
                    {
                        int employee1 = employees[i].Key;
                        int employee2 = employees[j].Key;

                        List<EmployeeProjectRecord> firstPeriods = MergePeriods(employees[i].ToList());
                        List<EmployeeProjectRecord> secondPeriods = MergePeriods(employees[j].ToList());

                        int daysWorked = CalculateOverlapDays(firstPeriods, secondPeriods);

                        if (daysWorked == 0)
                        {
                            continue;
                        }

                        var projectResult = new EmployeePairResult();

                        projectResult.EmployeeId1 = employee1;
                        projectResult.EmployeeId2 = employee2;
                        projectResult.ProjectId = project.Key;
                        projectResult.DaysWorked = daysWorked;

                        projectResults.Add(projectResult);
                    }
                }
            }

            if (projectResults.Count == 0)
            {
                result.Errors.Add(ErrorMessages.Get(ErrorCode.NoEmployeePairFound));
                return result;
            }

            var winningPair = projectResults.GroupBy(project => (project.EmployeeId1, project.EmployeeId2)).Select(group => (Employee1: group.Key.EmployeeId1, Employee2: group.Key.EmployeeId2, TotalDays: group
                .Sum(project => project.DaysWorked))).OrderByDescending(pair => pair.TotalDays).ThenBy(pair => pair.Employee1).ThenBy(pair => pair.Employee2).First();

            List<EmployeePairResult> winningProjects = projectResults.Where(project => project.EmployeeId1 == winningPair.Employee1 && project.EmployeeId2 == winningPair.Employee2)
                .OrderBy(project => project.ProjectId).ToList();

            result.Results.AddRange(winningProjects);

            return result;
        }

        private static List<EmployeeProjectRecord> MergePeriods(List<EmployeeProjectRecord> records)
        {
            List<EmployeeProjectRecord> sortedRecords = records.OrderBy(record => record.DateFrom).ToList();

            var mergedRecords = new List<EmployeeProjectRecord>();

            EmployeeProjectRecord current = CopyRecord(sortedRecords[0]);

            for (int i = 1; i < sortedRecords.Count; i++)
            {
                EmployeeProjectRecord next = sortedRecords[i];

                if (next.DateFrom <= current.DateTo)
                {
                    if (next.DateTo > current.DateTo)
                    {
                        current.DateTo = next.DateTo;
                    }
                }
                else
                {
                    mergedRecords.Add(current);
                    current = CopyRecord(next);
                }
            }

            mergedRecords.Add(current);

            return mergedRecords;
        }

        private static int CalculateOverlapDays(List<EmployeeProjectRecord> firstPeriods, List<EmployeeProjectRecord> secondPeriods)
        {
            int totalDays = 0;

            foreach (EmployeeProjectRecord firstPeriod in firstPeriods)
            {
                foreach (EmployeeProjectRecord secondPeriod in secondPeriods)
                {
                    DateTime start = firstPeriod.DateFrom > secondPeriod.DateFrom ? firstPeriod.DateFrom : secondPeriod.DateFrom;
                    DateTime end = firstPeriod.DateTo < secondPeriod.DateTo ? firstPeriod.DateTo : secondPeriod.DateTo;

                    if (start <= end)
                    {
                        totalDays += (end - start).Days + 1;
                    }
                }
            }

            return totalDays;
        }

        private static EmployeeProjectRecord CopyRecord(EmployeeProjectRecord record)
        {
            var copy = new EmployeeProjectRecord();

            copy.EmployeeId = record.EmployeeId;
            copy.ProjectId = record.ProjectId;
            copy.DateFrom = record.DateFrom;
            copy.DateTo = record.DateTo;

            return copy;
        }
    }
}