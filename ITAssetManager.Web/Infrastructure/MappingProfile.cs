using AutoMapper;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models.Assets;
using ITAssetManager.Web.Models.Brands;
using ITAssetManager.Web.Models.Categories;
using ITAssetManager.Web.Models.Statuses;
using ITAssetManager.Web.Models.Vendors;
using ITAssetManager.Web.Services.Assets.Models;
using ITAssetManager.Web.Services.Brands.Models;
using ITAssetManager.Web.Services.Categories.Models;
using ITAssetManager.Web.Services.Statuses.Models;
using ITAssetManager.Web.Services.Vendors.Models;

namespace ITAssetManager.Web.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<VendorDetailsServiceModel, VendorEditServiceModel>().ReverseMap();
            this.CreateMap<VendorQueryServiceModel, VendorsQueryModel>();
            this.CreateMap<VendorAddFormServiceModel, Vendor>();
            this.CreateMap<AssetsQueryServiceModel, AssetsQueryModel>();
            this.CreateMap<StatusAddFormServiceModel, Status>();
            this.CreateMap<StatusQueryServiceModel, StatusesQueryModel>();
            this.CreateMap<CategoryAddFormServiceModel, Category>();
            this.CreateMap<CategoryQueryServiceModel, CategoriesQueryModel>();
            this.CreateMap<BrandAddFormServiceModel, Brand>();
            this.CreateMap<BrandQueryServiceModel, BrandsQueryModel>();
        }
    }
}
