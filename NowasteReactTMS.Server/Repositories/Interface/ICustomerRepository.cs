using NowasteTms.Model;

public interface ICustomerRepository
{
    Task<List<Customer>> GetCustomers();
    Task<Customer> GetCustomer(Guid id);
    Task<Customer> UpdateCustomer(Guid id, Customer customer);
    Task<Customer> CreateCustomer(Customer customer);
    Task<Guid> DeleteCustomer(Guid id);
    Task<List<Customer>> SearchCustomers(SearchParameters parameters);
    Task<int> SearchCustomersCount(SearchParameters parameters);
    Task<List<Customer>> GetCustomerList();
}