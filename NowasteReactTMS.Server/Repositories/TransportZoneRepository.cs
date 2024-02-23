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

    public Task<int> Connect(Guid transportZonePK, Guid contactInformationPK)
    {
        throw new NotImplementedException();
    }

    public Task<TransportZone> Create(TransportZone transportZone)
    {
        throw new NotImplementedException();
    }

    public Task<int> Disconnect(Guid transportZonePK, Guid contactInformationPK)
    {
        throw new NotImplementedException();
    }

    public Task<TransportZone> Get(Guid pk)
    {
        throw new NotImplementedException();
    }

    public Task<List<TransportZone>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransportZone>> GetAllForContactInformation(Guid pk)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransportZone>> GetAllForContactInformation(List<Guid> pk)
    {
        throw new NotImplementedException();
    }

    public Task<TransportZone> GetForContactInformation(Guid contactInformationPK)
    {
        throw new NotImplementedException();
    }
}