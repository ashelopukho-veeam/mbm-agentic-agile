namespace mbt.webapi.Configuration;

public class MbtDatabaseSettings : IMbtDatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}


public interface IMbtDatabaseSettings
{
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
}
