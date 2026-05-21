namespace mbt.webapi.Configuration;

public interface ISmtpSettings
{
    string Host { get; set; }
    string From { get; set; }
    int Port { get; set; }
}

public class SmtpSettings : ISmtpSettings
{
    public string Host { get; set; }
    public string From { get; set; }
    public int Port { get; set; }
}
