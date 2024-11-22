
using DotNetEnv;
using TheFullStackTeam.Common.Configuration;
using TheFullStackTeam.Common.Converters;
using TheFullStackTeam.Domain.Services;
using TheFullStackTeam.Infrastructure.Extensions;
using TheFullStackTeam.Infrastructure.Persistence.MongoDB.Extensions;
using TheFullStackTeam.Infrastructure.Persistence.Sql.Extensions;
using TheFullStackTeam.Infrastructure.Persistence.Sql.Initialization;
using TheFullStackTeam.Infrastructure.Persistence.Sql.Services;
using TheFullStackTeam.Infrastructure.Services;
using Serilog;
using System.Text.Json;
using TheFullStackTeam.Api.Extensions;
using TheFullStackTeam.Api.Middlewares;

namespace TheFullStackTeam.Api
{

    public class Program
    {
        private static async Task Main(string[] args)
        {
            // Load environment variables from .env file
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            // Add secrets provider extension here before other services
            builder.Services.AddCustomSecrets(builder.Configuration);

            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            // Configure feature flags and specific settings
            builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings"));
            builder.Services.Configure<FeatureFlags>(builder.Configuration.GetSection("FeatureFlags"));
            builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));
            builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


            builder.Host.UseSerilog((context, loggerConfig) =>
                loggerConfig.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddRouting(options => options.LowercaseUrls = true);

            // Configure services
            builder.Services.AddDatabaseConfiguration(builder.Configuration);
            builder.Services.AddMongoDb(builder.Configuration);
            builder.Services.AddCommandRepositories();
            builder.Services.AddQueryRepositories();
            builder.Services.AddMediatRConfiguration();
            builder.Services.AddSwaggerConfiguration();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddEventDispatcher(builder.Configuration);

            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IMonikerService, MonikerService>();
            builder.Services.AddScoped<DatabaseInitializer>();
            builder.Services.AddSingleton(new JsonSerializerOptions
            {
                Converters = { new UlidJsonConverter() }
            });

            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            var app = builder.Build();

            // Add Middlewares
            app.UseMiddleware<RequestContextLoggingMiddleware>();
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            await app.Services.InitializeDatabaseAsync(builder.Configuration);

            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TheFullStackTeam API v1");
                c.RoutePrefix = "swagger";
                c.OAuthUsePkce();
            });
            //}

            //app.MapGet("/", context =>
            //{
            //    context.Response.Redirect("/swagger");
            //    return Task.CompletedTask;
            //});

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}