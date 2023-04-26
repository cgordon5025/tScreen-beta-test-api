using System.Collections.Generic;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace Application.CsvFiles;

public interface ICsvFileProcessor<T>
{
    public Task<ICollection<CsvRecordResult<T>>> ParseAsync<TMap>() where TMap : ClassMap<T>;
}