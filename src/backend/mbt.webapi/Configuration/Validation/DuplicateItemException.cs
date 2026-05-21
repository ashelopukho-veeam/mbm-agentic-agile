using System;

namespace mbt.webapi.Configuration.Validation;

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message)
    {
    }
}
