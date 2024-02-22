using NowasteTms.Model;

public interface IOrderRepository
{
    Task<Order> GetOrder(Guid id);
}