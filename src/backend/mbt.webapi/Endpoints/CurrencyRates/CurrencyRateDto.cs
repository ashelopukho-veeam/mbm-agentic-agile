using CurrencyRate = mbt.webapi.Domain.Entities.CurrencyRate;


namespace mbt.webapi.Endpoints.CurrencyRates;

public class CurrencyRateDto
{
    public string Id { get; set; }
    public double Rate { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }

    public static CurrencyRateDto FromCurrencyRate(CurrencyRate currencyRate)
    {
        return new CurrencyRateDto
        {
            Id = currencyRate.Id,
            Rate = currencyRate.Rate,
            Title = currencyRate.Title,
            Year = currencyRate.Year
        };
    }
}
