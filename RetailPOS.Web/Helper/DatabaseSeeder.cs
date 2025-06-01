using RetailPOS.Web.Data;

namespace RetailPOS.Web.Helper;

public class DatabaseSeeder
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public DatabaseSeeder(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = loggerFactory.CreateLogger<DatabaseSeeder>();
    }

    public async Task SeedAync()
    {
        try
        {
            await DbInitializer.InitializeAsync(_serviceProvider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error while seeding database.");
        }
    }
}
