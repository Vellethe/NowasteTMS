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
        public async Task<IActionResult> Create(CreateTransportZonePriceDTO dto)
        {
            dto.TransportZonePrice.TransportZonePricePK = Guid.NewGuid();

            if (dto.TransportZonePrice.TransportZonePriceLines != null)
            {
                var sumTotal = dto.TransportZonePrice.TransportZonePriceLines.Sum(line => line.Price);

                if (sumTotal <= 0)
                {
                    sumTotal = dto.TransportZonePrice.Price;
                }

                dto.TransportZonePrice.Price = sumTotal;
            }

            await _transportZonePriceRepo.Add(dto.TransportZonePrice);

            if (dto.TransportZonePrice.TransportZonePriceLines != null)
            {
                foreach (var lines in dto.TransportZonePrice.TransportZonePriceLines)
                {
                    lines.TransportZonePricePK = dto.TransportZonePrice.TransportZonePricePK;
                    lines.TransportZonePriceLinePK = Guid.NewGuid();
                    await _transportZonePriceLineRepo.Add(lines);
                }
            }

            return Ok("Created successfully");
        }

        [HttpPut("{pk}")]
        public async Task<IActionResult> Edit(TransportPriceDTO dto)
        {
            var updatePrice = await _transportZonePriceRepo.Get(dto.TransportZonePricePK);
            updatePrice.Price = dto.Price;
            updatePrice.Description = dto.Description;
            updatePrice.ValidFrom = dto.ValidFrom;
            updatePrice.ValidTo = dto.ValidTo;

            if (updatePrice.TransportType.Description == "Groupage")
            {
                updatePrice.Price = updatePrice.TransportZonePriceLines.Sum(line => line.Price);
                updatePrice.TransportZonePriceLines = updatePrice.TransportZonePriceLines.Select(line => new TransportZonePriceLine
                {
                    TransportZonePriceLinePK = line.TransportZonePriceLinePK,
                    Price = line.Price
                }).ToList();
            }
            await _transportZonePriceRepo.Update(updatePrice.TransportZonePriceLine.TransportZonePricePK, updatePrice);
            return Ok("Succesfully edited");
        }

        [HttpDelete("{pk}")]
        public async Task<IActionResult> Delete(Guid pk)
        {
            await _transportZonePriceRepo.Remove(pk);
            return Ok("Deleted successfully");
        }
    }
}
