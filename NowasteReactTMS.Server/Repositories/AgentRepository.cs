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
}