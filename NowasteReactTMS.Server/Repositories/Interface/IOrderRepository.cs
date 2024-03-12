using Microsoft.EntityFrameworkCore;
using NowasteReactTMS.Server;
using NowasteTms.Model;

public interface IOrderRepository
{
    Task<Order> Get(Guid pk);
    Task<Order> GetOrder(Guid id);
    Task<Order> GetById(string orderId);
    Task<int> UpdateOrder(Order order);
    Task<int> AddOrder(Order order);
    //Task<List<Order>> GetAllOrders();
    //Task<List<Order>> GetHistoricalOrders();
    Task<SearchOrderResponse> SearchOrders(SearchParameters parameters);
}