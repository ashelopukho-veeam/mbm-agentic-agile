namespace mbt.webapi.BuiltIn;

public static class StringTokens
{
    public const string Host = "#HOST#";
}

public static class HostRoutesUtils
{
    public static string GetTaskUri(string taskId)
    {
        return $"{StringTokens.Host}/tasks/view/{taskId}";
    }
}
