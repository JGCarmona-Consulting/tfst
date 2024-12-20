using System.Text.Json.Serialization;

namespace TheFullStackTeam.Application.Auth.Models;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "bearer";

    [JsonPropertyName("refresh-token")]
    public string RefreshToken { get; set; } = string.Empty;
    
}