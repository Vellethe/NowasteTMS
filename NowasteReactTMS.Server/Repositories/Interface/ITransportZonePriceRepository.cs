using NowasteTms.Model;

public interface ITransportZonePriceRepository
{
    Task<List<TransportZonePrice>> GetAll(bool includeInactive = false);
    Task<int> Remove(Guid pk);
    Task<int> Add(TransportZonePrice vmTransportZonePrice);
    Task<TransportZonePrice> Get(Guid id);
    Task<List<List<TransportZonePrice>>> Get(GetBestPricesQuery query);
    Task<List<List<TransportZonePrice>>> GetGrouped(GetBestPricesQuery query);
    Task<TransportZonePrice> Update(Guid id, TransportZonePrice transportZonePrice);
    Task<IEnumerable<TransportZonePrice>> GetPrices(Guid fromTransportZonePK, Guid toTransportZonePK);
    Task<IEnumerable<TransportZonePrice>> GetGroupedPrices(Guid fromTransportZonePK, Guid toTransportZonePK);
}