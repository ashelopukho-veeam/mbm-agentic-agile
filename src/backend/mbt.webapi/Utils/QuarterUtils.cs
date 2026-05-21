using System;
using System.Linq;

namespace mbt.webapi.Utils;

public static class QuarterUtils
{
    public static readonly string[] Quarters = { "Q1", "Q2", "Q3", "Q4" };

    public static string GetNextQuarterName(string quarter)
    {
        return Quarters.SkipWhile(item => item != quarter).Skip(1).FirstOrDefault();
    }

    public static int GetQuarterNumber(string quarter)
    {
        return Array.IndexOf(Quarters, quarter) + 1;
    }
}
