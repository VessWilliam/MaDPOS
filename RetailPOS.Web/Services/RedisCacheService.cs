using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RetailPOS.Web.Services.IService;

namespace RetailPOS.Web.Services;

public class RedisCacheService : IRedisCacheService
{

    private readonly IDistributedCache? _cache;

    public RedisCacheService(IDistributedCache cache) => _cache = cache;
    

    public async Task<T?> GetData<T>(string key)
    {
        var data = await _cache!.GetStringAsync(key);


        if (data is null) return default(T);   

        return JsonConvert.DeserializeObject<T>(data);  

    }

    public async Task RemoveData(string key)
    {
       await _cache!.RemoveAsync(key);
    }

    public async Task SetData<T>(string key, T data, TimeSpan? expire = null)
    {
        var json = JsonConvert.SerializeObject(data); 

        var opt = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = expire ?? TimeSpan.FromMinutes(30),
        };


        await _cache!.SetStringAsync(key, json, opt);
    }
}
