using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using mbt.webapi.Domain.Entities;
using MongoDB.Bson;

namespace mbt.webapi;

public static class CustomValidators
{
    // check if string is a valid ObjectId
    public static IRuleBuilderOptions<T, string> IsObjectId<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Length(24)
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage((_, id) => id + " is not a valid 24 digit hex string.")
            .NotEmpty();
    }

    // sum of items should be 100
    public static IRuleBuilderOptions<T, List<TitleNumberValuePair>> SumOfItemsShouldBe100<T>(
        this IRuleBuilder<T, List<TitleNumberValuePair>> ruleBuilder)
    {
        return ruleBuilder.Must(x => (int)Math.Ceiling(x.Sum(s => s.Value)) == 100)
            .WithMessage("Sum of items should be 100");
    }

    // all items are positive
    public static IRuleBuilderOptions<T, List<TitleNumberValuePair>> AllItemsPositive<T>(
        this IRuleBuilder<T, List<TitleNumberValuePair>> ruleBuilder)
    {
        return ruleBuilder.Must(x => x.All(s => s.Value > 0))
            .WithMessage("All items should be positive");
    }

    // is valid percentage distribution == all items are positive and sum of items should be 100
    public static IRuleBuilderOptions<T, List<TitleNumberValuePair>> IsValidPercentageDistribution<T>(
        this IRuleBuilder<T, List<TitleNumberValuePair>> ruleBuilder)
    {
        return ruleBuilder.AllItemsPositive().SumOfItemsShouldBe100();
    }
}
