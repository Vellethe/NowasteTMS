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

    public Task<Reference> Add(Reference reference)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Reference reference)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Reference>> GetAllForContactInformation(Guid? contactInformationPK)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Reference>> GetAllFromListOfContactInformation(List<Guid> contactInformationPK)
    {
        throw new NotImplementedException();
    }

    public Task<Reference> Update(Reference reference)
    {
        throw new NotImplementedException();
    }
}