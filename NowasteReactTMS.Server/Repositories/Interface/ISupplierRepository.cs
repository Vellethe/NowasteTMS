using NowasteTms.Model;

public interface ISupplierRepository
{
    Task<List<Supplier>> GetSuppliers(bool includeInactive = false);
    Task<Supplier> GetSupplier(Guid id);
    Task<Supplier> GetById(string id);
    Task<Supplier> UpdateSupplier(Guid id, Supplier supplier);
    Task<Supplier> CreateSupplier(Supplier supplier);
    Task<Guid> DeleteSupplier(Guid id);
    Task<List<Supplier>> SearchSuppliers(SearchParameters parameters);
    Task<int> SearchSuppliersCount(SearchParameters parameters);
}