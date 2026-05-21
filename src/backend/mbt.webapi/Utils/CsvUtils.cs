using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using mbt.webapi.BuiltIn;
using Microsoft.AspNetCore.Http;

namespace mbt.webapi.Utils;

public static class CsvUtils<T>
{
    public static List<T> GetCsvDataFromFile(string path, CsvConfiguration csvConfiguration = null)
    {
        using var reader = File.OpenRead(path);

        return GetCsvDataInternal(reader, csvConfiguration ?? CsvConstants.DefaultCsvConfiguration);
    }

    public static List<T> GetCsvDataFromFile(IFormFile formFile, CsvConfiguration csvConfiguration = null)
    {
        return GetCsvDataInternal(formFile.OpenReadStream(), csvConfiguration ?? CsvConstants.DefaultCsvConfiguration);
    }

    private static List<T> GetCsvDataInternal(Stream stream, CsvConfiguration csvConfiguration)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, csvConfiguration);
        var records = csv.GetRecords<T>().ToList();

        return records;
    }
}
