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

    public async Task<List<Currency>> GetAll()
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var currencies = await connection.QueryAsync<Currency>(@"
                SELECT c.[CurrencyPK]
                      ,c.[Name] 
                      ,c.[ShortName]
                  FROM [dbo].[Currency] c");

            return currencies.ToList();
        }
    }

    public async Task<Currency> Get(Guid id)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var currencies = await connection.QueryAsync<Currency>(@"
                SELECT c.[CurrencyPK]
                      ,c.[Name] 
                      ,c.[ShortName]
                  FROM [dbo].[Currency] c
                WHERE c.[CurrencyPK] = @id",
                new
                {
                    id
                });

            return currencies.FirstOrDefault();
        }
    }
}