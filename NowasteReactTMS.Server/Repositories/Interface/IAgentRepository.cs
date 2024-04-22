using NowasteReactTMS.Server.Models.AgentDTOs;
using NowasteTms.Model;

public interface IAgentRepository
{
    Task<List<Agent>> GetAgents(bool includeInactive = false);
    Task<Agent> GetAgent(Guid id);
    Task<AgentDTO> UpdateAgent(Guid id, AgentDTO agent);
    Task<Agent> CreateAgent(Agent agent);
    Task<Guid> DeleteAgent(Guid id);
    Task<List<Agent>> SearchAgents(SearchParameters parameters);
    Task<int> SearchAgentsCount(SearchParameters parameters);
}