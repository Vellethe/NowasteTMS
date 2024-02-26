using NowasteTms.Model;
using System.Data;

public interface ITransportOrderLineRepository
{
    Task<TransportOrderLine> Get(Guid pk);
    Task<IEnumerable<TransportOrderLine>> GetAll();
    Task<IEnumerable<TransportOrderLine>> GetLinesForTransportOrders(IEnumerable<Guid> transportOrderPKs);
    Task Add(IDbConnection conn, TransportOrderLine line);
    Task RemoveAll(IDbConnection conn, Guid transportOrderPK);
}