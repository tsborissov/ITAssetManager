using AutoMapper;
using ITAssetManager.Web.Services.Vendors.Models;
using ITAssetManager.Web.Models.Vendors;

namespace ITAssetManager.Web.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<VendorDetailsServiceModel, VendorEditServiceModel>().ReverseMap();
            this.CreateMap<VendorQueryServiceModel, VendorsQueryModel>();

        }
    }
}
