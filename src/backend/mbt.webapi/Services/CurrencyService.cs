using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Shared;

namespace mbt.webapi.Services;

public class CurrencyService : ICurrencyService
{
    private readonly IDbBaseRepository<CurrencyRate> _currencyRepository;

    public CurrencyService(IDbBaseRepository<CurrencyRate> currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public async Task ImportCsvExchangeRates(FileModel file)
    {
        using var reader = new StreamReader(file.FormFile.OpenReadStream());
        using var csv = new CsvReader(reader, CsvConstants.DefaultCsvConfiguration);
        csv.Context.RegisterClassMap<CurrencyRateCsvMap>();
        var records = csv.GetRecords<CurrencyRate>().ToList();

        if (records.Count == 0)
            throw new ApiException("No records found in the file");

        if (records.Count > 0)
        {
            await _currencyRepository.ClearAsync();
            await _currencyRepository.CreateManyAsync(records);
        }
    }

    public Task<CurrencyRate> GetCurrencyRate(string currencyName, int year)
    {
        return _currencyRepository.FindOneAsync(c => c.Title == currencyName && c.Year == year);
    }

    public async Task<double> CalculateNetPlannedAmount(int year, double plannedAmount, string currency,
        double plannedSponsorship)
    {
        var currencyRate = await GetCurrencyRate(currency, year);
        if (currencyRate == null)
            throw new ApiException(ErrorMessages.CannotFindCurrencyRateFor(currency));

        return (plannedAmount - plannedSponsorship) * currencyRate.Rate;
    }

    public Task<List<CurrencyRate>> GetCurrencyRates()
    {
        return _currencyRepository.GetAsync();
    }

    public Task<List<CurrencyRate>> GetByYear(int year)
    {
        return _currencyRepository.FindAsync(c => c.Year == year);
    }
}
