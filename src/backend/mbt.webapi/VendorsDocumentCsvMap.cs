using CsvHelper.Configuration;
using JetBrains.Annotations;
using VendorsDocument = mbt.webapi.Domain.Entities.VendorsDocument;

namespace mbt.webapi;

[UsedImplicitly]
public sealed class VendorsDocumentCsvMap : ClassMap<VendorsDocument>
{
    public VendorsDocumentCsvMap()
    {
        Map(m => m.Title).Name("Title");
        Map(m => m.Id).Ignore();
    }
}
