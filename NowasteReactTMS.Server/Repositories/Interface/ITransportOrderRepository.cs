using NowasteTms.Model;

public interface ITransportOrderRepository
{
    Task<TransportOrder> Get(Guid id);
    Task<TransportOrder> GetWholeById(string transportOrderId);

    Task<List<TransportOrder>> GetConsolidatedTransportOrders(Guid consolidatedTransportOrderGuid);

    Task<Guid> Add(TransportOrder transportOrder);

    Task Update(TransportOrder transportOrder);

    Task<int> UpdateHeader(TransportOrder transportOrder);

    Task<int> SetRejected(Guid id);

    Task<int> SetConfirmed(Guid id, DateTime eta, string vehicleRegistrationPlate);

    Task<int> SetArrival(Guid id, DateTime arrival);

    Task<int> IncreaseFilenameRevision(Guid id);

    Task<SearchTransportOrderResponse> SearchOrders(SearchParameters parameters, Guid? agentPK, bool historical);

    Task<List<TransportOrder>> ExcelExportTransportOrder(SearchParameters parameters, Guid? agentPK);

    Task SetParentTransportOrder(Guid consolidatedTransportOrderPK, IEnumerable<Guid> transportOrderPKs);
    Task Delete(Guid transportOrderPk);

    Task<IEnumerable<TransportOrder>> GetForOrder(Guid PK);
}

public class SearchTransportOrderResponse
{
    public IEnumerable<TransportOrder> TransportOrders { get; set; }
    public int SearchCount { get; set; }
}