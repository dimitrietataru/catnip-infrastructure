using CatNip.Domain.Exceptions;
using CatNip.Domain.ImportExport.Csv;

namespace CatNip.Infrastructure.ImportExport;

public abstract class AceCsvConverter : ICsvConverter
{
    protected abstract IReadOnlyDictionary<Type, Type> Mappings { get; }
    protected abstract CsvConfiguration Configuration { get; }

    public virtual async Task<ICollection<TCsv>> ReadAsync<TCsv>(Stream stream, CancellationToken cancellation = default)
        where TCsv : ICsvMappable
    {
        using var reader = new StreamReader(stream, leaveOpen: false);
        using var csv = new CsvReader(reader, Configuration);

        if (!Mappings.TryGetValue(typeof(TCsv), out var classMap))
        {
            throw new CsvMappingNotFoundException(typeof(TCsv));
        }

        csv.Context.RegisterClassMap(classMap);
        var records = csv.GetRecords<TCsv>().ToList();

        return records;
    }
}
