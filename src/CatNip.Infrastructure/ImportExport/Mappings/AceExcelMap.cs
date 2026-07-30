using CatNip.Domain.ImportExport.Excel;

namespace CatNip.Infrastructure.ImportExport.Mappings;

public abstract class AceExcelMap<TExcel> : IExcelMap<TExcel>
    where TExcel : IExcelMappable
{
    public abstract TExcel Map(IXLRow row, IReadOnlyDictionary<string, int> headers);
}
