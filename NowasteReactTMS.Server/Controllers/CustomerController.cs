using Microsoft.AspNetCore.Mvc;
using NowasteReactTMS.Server.Models;
using NowasteTms.Model;

namespace NowasteReactTMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly ICurrencyRepository _currencyRepo;
        public CustomerController(ICustomerRepository customerRepository, ICurrencyRepository currencyRepo)
        {
            _customerRepo = customerRepository;
            _currencyRepo = currencyRepo;
        }
        /// <summary>
        /// Display one Customers details by searching on their PK
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        [HttpGet("{pk}")]
        public async Task<IActionResult> GetCustomer(Guid pk)
        {
            var customer = await _customerRepo.GetCustomer(pk);

            return Ok(customer);
        }

        /// <summary>
        /// Displays all active Customers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetAllCustomers()
        {
            try
            {
                var customers = await _customerRepo.GetCustomers();
                return Ok(customers);
            }
            catch
            {
                return StatusCode(500, "Internal server error");

            }
        }
        /// <summary>
        /// Creates a new Customer with the needed parameters
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
            public async Task<IActionResult> CreateCustomer([FromBody] CustomerDTO dto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Generate GUIDs
                dto.CustomerPK = Guid.NewGuid();
                dto.BusinessUnit.BusinessUnitPK = Guid.NewGuid();
                dto.BusinessUnit.FinanceInformationPK = Guid.NewGuid();

                // Map ViewModel to Entity
                var customer = new Customer
                {
                    CustomerID = dto.CustomerID,
                    CustomerPK = dto.CustomerPK,
                    BusinessUnitPK = dto.BusinessUnitPK,
                    BusinessUnit = new BusinessUnit
                    {
                        Name = dto.BusinessUnit.Name,
                        Company = dto.BusinessUnit.Company,
                        BusinessUnitPK = dto.BusinessUnit.BusinessUnitPK,
                        FinanceInformation = new FinanceInformation
                        {
                            FinanceInformationPK = dto.BusinessUnit.FinanceInformationPK,
                            CurrencyPK = dto.BusinessUnit.FinanceInformation.Currency.CurrencyPK,
                            VAT = dto.BusinessUnit.FinanceInformation.VAT
                        },
                        ContactInformations = dto.BusinessUnit.ContactInformations?.Select(c => new ContactInformation
                        {
                            BusinessUnitPK = dto.BusinessUnitPK,
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
                        FinanceInformationPK = dto.BusinessUnit.FinanceInformationPK,
                        IsEditable = dto.BusinessUnit.IsEditable
                    }
                };

                try
                {
                    await _customerRepo.CreateCustomer(customer);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }

                return Ok(customer.CustomerPK); // Return the created customer's PK
            }
        /// <summary>
        /// Updates an existing Customer and makes sure the required parameters are valid
        /// </summary>
        /// <param name="customerViewModel"></param>
        /// <param name="pk"></param>
        /// <returns></returns>
        [HttpPut("{pk}")]
        public async Task<IActionResult> EditCustomer(CustomerDTO customerViewModel, Guid pk)
        {
            var ci = new List<ContactInformation>();

            if (customerViewModel.BusinessUnit?.ContactInformations != null)
            {
                foreach (var c in customerViewModel.BusinessUnit.ContactInformations)
                {
                    if (c != null)
                    {
                        ci.Add(new ContactInformation
                        {
                            BusinessUnitPK = c.BusinessUnitPK,
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
                            IsActive = c.IsActive,
                            ExternalId = c.ExternalId,
                            Description = c.Description,
                            IsEditable = c.IsEditable
                        });
                    }
                }
            }

            var customer = new Customer
            {
                CustomerPK = customerViewModel.CustomerPK,
                CustomerID = customerViewModel.CustomerID,
                BusinessUnitPK = customerViewModel.BusinessUnitPK,
                BusinessUnit = new BusinessUnit
                {
                    Name = customerViewModel.BusinessUnit.Name,
                    IsEditable = customerViewModel.BusinessUnit.IsEditable,
                    Company = customerViewModel.BusinessUnit.Company,
                    BusinessUnitPK = customerViewModel.BusinessUnit.BusinessUnitPK,
                    FinanceInformation = new FinanceInformation
                    {
                        FinanceInformationPK = customerViewModel.BusinessUnit.FinanceInformationPK,
                        CurrencyPK = customerViewModel.BusinessUnit.FinanceInformation.Currency.CurrencyPK,
                        VAT = customerViewModel.BusinessUnit.FinanceInformation.VAT
                    },
                    ContactInformations = ci.AsEnumerable(),
                    FinanceInformationPK = customerViewModel.BusinessUnit.FinanceInformationPK
                }
            };

            await _customerRepo.UpdateCustomer(pk, customer);

            return Ok(new { message = "Edit successful" });
        }
        /// <summary>
        /// Deletes a existing Customer (put isActive status to 0)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{pk}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                await _customerRepo.DeleteCustomer(id);
                return Ok("Customer set to 0, deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while deleting the customer: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the customer.");
            }
        }
    }
}