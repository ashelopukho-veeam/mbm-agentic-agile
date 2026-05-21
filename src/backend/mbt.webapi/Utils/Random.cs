using System;
using System.Collections.Generic;
using System.Linq;
using mbt.webapi.Domain.Entities;

namespace mbt.webapi.Utils;

public static class RandomUtils
{
    public static TransferExpanded BuildRandomTransferExpanded()
    {
        var fromBudget = GetRandomBudget(2023);
        var toBudget = GetRandomBudget(2023);

        return new TransferExpanded
        {
            Id = Guid.NewGuid().ToString(),
            Title = GetRandomWords(3),
            Status = "Approved",
            Amount = GetRandomInt(1, 1000),
            CreatedByUser = GetRandomUserProfile(),
            Created = DateTime.Now,
            FromBudget = fromBudget,
            ToBudget = toBudget,
            ModifiedByUser = GetRandomUserProfile(),
            Modified = DateTime.Now,
            Year = 2023,
            Plan = "Q3",
            Team = GetRandomWords(2)
        };
    }


    public static List<TitleNumberValuePair> GetRandomTitleNumberPair(int count)
    {
        var pairs = new List<TitleNumberValuePair>();
        for (int i = 0; i < count; i++)
        {
            pairs.Add(new TitleNumberValuePair
            {
                Title = GetRandomString(),
                Value = GetRandomInt(1, 1000)
            });
        }

        return pairs;
    }

    public static BudgetPlan GetRandomPlan()
    {
        return new BudgetPlan
        {
            Id = GetRandomString(),
            Quarter = GetRandomString(),
            Q1 = GetRandomInt(1, 1000),
            Q2 = GetRandomInt(1, 1000),
            Q3 = GetRandomInt(1, 1000),
            Q4 = GetRandomInt(1, 1000),
            Status = GetRandomString(),
            Segments = GetRandomTitleNumberPair(4),
            Campaigns = GetRandomTitleNumberPair(4)
        };
    }

    public static List<BudgetPlan> GetRandomPlans()
    {
        var plans = new List<BudgetPlan>();
        for (int i = 0; i < 4; i++)
        {
            plans.Add(GetRandomPlan());
        }

        return plans;
    }

    public static Budget GetRandomBudget(int year)
    {
        var budget = new Budget
        {
            Id = GetRandomString(),
            Title = GetRandomString(),
            Year = year,
            Level1 = GetRandomString(),
            Level2 = GetRandomString(),
            Level3 = GetRandomString(),
            CostCenter = GetRandomString(),
            OwnerId = GetRandomString(),
            ParentManagerId = GetRandomString(),
            ManagerId = GetRandomString(),
            Status = GetRandomString(),
            BudgetType = GetRandomString(),
            UseInCoupa = GetRandomBool(),
            UseInTableau = GetRandomBool(),
            IsPaidMedia = GetRandomBool(),
            CreatedBy = GetRandomString(),
            ModifiedBy = GetRandomString(),
            Created = DateTime.Now,
            Modified = DateTime.Now,
        };

        var plans = GetRandomPlans();
        foreach (var plan in plans)
        {
            budget.Plans.Add(plan);
        }

        return budget;
    }


    public static int GetRandomInt(int min, int max)
    {
        var random = new Random();
        return random.Next(min, max);
    }

    public static bool GetRandomBool()
    {
        return GetRandomInt(0, 1) == 1;
    }

    public static UserProfile GetRandomUserProfile()
    {
        string email = GetRandomEmail();
        string displayName = GetRandomWords(2);

        return new UserProfile
        {
            DisplayName = displayName,
            Email = email
        };
    }

    public static string GetRandomEmail()
    {
        return $"{GetRandomString()}@{GetRandomString()}.com";
    }

    public static string GetRandomString()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, random.Next(1, 10))
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string GetRandomWords(int count)
    {
        var words = new List<string>();
        for (int i = 0; i < count; i++)
        {
            words.Add(GetRandomString());
        }

        return string.Join(" ", words);
    }
}
