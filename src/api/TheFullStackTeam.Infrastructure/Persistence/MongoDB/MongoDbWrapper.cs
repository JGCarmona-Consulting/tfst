using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Polly;
using Polly.Timeout;
using TheFullStackTeam.Common.Configuration;
using TheFullStackTeam.Domain.Views;

namespace TheFullStackTeam.Infrastructure.Persistence.MongoDB;
public class MongoDbWrapper
{
    private readonly Lazy<IMongoDatabase?> _databaseLazy;
    private readonly ILogger<MongoDbWrapper> _logger;
    private readonly string _databaseName;
    private readonly MongoDbSettings _mongoSettings;
    private readonly AsyncPolicy _retryAndTimeoutPolicy;

    public MongoDbWrapper(IOptions<MongoDbSettings> options, ILogger<MongoDbWrapper> logger)
    {
        _logger = logger;
        _mongoSettings = options.Value;
        _databaseName = _mongoSettings.DatabaseName;

        // Sync Retry policy for lazy initialization of the database
        var syncRetryPolicyForLazy = Policy
            .Handle<Exception>()
            .WaitAndRetry(3, // retry 3 times
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // exponential backoff
                (exception, timeSpan, attempt, context) => // Callback
                {
                    _logger.LogWarning(exception, $"MongoDB retry {attempt} after {timeSpan.TotalSeconds} seconds: {exception.Message}");
                });

        // Async Retry policy
        var retryPolicyAsync = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, // retry 3 times
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                (exception, timeSpan, attempt, context) => // Callback
                {
                    _logger.LogWarning(exception, $"MongoDB async retry {attempt} after {timeSpan.TotalSeconds} seconds: {exception.Message}");
                    return Task.CompletedTask;
                });

        // Async Timeout policy
        var timeoutPolicyAsync = Policy
            .TimeoutAsync(5, // timeout after 5 seconds
                TimeoutStrategy.Pessimistic,
                (context, timespan, task) => // Callback
                {
                    _logger.LogWarning($"MongoDB Operation timed out after {timespan.TotalSeconds} seconds.");
                    return Task.CompletedTask;
                });

        // Wrap the retry and timeout policies
        _retryAndTimeoutPolicy = Policy.WrapAsync(retryPolicyAsync, timeoutPolicyAsync);

        _databaseLazy = new Lazy<IMongoDatabase?>(() =>
        {
            try
            {
                return syncRetryPolicyForLazy.Execute(() =>
                {
                    _logger.LogInformation("Initializing MongoDB database...");
                    var settings = MongoClientSettings.FromConnectionString(_mongoSettings.ConnectionString);
                    var mongoClient = new MongoClient(settings);
                    var database = mongoClient.GetDatabase(_databaseName);
                    _logger.LogInformation("MongoDB database initialized.");
                    return database;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize MongoDB database.");
                return null;
            }
        });
    }

    public IMongoDatabase? Database => _databaseLazy.Value;

    public bool IsDatabaseInitialized => _databaseLazy.IsValueCreated && _databaseLazy.Value != null;

    public async Task EnsureDatabaseIsReady()
    {
        if (!IsDatabaseInitialized)
        {
            _logger.LogWarning("MongoDB is not initialized. Skipping database readiness check.");
            return;
        }

        try
        {
            await _retryAndTimeoutPolicy.ExecuteAsync(async () =>
            {
                _logger.LogInformation("Pinging MongoDB...");
                var result = await Database!.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
                _logger.LogInformation("Successfully connected to MongoDB.");

                await InitializeIndexes();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MongoDB.");
        }
    }

    private async Task InitializeIndexes()
    {
        try
        {
            await _retryAndTimeoutPolicy.ExecuteAsync(async () =>
            {
                // Indexes AccountView
                var accountCollection = Database!.GetCollection<AccountView>("Accounts");
                var accountIndexKeys = Builders<AccountView>.IndexKeys
                    .Ascending(u => u.EntityId)
                    .Ascending(u => u.Version);
                var accountIndexModel = new CreateIndexModel<AccountView>(accountIndexKeys);
                await accountCollection.Indexes.CreateOneAsync(accountIndexModel);

                // Indexes UserProfileView
                var userProfileCollection = Database!.GetCollection<UserProfileView>("UserProfiles");
                var userProfileIndexKeys = Builders<UserProfileView>.IndexKeys
                    .Ascending(up => up.EntityId)
                    .Ascending(up => up.Version);
                var userProfileIndexModel = new CreateIndexModel<UserProfileView>(userProfileIndexKeys);
                await userProfileCollection.Indexes.CreateOneAsync(userProfileIndexModel);

                _logger.LogInformation("MongoDB indexes initialized successfully.");
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MongoDB indexes.");
        }
    }
}

