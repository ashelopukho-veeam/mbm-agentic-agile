using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.Dictionaries;

[PublicAPI]
public record DictionaryDto(string Id, string Title, string InternalName, string[] Items);
