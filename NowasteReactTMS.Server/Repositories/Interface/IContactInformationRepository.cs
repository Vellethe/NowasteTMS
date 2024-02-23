using NowasteTms.Model;

public interface IContactInformationRepository
{
    Task<ContactInformation> Get(Guid pk);

    Task<IEnumerable<ContactInformation>> GetAll();

    Task<IEnumerable<ContactInformation>> GetForBusinessUnits(IEnumerable<Guid> businessUnitPKs);

    Task<ContactInformation> Add(ContactInformation ci);

    Task<ContactInformation> Update(ContactInformation ci);

    Task Delete(ContactInformation ci);
}