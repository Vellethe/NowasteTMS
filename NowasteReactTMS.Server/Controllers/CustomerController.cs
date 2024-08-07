﻿using Microsoft.AspNetCore.Mvc;
using NowasteReactTMS.Server.Models;
using NowasteReactTMS.Server.Models.CustomerDTOs;
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

        [HttpGet("CustomerList")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customer = await _customerRepo.GetCustomerList();

            return Ok(customer);
        }

        /// <summary>
        /// Displays all active Customers
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetAllCustomers(SearchDTO dto)

        {
            var searchParameters = new SearchParameters
            {
                Limit = dto.Size,
                Offset = dto.Page * dto.Size,
                Filters = dto.Filter,
                SortOrders = dto.Column
            };

            var customers = await _customerRepo.SearchCustomers(searchParameters);

            return Ok(customers);
        }
        /// <summary>
        /// Creates a new Customer with the needed parameters
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("create")]
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
        public async Task<IActionResult> EditCustomer(Guid pk, [FromBody] Customer updatedCustomer)
        {
            try
            {
                if (pk != updatedCustomer.CustomerPK)
                {
                    return BadRequest("Customer ID in the URL does not match the one in the request body.");
                }

                    updatedCustomer.BusinessUnit.Company = "1";
                    updatedCustomer.BusinessUnit.FinanceInformation.VAT = null;

                // Update the customer information in the repository
                await _customerRepo.UpdateCustomer(pk, updatedCustomer);

                return Ok(new { message = "Edit successful" });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while updating the customer: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the customer.");
            }
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