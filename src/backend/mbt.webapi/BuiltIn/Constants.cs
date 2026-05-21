using System;
using System.Globalization;
using CsvHelper.Configuration;

namespace mbt.webapi.BuiltIn;

public static class BuiltInConstants
{
    public static readonly string SystemUserId = Guid.Empty.ToString();
    public static readonly string SystemUserName = "System Account";
    public const int ObjectIdLength = 24;

    public const int BudgetTitleLengthLimit = 80;

    public const int ForecastsPerYear = 4;
}

public static class Quarters
{
    public const string Q1 = "Q1";
    public const string Q2 = "Q2";
    public const string Q3 = "Q3";
    public const string Q4 = "Q4";
}

public static class WorkflowEventNames
{
    public static readonly string ResolveTask = "ResolveTask";
}

public static class WorkflowNames
{
    public static readonly string IncrementalFundsApproveWorkflow = "IncrementalFundsApproveWorkflow";
    public static readonly string SubmitTransferWorkflow = "SubmitTransferWorkflow";
}

public static class TaskTypes
{
    public const string IncrementalFund = "IncrementalFund";
    public const string Transfer = "Transfer";
}

public static class CsvConstants
{
    private const string NumberDecimalSeparator = ".";
    private const string Delimiter = ",";

    public static CsvConfiguration DefaultCsvConfiguration
    {
        get
        {
            CultureInfo cultureInfo = new(CultureInfo.InvariantCulture.LCID)
            {
                NumberFormat =
                {
                    NumberDecimalSeparator = NumberDecimalSeparator
                }
            };

            return new CsvConfiguration(cultureInfo)
            {
                Delimiter = Delimiter
            };
        }
    }
}

public static class ExceptionMessages
{
    public const string UserNotFound = "User not found";
}
