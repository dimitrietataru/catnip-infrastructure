using CatNip.Domain.ImportExport.Excel;

namespace CatNip.Infrastructure.ImportExport.Mappings;

public interface IExcelMap<TExcel> : IExcelMap
    where TExcel : IExcelMappable
{
    TExcel Map(IXLRow row, IReadOnlyDictionary<string, int> headers);
}

public interface IExcelMap
{
}
