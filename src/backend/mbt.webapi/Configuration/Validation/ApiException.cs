using System;

namespace mbt.webapi.Configuration.Validation;

public class ApiException : Exception
{
    public ApiException(string message) : base(message)
    {
    }
}

public class AccessDeniedException : Exception
{
}
