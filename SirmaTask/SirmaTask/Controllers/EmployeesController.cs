using Microsoft.AspNetCore.Mvc;
using SirmaTask.Enums;
using SirmaTask.Helpers;
using SirmaTask.Models;
using SirmaTask.Services.Interfaces;

namespace SirmaTask.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ICsvParserService _csvParserService;
        private readonly IEmployeePairAnalyzer _employeePairAnalyzer;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(ICsvParserService csvParserService, IEmployeePairAnalyzer employeePairAnalyzer, ILogger<EmployeesController> logger)
        {
            _csvParserService = csvParserService;
            _employeePairAnalyzer = employeePairAnalyzer;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Analyze()
        {
            return View(new EmployeePairAnalysisResult());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Analyze(IFormFile csvFile)
        {
            var result = new EmployeePairAnalysisResult();

            if (csvFile == null || csvFile.Length == 0)
            {
                _logger.LogWarning("CSV analysis requested without a file.");

                result.Errors.Add(ErrorMessages.Get(ErrorCode.FileNotSelected));
                return View(result);
            }

            _logger.LogInformation("CSV file received. FileName: {FileName}, Size: {FileSize} bytes.", csvFile.FileName, csvFile.Length);

            string extension = Path.GetExtension(csvFile.FileName);

            if (!extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Invalid file type uploaded. FileName: {FileName}.", csvFile.FileName);

                result.Errors.Add(ErrorMessages.Get(ErrorCode.InvalidFileType));
                return View(result);
            }

            try
            {
                using Stream stream = csvFile.OpenReadStream();

                CsvParseResult parseResult = _csvParserService.Parse(stream);

                if (parseResult.Errors.Count > 0)
                {
                    _logger.LogWarning("CSV parsing completed with {ErrorCount} errors.", parseResult.Errors.Count);

                    foreach (CsvParseError error in parseResult.Errors)
                    {
                        result.Errors.Add($"Line {error.LineNumber}: {ErrorMessages.Get(error.ErrorCode)}");
                    }

                    return View(result);
                }

                _logger.LogInformation("CSV parsed successfully. Records: {RecordCount}.", parseResult.Records.Count);

                result = _employeePairAnalyzer.FindLongestWorkingPair(parseResult.Records);

                if (result.Errors.Count > 0)
                {
                    _logger.LogWarning("Employee pair analysis completed with {ErrorCount} errors.", result.Errors.Count);

                    return View(result);
                }

                _logger.LogInformation("Employee pair analysis completed successfully. Result rows: {ResultCount}.", result.Results.Count);

                return View(result);
            }
            catch (IOException exception)
            {
                _logger.LogError(exception, "An I/O error occurred while processing file {FileName}.", csvFile.FileName);

                result.Errors.Add(ErrorMessages.Get(ErrorCode.FileReadError));
                return View(result);
            }
        }
    }
}