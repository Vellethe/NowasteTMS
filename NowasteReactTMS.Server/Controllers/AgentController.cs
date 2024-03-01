using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace NowasteReactTMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : Controller
    {
        private readonly IAgentRepository _agentRepo;
        private readonly ICurrencyRepository _currencyRepo;

        public AgentController(IAgentRepository agentRepository, ICurrencyRepository currencyRepo)
        {
            _agentRepo = agentRepository;
            _currencyRepo = currencyRepo;
        }
    }
}
