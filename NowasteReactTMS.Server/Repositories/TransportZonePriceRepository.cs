using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class TransportZonePriceRepository : ITransportZonePriceRepository
{
    private readonly IConnectionFactory connectionFactory;

    public TransportZonePriceRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }
}