using System;

namespace mbt.webapi.Shared;

public class Period
{
    private bool Equals(Period other)
    {
        return Year == other.Year && Plan == other.Plan;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Period)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Year, Plan);
    }

    public Period(int year, int plan)
    {
        Year = year;
        Plan = plan;
    }

    public Period(int year, string plan)
    {
        Year = year;
        Plan = int.Parse(plan[1..]);
    }

    public int Year { get; }
    public int Plan { get; }

    public string PlanName => "Q" + Plan;

    public static Period GetPreviousPeriod(Period period)
    {
        return period.Plan == 1 ? new Period(period.Year - 1, 4) : new Period(period.Year, period.Plan - 1);
    }

    public static Period GetNextPeriod(Period period)
    {
        return period.Plan == 4 ? new Period(period.Year + 1, 1) : new Period(period.Year, period.Plan + 1);
    }

    public Period Previous()
    {
        return this - 1;
    }

    public Period Next()
    {
        return this + 1;
    }

    public static Period operator +(Period period, int value)
    {
        var plan = period.Plan + value;
        var year = period.Year;
        if (plan > 4)
        {
            plan = 1;
            year++;
        }

        return new Period(year, plan);
    }

    public static Period operator -(Period period, int value)
    {
        var plan = period.Plan - value;
        var year = period.Year;
        if (plan < 1)
        {
            plan = 4;
            year--;
        }

        return new Period(year, plan);
    }

    public static bool operator <(Period a, Period b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        return a.Year < b.Year || a.Year == b.Year && a.Plan < b.Plan;
    }

    public static bool operator >(Period a, Period b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        return a.Year > b.Year || a.Year == b.Year && a.Plan > b.Plan;
    }

    public static bool operator ==(Period a, Period b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        return a.Year == b.Year && a.Plan == b.Plan;
    }

    public static bool operator !=(Period a, Period b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        return a.Year != b.Year || a.Plan != b.Plan;
    }
}
