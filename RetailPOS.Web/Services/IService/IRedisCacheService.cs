namespace RetailPOS.Web.Services.IService;

public interface IRedisCacheService
{
    Task<T?> GetData<T>(string key);

    Task SetData<T>(string key, T data, TimeSpan? expire);

    public Task RemoveData(string key); 

}
