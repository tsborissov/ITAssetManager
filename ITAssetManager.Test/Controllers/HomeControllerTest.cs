using ITAssetManager.Web.Controllers;
using MyTested.AspNetCore.Mvc;
using Xunit;

namespace ITAssetManager.Test.Controllers
{
    public class HomeControllerTest
    {
        [Fact]
        public void ErrorShouldReturnView()
            => MyMvc
                .Pipeline()
                .ShouldMap("/Home/Error")
                .To<HomeController>(c => c.Error())
                .Which()
                .ShouldReturn()
                .View();
    }
}
