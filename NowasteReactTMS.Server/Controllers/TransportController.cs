using Microsoft.AspNetCore.Mvc;

namespace NowasteReactTMS.Server.Controllers
{
    public class TransportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
