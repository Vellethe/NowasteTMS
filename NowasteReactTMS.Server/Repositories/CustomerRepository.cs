using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class CustomerRepository : ICustomerRepository
{
    private readonly IConnectionFactory connectionFactory;

    public CustomerRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public Task<Customer> CreateCustomer(Customer customer)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> DeleteCustomer(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Customer> GetCustomer(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Customer>> GetCustomerList()
    {
        throw new NotImplementedException();
    }

    public Task<List<Customer>> GetCustomers()
    {
        throw new NotImplementedException();
    }

    public Task<List<Customer>> SearchCustomers(SearchParameters parameters)
    {
        throw new NotImplementedException();
    }

    public Task<int> SearchCustomersCount(SearchParameters parameters)
    {
        throw new NotImplementedException();
    }

    public Task<Customer> UpdateCustomer(Guid id, Customer customer)
    {
        throw new NotImplementedException();
    }
}