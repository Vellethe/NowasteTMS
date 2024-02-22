using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class TransportZoneRepository : ITransportZoneRepository
{
    private readonly IConnectionFactory connectionFactory;

    public TransportZoneRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }
}