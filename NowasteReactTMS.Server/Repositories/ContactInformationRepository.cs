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

    public Task<ContactInformation> Add(ContactInformation ci)
    {
        throw new NotImplementedException();
    }

    public Task Delete(ContactInformation ci)
    {
        throw new NotImplementedException();
    }

    public Task<ContactInformation> Get(Guid pk)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ContactInformation>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ContactInformation>> GetForBusinessUnits(IEnumerable<Guid> businessUnitPKs)
    {
        throw new NotImplementedException();
    }

    public Task<ContactInformation> Update(ContactInformation ci)
    {
        throw new NotImplementedException();
    }
}