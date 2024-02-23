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

    public Task<int> Add(TransportZonePrice vmTransportZonePrice)
    {
        throw new NotImplementedException();
    }

    public Task<TransportZonePrice> Get(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<List<TransportZonePrice>>> Get(GetBestPricesQuery query)
    {
        throw new NotImplementedException();
    }

    public Task<List<TransportZonePrice>> GetAll(bool includeInactive = false)
    {
        throw new NotImplementedException();
    }

    public Task<List<List<TransportZonePrice>>> GetGrouped(GetBestPricesQuery query)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransportZonePrice>> GetGroupedPrices(Guid fromTransportZonePK, Guid toTransportZonePK)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransportZonePrice>> GetPrices(Guid fromTransportZonePK, Guid toTransportZonePK)
    {
        throw new NotImplementedException();
    }

    public Task<int> Remove(Guid pk)
    {
        throw new NotImplementedException();
    }

    public Task<TransportZonePrice> Update(Guid id, TransportZonePrice transportZonePrice)
    {
        throw new NotImplementedException();
    }
}