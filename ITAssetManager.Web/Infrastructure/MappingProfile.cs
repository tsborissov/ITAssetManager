using AutoMapper;
using ITAssetManager.Web.Services.Vendors.Models;
using ITAssetManager.Web.Models.Vendors;
using ITAssetManager.Web.Services.Assets.Models;
using ITAssetManager.Web.Models.Assets;
using ITAssetManager.Data.Models;

namespace ITAssetManager.Web.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<VendorDetailsServiceModel, VendorEditServiceModel>().ReverseMap();
            this.CreateMap<VendorQueryServiceModel, VendorsQueryModel>();
            this.CreateMap<AssetsQueryServiceModel, AssetsQueryModel>();
            this.CreateMap<VendorAddFormServiceModel, Vendor>();
        }
    }
}
