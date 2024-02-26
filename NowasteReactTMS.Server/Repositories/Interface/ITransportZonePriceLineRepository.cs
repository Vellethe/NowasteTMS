using NowasteTms.Model;

public interface ITransportZonePriceLineRepository
{
    Task<int> Add(TransportZonePriceLine transportZonePrice);
    Task<int> Update(TransportZonePriceLine transportZonePriceLine);
    Task<IEnumerable<TransportZonePriceLine>> GetByTransportZonePricePk(Guid pk);
}