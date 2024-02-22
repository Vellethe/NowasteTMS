using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class ReferenceRepository : IReferenceRepository
{
    private readonly IConnectionFactory connectionFactory;

    public ReferenceRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }
}