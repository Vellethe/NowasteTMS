using NowasteTms.Model;

public interface IOrderLineRepository
{
    Task<Order> GetOrder(Guid Id);
}