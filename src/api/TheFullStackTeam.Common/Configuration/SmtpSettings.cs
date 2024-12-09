namespace TheFullStackTeam.Common.Configuration;

public class SmtpSettings
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public string? SenderEmail { get; set; }
    public string? SenderName { get; set; }
}
