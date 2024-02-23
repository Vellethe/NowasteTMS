using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class ItemRepository : IItemRepository
{
    private readonly IConnectionFactory connectionFactory;

    public ItemRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public Task<Item> CreateItem(Item agent)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> DeleteItem(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Item> GetItem(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Item>> GetItems()
    {
        throw new NotImplementedException();
    }

    public Task<Item> UpdateItem(Guid id, Item agent)
    {
        throw new NotImplementedException();
    }
}