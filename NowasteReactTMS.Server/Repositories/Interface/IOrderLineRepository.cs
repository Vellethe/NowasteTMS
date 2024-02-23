using NowasteTms.Model;

public interface IOrderLineRepository
{
    Task<OrderLine> Add(OrderLine line);
    Task<OrderLine> Get(Guid pk);
    Task<Dictionary<Guid, List<OrderLine>>> Get(HashSet<Guid> pks);
    Task<IReadOnlyCollection<OrderLine>> GetByOrder(Guid pk);
    Task<IEnumerable<OrderLine>> Get();
    Task<int> Add(Guid pk, OrderLine orderline);
    Task<int> Update(Guid pk, OrderLine orderline);
    Task<OrderLine> Remove(Guid pk);
}