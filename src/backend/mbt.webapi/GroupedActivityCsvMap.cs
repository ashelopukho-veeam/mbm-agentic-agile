using CsvHelper.Configuration;
using JetBrains.Annotations;
using GroupedActivity = mbt.webapi.Domain.Entities.GroupedActivity;

namespace mbt.webapi;

[PublicAPI]
public sealed class GroupedActivityCsvMap : ClassMap<GroupedActivity>
{
    public GroupedActivityCsvMap()
    {
        Map(ga => ga.Title).Name("Grouped Activity Name");
        Map(ga => ga.Quarter).Convert(r => int.Parse(r.Row.GetField("Quarter")[1..]));
        Map(ga => ga.PlannedAmount).Name("Planned Amount");
        Map(ga => ga.PlannedSponsorship).Name("Planned Sponsorship");
        Map(ga => ga.LocalCurrency).Name("Local Currency");
        Map(ga => ga.Comment).Name("Comment");
        Map(ga => ga.GlobalRegion).Name("Global Region");
        Map(ga => ga.SubRegion).Name("Subregion");
        Map(ga => ga.Alliance).Name("Alliance Name");
        Map(ga => ga.ChannelDetails).Name("Channel Details");
        Map(ga => ga.Vendors).Name("Vendor"); // Convert(r => new List<string>() {r.Row.GetField("Vendor")});
        Map(ga => ga.AdService).Name("AdService");
        Map(ga => ga.ExecutionTeam).Name("Execution Team");
        Map(ga => ga.ContentType).Name("Content Type");
        Map(ga => ga.GlobalProgram).Name("Global Program");
        Map(ga => ga.Management).Name("Management");
        Map(ga => ga.Team).Name("Team");
        Map(ga => ga.Segments).TypeConverter<ToTitleNumberValuePairArrayConverter>();
        Map(ga => ga.Campaigns).TypeConverter<ToTitleNumberValuePairArrayConverter>();
    }
}
