using Dapper;
using Microsoft.AspNetCore.Connections;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
using IConnectionFactory = WMan.Data.ConnectionFactory.IConnectionFactory;
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

    public Task<int> AddOrder(Order order)
    {
        throw new NotImplementedException();
    }
}

// MED SQL QUERIES
//public async Task<Order> GetById(string orderId)
//{
//    using (var connection = connectionFactory.CreateConnection())
//    {
//        var order = await connection.QueryAsync<Order>(@"
//                SELECT 
//                       o.[OrderPK]

//                  FROM [dbo].[Order] o
//                WHERE [OrderID] = @orderId",
//            new
//            {
//                orderId
//            }
//        );
//        if (order.Any())
//            return await this.Get(order.First().OrderPK);

//        return null;
//    }
//}

// UTAN SQL QUERIES
//public async Task<Order> GetById(string orderId)
//{
//    // Assume dbContext is your Entity Framework DbContext
//    var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderID == orderId);
//    if (order != null)
//        return await this.Get(order.OrderPK);

//    return null;
//}
