namespace TheFullStackTeam.API;

public static class Constants
{
    public const string AppName = "The  Full Stack Team.Api";
    public const string Title = "The Full Stack Team REST API";
    public const string Description = @"
Welcome to the The Full Stack Team API documentation!

This API provides... 

ETCETERA...

Happy coding!
";
    public static readonly Dictionary<string, string> Contact = new()
    {
        { "name", "Juan García Carmona" },
        { "url", "https://jgcarmona.com/contact" },
        { "email", "juan@jgcarmona.com" }
    };

    public static readonly Dictionary<string, string> LicenseInfo = new()
    {
        { "name", "Apache License 2.0" },
        { "url", "https://www.apache.org/licenses/LICENSE-2.0" }
    };

    public static readonly Dictionary<string, string> SwaggerUiParameters = new()
    {
        { "syntaxHighlight.theme", "obsidian" },
        { "docExpansion", "none" }, // Collapse all sections by default
        { "persistAuthorization", "true" } // Preserve authorization across sessions
    };

    public const string SwaggerFaviconUrl = "https://jgcarmona.com/assets/favicon.ico";
}
