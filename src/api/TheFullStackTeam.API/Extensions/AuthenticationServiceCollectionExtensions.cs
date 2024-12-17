using TheFullStackTeam.Common.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TheFullStackTeam.API.Extensions;

public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.GetSection("JwtSettings").Bind(jwtSettings);

        if (string.IsNullOrEmpty(jwtSettings.Key))
        {
            throw new ArgumentNullException(nameof(jwtSettings.Key), "JWT Key is not configured properly.");
        }

        var keyBytes = Encoding.ASCII.GetBytes(jwtSettings.Key);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
                };
            });
        return services;
    }
}

