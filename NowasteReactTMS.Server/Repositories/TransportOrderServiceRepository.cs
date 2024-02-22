using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class TransportOrderServiceRepository : ITransportOrderServiceRepository
{
    private readonly IConnectionFactory connectionFactory;

    public TransportOrderServiceRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }
}