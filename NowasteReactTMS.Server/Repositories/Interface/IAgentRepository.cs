using NowasteTms.Model;

public interface IAgentRepository
{
    Task<List<Agent>> GetAgents(bool includeInactive = false);
    Task<Agent> GetAgent(Guid id);
    Task<Agent> UpdateAgent(Guid id, Agent agent);
    Task<Agent> CreateAgent(Agent agent);
    Task<Guid> DeleteAgent(Guid id);
    Task<List<Agent>> SearchAgents(SearchParameters parameters);
    Task<int> SearchAgentsCount(SearchParameters parameters);
}