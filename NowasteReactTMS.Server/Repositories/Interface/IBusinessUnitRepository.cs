using NowasteTms.Model;

public interface IBusinessUnitRepository
{
    Task<List<BusinessUnit>> GetAll();
    Task<BusinessUnit> Get(Guid pk);
    Task<IEnumerable<BusinessUnit>> Get(IEnumerable<Guid> businessUnitPKs);
    Task<BusinessUnit> GetByContactInformationPk(Guid guid);
    Task<BusinessUnit> Update(BusinessUnit bu);
    Task<BusinessUnit> Create(BusinessUnit bu);
    Task Delete(BusinessUnit bu);
}