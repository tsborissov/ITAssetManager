using ITAssetManager.Data;
using ITAssetManager.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ITAssetManager.Web.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/vendors")]
    public class VendorsApiController : ControllerBase
    {
        private readonly AppDbContext data;

        public VendorsApiController(AppDbContext data)
        {
            this.data = data;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Vendor>> GetVendors()
        {
            var vendors = this.data.Vendors.ToList();
            
            if (!vendors.Any())
            {
                return NotFound();
            }

            return vendors;
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<Vendor> GetDetails(int id)
        {
            var vendor = this.data.Vendors.Find(id);

            if (vendor == null)
            {
                return NotFound();
            }

            return vendor;
        }
    }
}
