namespace mbt.webapi.Configuration;

public class AzureConfigOptions
{
    public static readonly string SectionName = "AzureAd";

    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string TenantId { get; set; }
    public string Instance { get; set; }
}
