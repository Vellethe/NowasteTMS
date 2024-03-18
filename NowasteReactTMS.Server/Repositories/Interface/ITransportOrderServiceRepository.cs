using NowasteTms.Model;

public interface ITransportOrderServiceRepository
{
    Task<List<TransportOrderService>> GetAllTransportOrderServices(Guid? agentPk, bool includeInactive = false);
    Task<TransportOrderService> Get(Guid id);
    Task<int> Outdate(TransportOrderService transportOrderService);
    Task<int> Add(TransportOrderService transportOrderService);
    Task<List<TransportOrderService>> GetAllTransportOrderServices(bool includeInactive = false);
    Task<Guid> Delete(Guid id);
}