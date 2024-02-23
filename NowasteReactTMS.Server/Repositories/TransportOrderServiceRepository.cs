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

    public Task<int> Add(TransportOrderService transportOrderService)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<TransportOrderService> Get(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<TransportOrderService>> GetAllTransportOrderServices(Guid? agentPk, bool includeInactive = false)
    {
        throw new NotImplementedException();
    }

    public Task<int> Outdate(TransportOrderService transportOrderService)
    {
        throw new NotImplementedException();
    }
}