using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NowasteReactTMS.Server.Models;
using NowasteTms.Model;

namespace NowasteReactTMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : Controller
    {
        private readonly ITransportOrderServiceRepository _transportOrderServiceRepo;
        private readonly ICurrencyRepository _currencyRepo;
        private readonly IAgentRepository _agentRepo;
        public ServiceController(ITransportOrderServiceRepository transportOrderServiceRepo,
           ICurrencyRepository currencyRepo,
           IAgentRepository agentRepo)
        {
            _transportOrderServiceRepo = transportOrderServiceRepo;
            _currencyRepo = currencyRepo;
            _agentRepo = agentRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransportOrderServices(Guid agentPK)
        {
            var transportOrderServices = await _transportOrderServiceRepo.GetAllTransportOrderServices(agentPK);
            var currencies = await _currencyRepo.GetAll();
            var agents = await _agentRepo.GetAgents();

            var responseData = new
            {
                TransportOrderServices = transportOrderServices,
                Currencies = currencies,
                Agents = agents
            };

            return Ok(responseData);
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create(ServiceDTO dto)
        {
            var newTransportOrderService = new TransportOrderService
            {
                TransportOrderServicePK = Guid.NewGuid(),
                Name = dto.TransportOrderService.Name,
                Price = dto.TransportOrderService.Price,
                CurrencyPK = dto.TransportOrderService.CurrencyPK,
                SucceedingVersionPK = null,
                isActive = true,
                Timestamp = DateTime.Now,
                AgentPK = dto.TransportOrderService.AgentPK
            };

            await _transportOrderServiceRepo.Add(newTransportOrderService);

            return Ok("Order created");
        }
        [HttpPut]
        public async Task<IActionResult> Edit(ServiceDTO dto)
        {
            var newVersionOfTransportOrderService = new TransportOrderService
            {
                TransportOrderServicePK = Guid.NewGuid(),
                Name = dto.TransportOrderService.Name,
                Price = dto.TransportOrderService.Price,
                CurrencyPK = dto.TransportOrderService.CurrencyPK,
                SucceedingVersionPK = null,
                isActive = true,
                Timestamp = DateTime.Now,
                AgentPK = dto.TransportOrderService.AgentPK
            };

            var outdatedVersionOfTransportOrderService = new TransportOrderService
            {
                Name = dto.TransportOrderService.Name,
                Price = dto.TransportOrderService.Price,
                CurrencyPK = dto.TransportOrderService.CurrencyPK,
                TransportOrderServicePK = dto.TransportOrderService.TransportOrderServicePK,
                SucceedingVersionPK = newVersionOfTransportOrderService.TransportOrderServicePK,
                isActive = false,
                Timestamp = DateTime.Now,
                AgentPK = dto.TransportOrderService.AgentPK
            };

            await _transportOrderServiceRepo.Outdate(outdatedVersionOfTransportOrderService);
            await _transportOrderServiceRepo.Add(newVersionOfTransportOrderService);

            return Ok("Order edited");
        }
        [HttpDelete]
        public async Task <IActionResult> Delete(Guid pk)
        {
            await _transportOrderServiceRepo.Delete(pk);
            return Ok("Successfully deleted");
        }


    }
}
