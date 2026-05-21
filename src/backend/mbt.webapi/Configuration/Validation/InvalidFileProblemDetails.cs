using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace mbt.webapi.Configuration.Validation;

public class InvalidFileProblemDetails : ProblemDetails
{
    public InvalidFileProblemDetails(CsvHelperException exception)
    {
        Title = "Invalid file";
        Status = StatusCodes.Status400BadRequest;
        Type = "invalid-file";
        Detail = exception.Message;
    }
}
