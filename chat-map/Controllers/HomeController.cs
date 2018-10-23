using Microsoft.AspNetCore.Mvc;

namespace ChatMap.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}