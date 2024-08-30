using Microsoft.AspNetCore.Mvc;

namespace ABC_Retailers.Controllers
{
    public class AboutUsController : Controller
    {
        //controller to display about page
        public IActionResult AboutUs()
        {
            return View("~/Views/AboutUs/AboutUs.cshtml");
        }
    }
}
