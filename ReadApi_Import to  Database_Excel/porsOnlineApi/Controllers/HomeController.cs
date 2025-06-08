using Microsoft.AspNetCore.Mvc;

namespace porsOnlineApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
