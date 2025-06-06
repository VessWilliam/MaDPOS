using Mapster;
using RetailPOS.Web.Models;
using RetailPOS.Web.Models.ViewModel;

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

        TypeAdapterConfig<ProductViewModel, Product>
              .NewConfig()
              .Ignore(dest => dest.Id)
              .Ignore(dest => dest.Category!)
              .Ignore(dest => dest.CreatedAt)
              .Ignore(dest => dest.UpdatedAt!)
              .Ignore(dest => dest.Items)
              .Ignore(dest => dest.PriceHistory);

    }

}
