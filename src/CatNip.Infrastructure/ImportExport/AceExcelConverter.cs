using CatNip.Domain.Exceptions;
using CatNip.Domain.ImportExport.Excel;
using CatNip.Infrastructure.ImportExport.Mappings;

namespace CatNip.Infrastructure.ImportExport;

public abstract class AceExcelConverter : IExcelConverter
{
    protected abstract IReadOnlyDictionary<Type, IExcelMap> Mappings { get; }

    public virtual async Task<ICollection<TExcel>> ReadAsync<TExcel>(Stream stream, CancellationToken cancellation = default)
        where TExcel : IExcelMappable
    {
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheet(1);

        var mapper = GetMapping<TExcel>();
        var headers = GetHeaders(sheet);

        var records = sheet
            .RowsUsed()
            .Skip(1)
            .Select(row => mapper.Map(row, headers))
            .ToList();

        return records;
    }

    protected virtual IExcelMap<TExcel> GetMapping<TExcel>()
        where TExcel : IExcelMappable
    {
        if (!Mappings.TryGetValue(typeof(TExcel), out var map))
        {
            throw new ExcelMappingNotFoundException(typeof(TExcel));
        }

        return (IExcelMap<TExcel>)map;
    }

    protected virtual IReadOnlyDictionary<string, int> GetHeaders(IXLWorksheet sheet)
    {
        var headerRow = sheet.Row(1);

        return headerRow
            .CellsUsed()
            .Select((cell, index) => (Name: cell.GetString().Trim(), ColumnIndex: index + 1))
            .ToDictionary(k => k.Name, v => v.ColumnIndex, StringComparer.Ordinal);
    }
}
