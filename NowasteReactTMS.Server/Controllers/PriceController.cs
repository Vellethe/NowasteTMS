using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NowasteReactTMS.Server.Models;
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

        [HttpPut]
        public async Task<IActionResult> Edit(TransportPriceDTO dto)
        {
            var updatePrice = await _transportZonePriceRepo.Get(dto.TransportZonePrice.TransportZonePricePK);
            updatePrice.Price = dto.TransportZonePrice.Price;
            updatePrice.Description = dto.TransportZonePrice.Description;
            updatePrice.ValidFrom = dto.TransportZonePrice.ValidFrom;
            updatePrice.ValidTo = dto.TransportZonePrice.ValidTo;

            if (updatePrice.TransportType.Description == "Groupage")
            {
                updatePrice.Price = dto.TransportZonePrice.TransportZonePriceLines.Sum(line => line.Price);
                updatePrice.TransportZonePriceLines = dto.TransportZonePrice.TransportZonePriceLines.Select(line => new TransportZonePriceLine
                {
                    TransportZonePriceLinePK = line.TransportZonePriceLinePK,
                    Price = line.Price
                }).ToList();
            }
            await _transportZonePriceRepo.Update(dto.TransportZonePrice.TransportZonePricePK, updatePrice);
            return Ok("Succesfully edited");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid pk)
        {
            await _transportZonePriceRepo.Remove(pk);
            return Ok("Deleted successfully");
        }
    }
}
