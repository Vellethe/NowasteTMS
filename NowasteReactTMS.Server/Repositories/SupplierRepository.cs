using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class SupplierRepository : ISupplierRepository
{
    private readonly IConnectionFactory connectionFactory;

    public SupplierRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public Task<Supplier> CreateSupplier(Supplier supplier)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> DeleteSupplier(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Supplier> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Supplier> GetSupplier(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Supplier>> GetSuppliers(bool includeInactive = false)
    {
        throw new NotImplementedException();
    }

    public Task<List<Supplier>> SearchSuppliers(SearchParameters parameters)
    {
        throw new NotImplementedException();
    }

    public Task<int> SearchSuppliersCount(SearchParameters parameters)
    {
        throw new NotImplementedException();
    }

    public Task<Supplier> UpdateSupplier(Guid id, Supplier supplier)
    {
        throw new NotImplementedException();
    }
}