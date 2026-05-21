using System.Collections.Generic;
using JetBrains.Annotations;

namespace mbt.webapi.Services;

[PublicAPI]
public class BulkOperationResult
{
    public string Title { get; private set; }
    public List<ErrorData> Errors { get; private set; } = new();

    public void AddError(string id, string error)
    {
        Errors.Add(new ErrorData { Id = id, Error = error });
    }

    public BulkOperationResult(string title)
    {
        Title = title;
    }
}

[PublicAPI]
public class ErrorData
{
    public string Id { get; set; }
    public string Error { get; set; }
}
