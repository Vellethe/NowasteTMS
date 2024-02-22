using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class ContactInformationRepository : IContactInformationRepository
{
    private readonly IConnectionFactory connectionFactory;

    public ContactInformationRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }
}