using System.Data;
namespace RetailPOS.Web.Services.IService;
public interface IDapperBaseService
{ 
    Task<T> getDBConnectionAsync<T>(Func<IDbConnection, Task<T>> getData);
}
