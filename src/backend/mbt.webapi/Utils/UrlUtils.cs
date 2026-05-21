using System.Linq;

namespace mbt.webapi.Utils;

public static class UrlUtils
{
    public static string Combine(params string[] urlParts)
    {
        var trimmedParts = urlParts
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Select(u => u.Trim('/'));
        return string.Join("/", trimmedParts);
    }
}
