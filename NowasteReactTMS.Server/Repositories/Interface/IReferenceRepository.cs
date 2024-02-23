using NowasteTms.Model;
public interface IReferenceRepository
{
    Task<IEnumerable<Reference>> GetAllForContactInformation(Guid? contactInformationPK);
    Task<IEnumerable<Reference>> GetAllFromListOfContactInformation(List<Guid> contactInformationPK);
    Task<Reference> Add(Reference reference);
    Task<Reference> Update(Reference reference);
    Task Delete(Reference reference);
}