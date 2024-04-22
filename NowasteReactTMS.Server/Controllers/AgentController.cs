using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NowasteReactTMS.Server.Models;
using NowasteReactTMS.Server.Models.AgentDTOs;
using NowasteTms.Model;

namespace NowasteReactTMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : Controller
    {
        private readonly IAgentRepository _agentRepo;
        private readonly ICurrencyRepository _currencyRepo;
        private readonly IContactInformationRepository _contactInformationRepo;

        public AgentController(IAgentRepository agentRepository, ICurrencyRepository currencyRepo, IContactInformationRepository contactInformationRepo)
        {
            _agentRepo = agentRepository;
            _currencyRepo = currencyRepo;
            _contactInformationRepo = contactInformationRepo;
        }
        /// <summary>
        /// Create a new Agent with all the needed parameters
        /// </summary>
        /// <param name="newAgent"></param>
        /// <returns></returns>
        [HttpPost("create")]
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

        [HttpGet("Contacts")]
        public async Task<IActionResult> GetContacts()
        {
            var contacts = await _contactInformationRepo.GetAll();
            return Ok(contacts);
        }

        /// <summary>
        /// Display one Agents details by searching on their PK
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Displays all existing Agents
        /// </summary>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SearchAgents(SearchDTO dto)

        {
            var searchParameters = new SearchParameters
            {
                Limit = dto.Size,
                Offset = dto.Page * dto.Size,
                Filters = dto.Filter,
                SortOrders = dto.Column
            };

            var agents = await _agentRepo.SearchAgents(searchParameters);

            return Ok(agents);
        }
        /// <summary>
        /// Update a Agent and their values when searching on their PK
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="updatedAgent"></param>
        /// <returns></returns>
        [HttpPut("{pk}")]
        public async Task<IActionResult> UpdateAgent(Guid pk, [FromBody] Agent updatedAgent)
        {
            try
            {
                var agentDTO = new AgentDTO
                {
                    AgentPK = pk,
                    AgentID = updatedAgent.AgentID,
                    IsSelfBilling = updatedAgent.IsSelfBilling,
                    IsActive = updatedAgent.IsActive,
                    BusinessUnit = updatedAgent.BusinessUnit
                };

                if (pk != updatedAgent.AgentPK)
                {
                    return BadRequest("Agent ID in the URL does not match the one in the request body.");
                }

                var result = await _agentRepo.UpdateAgent(pk, agentDTO);

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

        /// <summary>
        /// Puts an Agent into isActive=0 when "deleting" them using their PK
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        [HttpDelete("{pk}")]
        public async Task<IActionResult> DeleteAgent(Guid pk)
        {
            try
            {
                await _agentRepo.DeleteAgent(pk);
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
