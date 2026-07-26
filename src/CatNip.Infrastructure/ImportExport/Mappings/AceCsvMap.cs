using CatNip.Domain.ImportExport.Csv;

namespace CatNip.Infrastructure.ImportExport.Mappings;

public abstract class AceCsvMap<TExchange> : ClassMap<TExchange>
    where TExchange : ICsvMappable
{
    protected AceCsvMap()
    {
        Map(exchange => exchange.RowNumber).Convert(args => args.Row.Context!.Parser!.Row);
    }
}
