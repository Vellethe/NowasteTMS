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

    public Task<Guid> Add(TransportOrder transportOrder)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid transportOrderPk)
    {
        throw new NotImplementedException();
    }

    public Task<List<TransportOrder>> ExcelExportTransportOrder(SearchParameters parameters, Guid? agentPK)
    {
        throw new NotImplementedException();
    }

    public Task<TransportOrder> Get(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<TransportOrder>> GetConsolidatedTransportOrders(Guid consolidatedTransportOrderGuid)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransportOrder>> GetForOrder(Guid PK)
    {
        throw new NotImplementedException();
    }

    public Task<TransportOrder> GetWholeById(string transportOrderId)
    {
        throw new NotImplementedException();
    }

    public Task<int> IncreaseFilenameRevision(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<SearchTransportOrderResponse> SearchOrders(SearchParameters parameters, Guid? agentPK, bool historical)
    {
        throw new NotImplementedException();
    }

    public Task<int> SetArrival(Guid id, DateTime arrival)
    {
        throw new NotImplementedException();
    }

    public Task<int> SetConfirmed(Guid id, DateTime eta, string vehicleRegistrationPlate)
    {
        throw new NotImplementedException();
    }

    public Task SetParentTransportOrder(Guid consolidatedTransportOrderPK, IEnumerable<Guid> transportOrderPKs)
    {
        throw new NotImplementedException();
    }

    public Task<int> SetRejected(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task Update(TransportOrder transportOrder)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateHeader(TransportOrder transportOrder)
    {
        throw new NotImplementedException();
    }
}