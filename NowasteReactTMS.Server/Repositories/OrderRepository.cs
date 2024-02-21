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
}