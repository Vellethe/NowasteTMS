using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NowasteReactTMS.Server.Models.TransportDTOs;
using NowasteReactTMS.Server.Repositories.Interface;
using NowasteTms.Model;

namespace NowasteReactTMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController : Controller
    {
        private readonly ITransportZonePriceRepository _transportZonePriceRepo;
        private readonly ITransportZoneRepository _transportZoneRepo;
        private readonly ITransportZonePriceLineRepository _transportZonePriceLineRepo;
        private readonly ITransportTypeRepository _transportTypeRepo;
        private readonly IAgentRepository _agentRepo;
        private readonly ICurrencyRepository _currencyRepo;
        private readonly IPalletInventoryRepository _inventoryRepo;

        public PriceController(ITransportZonePriceRepository transportZonePriceRepo,
            ITransportZoneRepository transportZoneRepo,
            ITransportZonePriceLineRepository transportZonePriceLineRepo,
            IAgentRepository agentRepo,
            ITransportTypeRepository transportTypeRepo,
            ICurrencyRepository currencyRepo,
            IPalletInventoryRepository inventoryRepo)
        {
            _transportZonePriceRepo = transportZonePriceRepo;
            _transportZonePriceLineRepo = transportZonePriceLineRepo;
            _transportZoneRepo = transportZoneRepo;
            _transportTypeRepo = transportTypeRepo;
            _agentRepo = agentRepo;
            _currencyRepo = currencyRepo;
            _inventoryRepo = inventoryRepo;
        }

        [HttpGet("{pk}")]
        public async Task<IActionResult> GetPrice(Guid pk)
        {
            var price = await _transportZonePriceRepo.Get(pk);

            return Ok(price);
        }

        [HttpPost]
        public async Task<IActionResult> GetAllTransportZonePrices()
        {
            try
            {
                var agents = await _agentRepo.GetAgents();
                var prices = await _transportZonePriceRepo.GetAll();
                prices = prices.Where(x => agents.FirstOrDefault(y => y.AgentPK == x.AgentPK) != null).ToList();
                var transportZones = await _transportZoneRepo.GetAll();
                var transportTypes = await _transportTypeRepo.GetAll();

                // Create a combined array
                var combinedData = prices.Select(p => new
                {
                    Price = p,
                    TransportZone = transportZones.FirstOrDefault(tz => tz.TransportZonePK == p.ToTransportZonePK),
                    TransportType = transportTypes.FirstOrDefault(tt => tt.TransportTypePK == p.TransportTypePK),
                    Agent = agents.FirstOrDefault(a => a.AgentPK == p.AgentPK)
                }).ToArray();

                return Ok(combinedData);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(TransportPriceDTO dto)
        {

            dto.TransportZonePricePK = Guid.NewGuid();
            await _transportZonePriceRepo.Add(dto);

            return Ok("Created successfully");
        }

        [HttpPut("{pk}")]
        public async Task<IActionResult> Edit(Guid pk, TransportPriceDTO dto)
        {
            try
            {
                var updatePrice = await _transportZonePriceRepo.Get(pk);

                if (updatePrice == null)
                {
                    return NotFound("Price not found");
                }

                updatePrice.Price = dto.Price;
                updatePrice.Description = dto.Description;
                updatePrice.ValidFrom = dto.ValidFrom;
                updatePrice.ValidTo = dto.ValidTo;

                if (updatePrice.TransportType.Description == "Groupage")
                {
                    updatePrice.Price = updatePrice.TransportZonePriceLines.Sum(line => line.Price);
                }

                await _transportZonePriceRepo.Update(pk, updatePrice);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating price: {ex.Message}");
            }
        }



        [HttpDelete("{pk}")]
        public async Task<IActionResult> Delete(Guid pk)
        {
            await _transportZonePriceRepo.Remove(pk);
            return Ok("Deleted successfully");
        }
    }
}
