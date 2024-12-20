namespace TheFullStackTeam.Common.Configuration;

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "TheFullStackTeam";
    public string Audience { get; set; } = "TheFullStackTeam.Users";
    public double TokenLifetimeInHours { get; set; }
    public double RefreshTokenLifetimeInDays { get; set; }
}
