using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class OrderRepository: IOrderRepository
{
    private readonly IConnectionFactory connectionFactory;

    public OrderRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public Task<Order> GetOrder(Guid id)
    {
        throw new NotImplementedException();
    }
}