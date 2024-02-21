using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class CustomerRepository : ICustomerRepository
{
    private readonly IConnectionFactory connectionFactory;

    public CustomerRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }
}