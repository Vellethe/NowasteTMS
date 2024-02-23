using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class OrderRepository : IOrderRepository
{
    private readonly IConnectionFactory connectionFactory;

    public OrderRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    Task<List<Order>> IOrderRepository.GetOrderAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Order>> GetAllOrderAsync()
    {
        throw new NotImplementedException();
    }
}