using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class AgentRepository : IAgentRepository
{
    private readonly IConnectionFactory connectionFactory;

    public AgentRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public Task<Agent> CreateAgent(Agent agent)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> DeleteAgent(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Agent> GetAgent(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Agent>> GetAgents(bool includeInactive = false)
    {
        throw new NotImplementedException();
    }

    public Task<List<Agent>> SearchAgents(SearchParameters parameters)
    {
        throw new NotImplementedException();
    }

    public Task<int> SearchAgentsCount(SearchParameters parameters)
    {
        throw new NotImplementedException();
    }

    public Task<Agent> UpdateAgent(Guid id, Agent agent)
    {
        throw new NotImplementedException();
    }
}