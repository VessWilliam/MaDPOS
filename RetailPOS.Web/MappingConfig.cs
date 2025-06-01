using Mapster;
using RetailPOS.Web.Models;

namespace RetailPOS.Web;

public static class MappingConfig
{

    public static void RegisterMapping()
    {
        TypeAdapterConfig<RegisterViewModel, ApplicationUser>
            .NewConfig()
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.EmailConfirmed, src => true);

    }

}
