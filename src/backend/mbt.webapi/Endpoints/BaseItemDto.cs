using System;

namespace mbt.webapi.Endpoints;

public class BaseItemDto
{
    public string Id { get; set; }
    public string Title { get; set; }

    public string CreatedBy { get; set; }
    public DateTime Created { get; set; }

    public string ModifiedBy { get; set; }
    public DateTime Modified { get; set; }
}
