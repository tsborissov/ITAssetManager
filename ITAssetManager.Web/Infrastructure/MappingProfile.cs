using AutoMapper;
using ITAssetManager.Data.Models;
using ITAssetManager.Web.Models.Assets;
using ITAssetManager.Web.Models.Brands;
using ITAssetManager.Web.Models.Categories;
using ITAssetManager.Web.Models.Statuses;
using ITAssetManager.Web.Models.Vendors;
using ITAssetManager.Web.Services.AssetModels.Models;
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
            this.CreateMap<AssetAddFormServiceModel, Asset>();
            this.CreateMap<Asset, AssetEditFormServiceModel>();
            
            this.CreateMap<UserAsset, AssetCollectServiceModel>()
                .ForMember(ua => ua.Id, cfg => cfg.MapFrom(ua => ua.AssetId))
                .ForMember(ua => ua.Model, cfg => cfg.MapFrom(ua => ua.Asset.AssetModel.Name))
                .ForMember(ua => ua.SerialNr, cfg => cfg.MapFrom(ua => ua.Asset.SerialNr))
                .ForMember(ua => ua.InventoryNr, cfg => cfg.MapFrom(ua => ua.Asset.InventoryNr))
                .ForMember(ua => ua.AssignDate, cfg => cfg.MapFrom(ua => ua.AssignDate.ToLocalTime()))
                .ForMember(ua => ua.UserName, cfg => cfg.MapFrom(ua => ua.User.UserName));

            this.CreateMap<StatusAddFormServiceModel, Status>();
            this.CreateMap<StatusQueryServiceModel, StatusesQueryModel>();

            this.CreateMap<CategoryAddFormServiceModel, Category>();
            this.CreateMap<CategoryQueryServiceModel, CategoriesQueryModel>();

            this.CreateMap<BrandAddFormServiceModel, Brand>();
            this.CreateMap<BrandQueryServiceModel, BrandsQueryModel>();

            this.CreateMap<AssetModelsAddFormServiceModel, AssetModel>();
        }
    }
}
