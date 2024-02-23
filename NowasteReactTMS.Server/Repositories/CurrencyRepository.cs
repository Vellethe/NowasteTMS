using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class CurrencyRepository : ICurrencyRepository
{
    private readonly IConnectionFactory connectionFactory;

    public CurrencyRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public Task<Currency> Get(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Currency>> GetAll()
    {
        throw new NotImplementedException();
    }
}