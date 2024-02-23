using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class OrderLineRepository : IOrderLineRepository
{
    private readonly IConnectionFactory connectionFactory;

    public OrderLineRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public Task<OrderLine> Add(OrderLine line)
    {
        throw new NotImplementedException();
    }

    public Task<int> Add(Guid pk, OrderLine orderline)
    {
        throw new NotImplementedException();
    }

    public Task<OrderLine> Get(Guid pk)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<Guid, List<OrderLine>>> Get(HashSet<Guid> pks)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderLine>> Get()
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<OrderLine>> GetByOrder(Guid pk)
    {
        throw new NotImplementedException();
    }

    public Task<Order> GetOrder(Guid Id)
    {
        throw new NotImplementedException();
    }

    public Task<OrderLine> Remove(Guid pk)
    {
        throw new NotImplementedException();
    }

    public Task<int> Update(Guid pk, OrderLine orderline)
    {
        throw new NotImplementedException();
    }
}