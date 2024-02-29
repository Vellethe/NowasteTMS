using Microsoft.AspNetCore.Mvc;
using NowasteReactTMS.Server.Models;
using NowasteTms.Model;

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

        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                await _customerRepository.DeleteCustomer(id);
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while deleting the customer: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the customer.");
            }
        }

        [HttpPost]
            public async Task<IActionResult> CreateCustomer([FromBody] CustomerDTO customerViewModel)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Generate GUIDs
                customerViewModel.CustomerPK = Guid.NewGuid();
                customerViewModel.BusinessUnit.BusinessUnitPK = Guid.NewGuid();
                customerViewModel.BusinessUnit.FinanceInformationPK = Guid.NewGuid();

                // Map ViewModel to Entity
                var customer = new Customer
                {
                    CustomerID = customerViewModel.CustomerID,
                    CustomerPK = customerViewModel.CustomerPK,
                    BusinessUnitPK = customerViewModel.BusinessUnitPK,
                    BusinessUnit = new BusinessUnit
                    {
                        Name = customerViewModel.BusinessUnit.Name,
                        Company = customerViewModel.BusinessUnit.Company,
                        BusinessUnitPK = customerViewModel.BusinessUnit.BusinessUnitPK,
                        FinanceInformation = new FinanceInformation
                        {
                            FinanceInformationPK = customerViewModel.BusinessUnit.FinanceInformationPK,
                            CurrencyPK = customerViewModel.BusinessUnit.FinanceInformation.Currency.CurrencyPK,
                            VAT = customerViewModel.BusinessUnit.FinanceInformation.VAT
                        },
                        ContactInformations = customerViewModel.BusinessUnit.ContactInformations?.Select(c => new ContactInformation
                        {
                            BusinessUnitPK = customerViewModel.BusinessUnitPK,
                            IsDefault = c.IsDefault,
                            References = c.References,
                            ContactInformationPK = c.ContactInformationPK,
                            Email = c.Email,
                            Phone = c.Phone,
                            CellularPhone = c.CellularPhone,
                            Fax = c.Fax,
                            Address = c.Address,
                            Zipcode = c.Zipcode,
                            City = c.City,
                            Country = c.Country,
                            Description = c.Description,
                            IsEditable = c.IsEditable,
                        }).ToList(),
                        FinanceInformationPK = customerViewModel.BusinessUnit.FinanceInformationPK,
                        IsEditable = customerViewModel.BusinessUnit.IsEditable
                    }
                };

                try
                {
                    await _customerRepository.CreateCustomer(customer);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }

                return Ok(customer.CustomerPK); // Return the created customer's PK
            }
        }
    }