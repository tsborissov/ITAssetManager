using AutoMapper;
using ITAssetManager.Web.Services.Vendors.Models;
using ITAssetManager.Web.Models.Vendors;
using ITAssetManager.Web.Services.Assets.Models;
using ITAssetManager.Web.Models.Assets;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Services.Statuses.Models;
using ITAssetManager.Web.Services.Categories.Models;

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
            this.CreateMap<StatusAddFormServiceModel, Status>();
            this.CreateMap<CategoryAddFormServiceModel, Category>();
        }
    }
}
