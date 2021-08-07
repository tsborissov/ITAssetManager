using ITAssetManager.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ITAssetManager.Web.Areas.Admin.Controllers
{
    public class HomeController : AdminController
    {
        public IActionResult Index() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
