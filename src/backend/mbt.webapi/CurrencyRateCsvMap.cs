using CsvHelper.Configuration;
using JetBrains.Annotations;
using CurrencyRate = mbt.webapi.Domain.Entities.CurrencyRate;

namespace mbt.webapi;

[UsedImplicitly]
public sealed class CurrencyRateCsvMap : ClassMap<CurrencyRate>
{
    public CurrencyRateCsvMap()
    {
        Map(m => m.Title).Name("Currency");
        Map(m => m.Year).Name("Effective Year");
        Map(m => m.Rate).Name("Price");
    }
}
