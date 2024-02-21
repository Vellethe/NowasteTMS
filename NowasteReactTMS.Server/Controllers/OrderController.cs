using Microsoft.AspNetCore.Mvc;

namespace NowasteReactTMS.Server.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
