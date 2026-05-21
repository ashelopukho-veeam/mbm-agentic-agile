using Microsoft.AspNetCore.Http;

namespace mbt.webapi.Shared;

public class FileModel
{
    public string FileName { get; set; }
    public IFormFile FormFile { get; set; }
}
