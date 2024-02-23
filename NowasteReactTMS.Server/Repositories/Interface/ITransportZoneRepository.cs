using NowasteTms.Model;

public interface ITransportZoneRepository
{
    Task<List<TransportZone>> GetAll();
    Task<TransportZone> GetForContactInformation(Guid contactInformationPK);
    Task<IEnumerable<TransportZone>> GetAllForContactInformation(Guid pk);
    Task<IEnumerable<TransportZone>> GetAllForContactInformation(List<Guid> pk);
    Task<TransportZone> Get(Guid pk);
    Task<TransportZone> Create(TransportZone transportZone);
    Task<int> Connect(Guid transportZonePK, Guid contactInformationPK);
    Task<int> Disconnect(Guid transportZonePK, Guid contactInformationPK);
}