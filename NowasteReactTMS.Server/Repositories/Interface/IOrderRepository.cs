using NowasteTms.Model;

public interface IOrderRepository
{
    Task<Order> Get(Guid pk);
    Task<Order> GetOrder();
    Task<Order> GetById(string orderId);
    Task<int> UpdateOrder(Order order);
    Task<int> AddOrder(Order order);
    Task<SearchOrderResponse> SearchOrders(SearchParameters parameters);
}