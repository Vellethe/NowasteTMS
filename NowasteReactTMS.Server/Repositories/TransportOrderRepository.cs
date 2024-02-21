using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class TransportOrderRepository : ITransportOrderRepository
{
    private readonly IConnectionFactory connectionFactory;

    public TransportOrderRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }
}