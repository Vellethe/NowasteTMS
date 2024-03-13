using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NowasteReactTMS.Server.Models;
using NowasteTms.Model;

namespace NowasteReactTMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : Controller
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICurrencyRepository _currencyRepo;

        public SupplierController(ISupplierRepository supplierRepository, ICurrencyRepository currencyRepo)
        {
            _supplierRepository = supplierRepository;
            _currencyRepo = currencyRepo;
        }
        /// <summary>
        /// Displays an active SUpplier when searching for their PK
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        [HttpGet("{pk}")]
        public async Task<IActionResult> GetSupplier(Guid pk)
        {
            var supplier = await _supplierRepository.GetSupplier(pk);
            return Ok(supplier);
        }

        /// <summary>
        /// Displays all active Suppliers
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SearchSuppliers(SearchOrderDTO dto)

        {
            var searchParameters = new SearchParameters
            {
                Limit = dto.Size,
                Offset = dto.Page * dto.Size,
                Filters = dto.Filter,
                SortOrders = dto.Column
            };

            var suppliers = await _supplierRepository.SearchSuppliers(searchParameters);

            return Ok(suppliers);
        }
        /// <summary>
        /// Creates a new Supplier with the required parameters
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            dto.SupplierPK = Guid.NewGuid();
            dto.BusinessUnit.BusinessUnitPK = Guid.NewGuid();
            dto.BusinessUnitPK = dto.BusinessUnit.BusinessUnitPK;
            dto.BusinessUnit.FinanceInformationPK = Guid.NewGuid();

            var currencies = await _currencyRepo.GetAll();
            var currencySEK = currencies.FirstOrDefault(x => x.ShortName == "SEK");

            dto.BusinessUnit.FinanceInformation = new CustomerFinanceInformation
            {
                Currency = currencySEK,
                Currencies = currencies,
                VAT = ""
            };

            // Clear error state for generated GUID fields.
            ModelState.Remove("SupplierPK");
            ModelState.Remove("BusinessUnit.BusinessUnitPK");
            ModelState.Remove("BusinessUnit.FinanceInformation.FinanceInformationPK");
            ModelState.Remove("BusinessUnit.DefaultContactInformation.ContactInformationPK");

            var supplier = MapToSupplier(dto);

            await _supplierRepository.CreateSupplier(supplier);

            return Ok("Supplier created successfully.");
        }

        private Supplier MapToSupplier(SupplierDTO dto)
        {
            var ci = dto.BusinessUnit.ContactInformations?
                .Select(c => new ContactInformation
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
                    IsEditable = c.IsEditable
                })
                .ToList();

            return new Supplier
            {
                SupplierID = dto.SupplierID,
                SupplierPK = dto.SupplierPK,
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
                    ContactInformations = ci,
                    FinanceInformationPK = dto.BusinessUnit.FinanceInformationPK,
                    IsEditable = dto.BusinessUnit.IsEditable
                }
            };
        }

        /// <summary>
        /// Updates an existing Supplier and makes sure the required parameters are valid
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="pk"></param>
        /// <returns></returns>
        [HttpPut("{pk}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SupplierDTO dto, Guid pk)
        {
            List<ContactInformation> ci = new List<ContactInformation>();

            var currencies = await _currencyRepo.GetAll();
            var currencySEK = currencies.FirstOrDefault(x => x.ShortName == "SEK");

            dto.BusinessUnit.FinanceInformation = new CustomerFinanceInformation
            {
                Currency = currencySEK,
                Currencies = currencies,
                VAT = ""
            };

            if (null != dto.BusinessUnit.ContactInformations)
            {
                foreach (var c in dto.BusinessUnit.ContactInformations)
                {
                    if (null != c)
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
                            IsEditable = c.IsEditable,
                        });
                    }
                }
            }

            var supplier = new Supplier
            {
                SupplierPK = dto.SupplierPK,
                SupplierID = dto.SupplierID,
                BusinessUnitPK = dto.BusinessUnitPK,
                BusinessUnit = new BusinessUnit
                {
                    Name = dto.BusinessUnit.Name,
                    IsEditable = dto.BusinessUnit.IsEditable,
                    Company = dto.BusinessUnit.Company,
                    BusinessUnitPK = dto.BusinessUnit.BusinessUnitPK,
                    FinanceInformation = new FinanceInformation
                    {
                        FinanceInformationPK = dto.BusinessUnit.FinanceInformationPK,
                        CurrencyPK = dto.BusinessUnit.FinanceInformation.Currency.CurrencyPK,
                        VAT = dto.BusinessUnit.FinanceInformation.VAT
                    },
                    ContactInformations = ci.AsEnumerable(),
                    FinanceInformationPK = dto.BusinessUnit.FinanceInformationPK
                }
            };

            await _supplierRepository.UpdateSupplier(pk, supplier);

            return Ok("Edit successful");
        }
        /// <summary>
        /// Deletes a existing Supplier (put isActive status to 0)
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        [HttpDelete("{pk}")]
        public async Task<IActionResult> Delete(Guid pk)
            {
                try
                {
                    await _supplierRepository.DeleteSupplier(pk);
                return Ok("Supplier set to 0, deleted successfully.");
            }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting the customer: {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the customer.");
                }
            }

    }
}
