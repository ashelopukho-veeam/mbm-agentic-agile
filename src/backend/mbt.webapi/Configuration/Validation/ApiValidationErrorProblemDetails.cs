using System;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace mbt.webapi.Configuration.Validation;

public class ApiValidationErrorProblemDetails : ValidationProblemDetails
{
    public ApiValidationErrorProblemDetails(ApiException exception)
    {
        Status = StatusCodes.Status400BadRequest;
        Detail = exception.Message;
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
    }

    public ApiValidationErrorProblemDetails(Exception exception)
    {
        Status = StatusCodes.Status400BadRequest;
        Detail = exception.Message;
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
    }

    public ApiValidationErrorProblemDetails(DuplicateItemException exception)
    {
        Status = StatusCodes.Status409Conflict;
        Detail = exception.Message;
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8";
    }

    public ApiValidationErrorProblemDetails(ValidationException exception)
    {
        Status = StatusCodes.Status400BadRequest;
        Detail = exception.Message;
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

        var errors = exception.Errors.GroupBy(x => x.PropertyName)
            .ToDictionary(x => x.Key, x => x.Select(y => y.ErrorMessage).ToList());

        foreach (var (key, value) in errors) Errors.Add(key, value.ToArray());
    }
}

// duplicate item exception
