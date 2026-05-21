using System.Collections.Generic;
using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Domain.Entities;

public class DictionaryDocument : BaseIdItem
{
    public DictionaryDocument(string title)
    {
        Title = title;
        InternalName = title.Replace(" ", "_").ToLower();
    }

    public string Title { get; set; }
    public string InternalName { get; set; }
    public List<string> Items { get; set; } = new();
}
