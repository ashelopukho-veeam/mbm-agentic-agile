using JetBrains.Annotations;
using DictionaryDocument = mbt.webapi.Domain.Entities.DictionaryDocument;


namespace mbt.webapi.Endpoints.Dictionaries;

[PublicAPI]
public class GetDictionariesResponse
{
    public string Title { get; set; }
    public string InternalName { get; set; }
    public string Id { get; set; }

    public static GetDictionariesResponse FromDictionaryDocument(DictionaryDocument dictionaryDocument)
    {
        return new GetDictionariesResponse()
        {
            Title = dictionaryDocument.Title,
            InternalName = dictionaryDocument.InternalName,
            Id = dictionaryDocument.Id
        };
    }
}
