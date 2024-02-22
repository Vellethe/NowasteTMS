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
}