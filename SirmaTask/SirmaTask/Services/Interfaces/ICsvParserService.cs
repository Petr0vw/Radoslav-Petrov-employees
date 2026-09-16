using SirmaTask.Models;

namespace SirmaTask.Services.Interfaces
{
    public interface ICsvParserService
    {
        CsvParseResult Parse(Stream stream);
    }
}