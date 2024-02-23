using NowasteTms.Model;

public interface IOrderRepository
{
    Task<List<Order>> GetOrderAsync(Guid id);
    Task<List<Order>> GetAllOrderAsync();
    Task<int> AddOrder(Order order);

}