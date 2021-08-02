using AutoMapper;
using ITAssetManager.Web.Services.Vendors.Models;
using ITAssetManager.Web.Models.Vendors;
using ITAssetManager.Web.Services.Assets.Models;
using ITAssetManager.Web.Models.Assets;

namespace ITAssetManager.Web.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<VendorDetailsServiceModel, VendorEditServiceModel>().ReverseMap();
            this.CreateMap<VendorQueryServiceModel, VendorsQueryModel>();
            this.CreateMap<AssetsQueryServiceModel, AssetsQueryModel>();
        }
    }
}
