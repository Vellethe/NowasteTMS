using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class PalletInventoryRepository : IPalletInventoryRepository
{
    private readonly IConnectionFactory connectionFactory;

    public PalletInventoryRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }
}