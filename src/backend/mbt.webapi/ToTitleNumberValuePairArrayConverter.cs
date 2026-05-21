using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using JetBrains.Annotations;
using mbt.webapi.Domain.Entities;

namespace mbt.webapi;

[PublicAPI]
public class ToTitleNumberValuePairArrayConverter : TypeConverter<List<TitleNumberValuePair>>
{
    public override List<TitleNumberValuePair> ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        var allElements = text.Split("##");
        var result =  (from element in allElements
            let title = element.Split('#')[0]
            let value = double.Parse(element.Split('#')[1])
            select new TitleNumberValuePair() { Title = title, Value = value }).ToList();

        return result;
    }

    public override string ConvertToString(List<TitleNumberValuePair> value, IWriterRow row, MemberMapData memberMapData)
    {
        var result = string.Join("##", value.Select(x => $"{x.Title}#{x.Value}"));
        return result;
    }
}
