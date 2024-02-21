using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class SupplierRepository : ISupplierRepository
{
    private readonly IConnectionFactory connectionFactory;

    public SupplierRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }
}