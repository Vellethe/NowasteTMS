using Microsoft.AspNetCore.Mvc;

namespace NowasteReactTMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrencyRepository _currencyRepo;
        public CustomerController(ICustomerRepository customerRepository, ICurrencyRepository currencyRepo)
        {
            _customerRepository = customerRepository;
            _currencyRepo = currencyRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            var customer = await _customerRepository.GetCustomer(id);

            return Ok(customer);
        }
    }
}
