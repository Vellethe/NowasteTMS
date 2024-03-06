using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NowasteTms.Model;

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
        [HttpPost]
        public async Task<IActionResult> CreateAgent([FromBody] Agent newAgent)
        {
            try
            {
                if (newAgent == null ||
                    newAgent.AgentPK == Guid.Empty ||
                    string.IsNullOrEmpty(newAgent.AgentID) ||
                    newAgent.BusinessUnitPK == Guid.Empty ||
                    newAgent.BusinessUnit == null ||
                    newAgent.TransportZonePrices == null ||
                    !newAgent.TransportZonePrices.Any())
                {
                    return BadRequest("Invalid agent data. Please provide all required properties.");
                }

                if (newAgent.IsActive == default)
                {
                    newAgent.IsActive = true;
                }

                var createdAgent = await _agentRepo.CreateAgent(newAgent);

                return Ok(createdAgent);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while creating the agent: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the agent.");
            }
        }


        [HttpGet("{pk}")]
    public async Task<IActionResult> GetAgent(Guid pk)
    {
            try
            {
            var agent = await _agentRepo.GetAgent(pk);
                if (agent == null)
                {
                    return NotFound($"Agent with ID {pk} not found");
                }
                return Ok(agent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving agent: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<Agent>>> GetAllAgents(bool includeInactive = false)
        {
            try
            {
                var agents = await _agentRepo.GetAgents(includeInactive);
                return Ok(agents);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{pk}")]
        public async Task<IActionResult> UpdateAgent(Guid pk, [FromBody] Agent updatedAgent)
        {
            try
            {
                if (pk != updatedAgent.AgentPK)
                {
                    return BadRequest("Agent ID in the URL does not match the one in the request body.");
                }

                var result = await _agentRepo.UpdateAgent(pk, updatedAgent);

                if (result == null)
                {
                    return NotFound($"Agent with ID {pk} not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while updating the agent: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the agent.");
            }
        }

        [HttpDelete("{pk}")]
        public async Task<IActionResult> DeleteAgent(Guid id)
        {
            try
            {
                await _agentRepo.DeleteAgent(id);
                return Ok("Agent deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while deleting the agent: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the agent.");
            }
        }

    }
}
