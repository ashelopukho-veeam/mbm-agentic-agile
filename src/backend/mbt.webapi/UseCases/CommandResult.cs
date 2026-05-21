namespace mbt.webapi.UseCases;

public class CommandResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public bool IsNotFound { get; set; }
}

public class CommandResult<T>
{
    public string Message { get; set; }
    public T Data { get; set; }

    public CommandResultStatus Status { get; set; }

    public static CommandResult<T> Success(T data, string message = null) => new()
        { Status = CommandResultStatus.Success, Message = message, Data = data };

    public static CommandResult<T> Failure(string message) =>
        new() { Status = CommandResultStatus.Failure, Message = message };

    public static CommandResult<T> NotFound(string message) =>
        new() { Status = CommandResultStatus.NotFound, Message = message };
}

public enum CommandResultStatus
{
    Success,
    Failure,
    NotFound
}
