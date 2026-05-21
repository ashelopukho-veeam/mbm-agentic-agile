using System.Collections.Generic;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Shared;

namespace mbt.webapi.Services.Interfaces;

public interface ICurrencyService : IBaseService
{
    Task ImportCsvExchangeRates(FileModel file);
    Task<CurrencyRate> GetCurrencyRate(string currencyName, int year);
    Task<List<CurrencyRate>> GetCurrencyRates();
    Task<List<CurrencyRate>> GetByYear(int year);
    Task<double> CalculateNetPlannedAmount(int year, double plannedAmount, string currency,
        double plannedSponsorship);
}
