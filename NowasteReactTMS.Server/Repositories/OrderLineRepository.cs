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

    public Task<Order> GetOrder(Guid Id)
    {
        throw new NotImplementedException();
    }
}