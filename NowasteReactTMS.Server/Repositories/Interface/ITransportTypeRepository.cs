using NowasteTms.Model;

namespace NowasteReactTMS.Server.Repositories.Interface
{
    public interface ITransportTypeRepository
    {
        Task<List<TransportType>> GetAll();
        Task<int> Add(TransportType transportType);
        Task<TransportType> Get(Guid pk);
        Task<int> Update(TransportType transportType);
    }
}
