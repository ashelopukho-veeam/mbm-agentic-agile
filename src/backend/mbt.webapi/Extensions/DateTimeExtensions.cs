using System;

namespace mbt.webapi.Extensions;

public static class DateTimeExtensions
{
    public static int GetQuarter(this DateTime date)
    {
        return (date.Month + 2) / 3;
    }
}
