using System.Collections.Generic;

namespace mbt.webapi.Services;

public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public long TotalPages { get; set; }
}
