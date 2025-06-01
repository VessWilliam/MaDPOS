using Npgsql;
using RetailPOS.Web.Services.IService;
using System.Data;

namespace RetailPOS.Web.Services;

public class DapperBaseService : IDapperBaseService
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public DapperBaseService(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _configuration = configuration; 
        _logger = loggerFactory.CreateLogger<DapperBaseService>();  
    }

    public async Task<T> getDBConnectionAsync<T>(Func<IDbConnection, Task<T>> getData)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection")
                                 ?? throw new InvalidOperationException("Dapper connection string not found.");


            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            return await getData(connection); 
        }
        catch (TimeoutException ex)
        {
           _logger.LogError($"Error get connection with Dapper Error : {ex} ");
           throw new Exception($"{GetType().FullName}.getDBConnection() experienced a NpgSQL timeout, {ex}");
        }
    }
}
