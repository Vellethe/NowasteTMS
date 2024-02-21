using Microsoft.AspNetCore.Mvc;

namespace NowasteReactTMS.Server.Controllers
{
    public class OrderController : Controller
    {
        private readonly IPalletReceiptRepository palletReceiptRepository;

        public OrderController(IPalletReceiptRepository palletReceiptRepository)
        {
            this.palletReceiptRepository = palletReceiptRepository;
        }
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
