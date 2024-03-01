using Dapper;
using NowasteTms.Model;
using System.Transactions;
using WMan.Data.ConnectionFactory;
public class CustomerRepository : ICustomerRepository
{
    // Match sorting and filtering keywords to column names
    private readonly Dictionary<string, string> columnMapping = new Dictionary<string, string>
        {
            {"Id", "[c].[CustomerId]"},
            {"Name", "[cbu].[Name]"},
            {"Country", "[ci].[Country]"},
            {"Currency", "[cu].[ShortName]" }
        };

    private readonly IConnectionFactory connectionFactory;
    private readonly IBusinessUnitRepository businessUnitRepository;

    public CustomerRepository(IConnectionFactory connectionFactory, IBusinessUnitRepository businessUnitRepository)
    {
        this.connectionFactory = connectionFactory;
        this.businessUnitRepository = businessUnitRepository;
    }

    public async Task<List<Customer>> GetCustomers()
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var customers = connection.Query<Customer>(@"
                SELECT c.[CustomerPK] AS Id
                      ,c.[CustomerPK]
                      ,c.[CustomerID] 
                      ,c.[BusinessUnitPK]
                      ,c.[IsActive]
                  FROM [dbo].[Customer] c
                  WHERE c.[isActive] = 1").ToList();

            foreach (var customer in customers)
                customer.BusinessUnit = await businessUnitRepository.Get(customer.BusinessUnitPK);

            return customers;
        }
    }

    public async Task<Customer> GetCustomer(Guid id)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var customer = connection.QueryFirstOrDefault<Customer>(@"
                SELECT c.[CustomerPK] AS Id
                      ,c.[CustomerPK]
                      ,c.[CustomerID] 
                      ,c.[BusinessUnitPK]
                      ,c.[IsActive]
                  FROM [dbo].[Customer] c
                WHERE c.[CustomerPK] = @id",
                new { id });

            if (null == customer) return null;

            var bu = await businessUnitRepository.Get(customer.BusinessUnitPK);
            customer.BusinessUnit = bu;

            return customer;
        }
    }

    public async Task<Customer> UpdateCustomer(Guid id, Customer customer)
    {
        if (await GetCustomer(id) == null)
            throw new Exception("Could not find customer");

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [dbo].[Customer]
                SET         [CustomerID] = @CustomerID
                           ,[IsActive] = @IsActive
                WHERE [CustomerPK] = @CustomerPK",
                                customer);

            customer.BusinessUnit = await businessUnitRepository.Update(customer.BusinessUnit);
            scope.Complete();
        }

        return customer;
    }

    public async Task<Customer> CreateCustomer(Customer customer)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            await businessUnitRepository.Create(customer.BusinessUnit);

            await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[Customer]
                            ([CustomerPK]
                            ,[CustomerID]
                            ,[BusinessUnitPK]
                            ,[IsActive])

                VALUES
                            (@CustomerPK
                            ,@CustomerID
                            ,@BusinessUnitPK
                            ,@IsActive)",
                customer);

            scope.Complete();
        }
        return customer;
    }


    public async Task<Guid> DeleteCustomer(Guid id)
    {
        if (await GetCustomer(id) == null)
            throw new Exception("Could not find customer");

        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE[dbo].[Customer]
                SET[isActive] = 0
                WHERE [CustomerPK] = @id",
                new { id }
            );
        }
        return id;
    }


    public async Task<List<Customer>> SearchCustomers(SearchParameters parameters)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var builder = new SqlBuilder();
            var template = builder.AddTemplate(@"
                SELECT 
  		             c.[CustomerPK] AS Id
		            ,c.[CustomerPK]
		            ,c.[CustomerID] 
		            ,c.[BusinessUnitPK]

		            ,cbu.[BusinessUnitPK] AS Id
		            ,cbu.[BusinessUnitPK]
		            ,cbu.[Name]

		            ,ci.[ContactInformationPK] AS Id
		            ,ci.[ContactInformationPK]
		            ,ci.[BusinessUnitPK]
		            ,ci.[Country]

		            ,fi.[FinanceInformationPK] AS Id
		            ,fi.[FinanceInformationPK]
		            ,fi.[CurrencyPK]

		            ,cu.[CurrencyPK] AS Id
		            ,cu.[CurrencyPK]
		            ,cu.[ShortName]

                FROM [dbo].[Customer] c
                JOIN [BusinessUnit] cbu ON c.[BusinessUnitPK] = cbu.[BusinessUnitPK]
                LEFT JOIN [ContactInformation] ci ON ci.[ContactInformationPK] = 
		            (SELECT TOP 1 c.[ContactInformationPK]
				       FROM [ContactInformation] c
				      WHERE c.[BusinessUnitPK] = cbu.[BusinessUnitPK]
                        AND c.[IsActive] = 1
                        AND c.[IsDefault] = 1 
				        AND c.[isEditable] = 0
				        AND c.[Country] IS NOT NULL 
				        AND c.[Country] <> '')    
                LEFT JOIN [FinanceInformation] fi ON cbu.[FinanceInformationPK] = fi.[FinanceInformationPK]
                LEFT JOIN [Currency] cu ON fi.[CurrencyPK] = cu.[CurrencyPK]
               /**where**/             
               /**orderby**/
            OFFSET @offset ROWS
            FETCH NEXT @limit ROWS ONLY",
                new
                {
                    offset = parameters.Offset,
                    limit = parameters.Limit
                }
            );

            //where clauses for placeholder /**where**/
            builder.Where("c.[CustomerID] IS NOT NULL");
            builder.Where("c.[CustomerID] <> ''");
            builder.Where("c.[isActive] = 1");

            foreach (var filter in parameters.Filters)
            {
                builder.Where($"{columnMapping[filter.Key]} LIKE '%{filter.Value}%'");
            }

            if (parameters.SortOrders != null && parameters.SortOrders.Any())
            {
                foreach (var column in parameters.SortOrders)
                {
                    builder.OrderBy(columnMapping[column.Key] + " " + (column.Value ? "ASC" : "DESC"));
                }
            }
            else // Forced default sort, OFFSET won't work otherwise (for placeholder /**orderby**/)
            {
                builder.OrderBy("[CustomerID] ASC");
            }

            var customers =
                await connection
                    .QueryAsync<Customer, BusinessUnit, ContactInformation, FinanceInformation, Currency, Customer>(
                        template.RawSql, map: (c, cbu, ci, fi, cu) =>
                        {
                            c.BusinessUnit = cbu;
                            c.BusinessUnit.FinanceInformation = fi;
                            c.BusinessUnit.FinanceInformation.Currency = cu;
                            c.BusinessUnit.ContactInformations = new List<ContactInformation>() { ci };
                            return c;
                        }, template.Parameters);

            return customers.ToList();
        }
    }

    public async Task<int> SearchCustomersCount(SearchParameters parameters)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var builder = new SqlBuilder();
            var template = builder.AddTemplate(@"
              SELECT count(*)
                FROM [dbo].[Customer] c
                JOIN [BusinessUnit] cbu ON c.[BusinessUnitPK] = cbu.[BusinessUnitPK]
           LEFT JOIN [ContactInformation] ci ON ci.[ContactInformationPK] = 
		            (SELECT TOP 1 c.[ContactInformationPK]
				       FROM [ContactInformation] c
				      WHERE c.[BusinessUnitPK] = cbu.[BusinessUnitPK]
                        AND c.[IsActive] = 1
                        AND c.[IsDefault] = 1 
				        AND c.[isEditable] = 0
				        AND c.[Country] IS NOT NULL 
				        AND c.[Country] <> '')    
           LEFT JOIN [FinanceInformation] fi ON cbu.[FinanceInformationPK] = fi.[FinanceInformationPK]
           LEFT JOIN [Currency] cu ON fi.[CurrencyPK] = cu.[CurrencyPK]
               /**where**/             
                    ");

            //where clauses for placeholder /**where**/
            builder.Where("c.[CustomerID] IS NOT NULL");
            builder.Where("c.[CustomerID] <> ''");
            builder.Where("c.[isActive] = 1");

            foreach (var filter in parameters.Filters)
                builder.Where($"{columnMapping[filter.Key]} LIKE '%{filter.Value}%'");

            var result = await connection.QueryAsync<int>(template.RawSql, template.Parameters);

            return result.FirstOrDefault();
        }
    }

    public async Task<List<Customer>> GetCustomerList()
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var builder = new SqlBuilder();
            var template = builder.AddTemplate(@"

  		     SELECT  c.[CustomerPK] AS Id
		            ,c.[CustomerPK]
		            ,c.[CustomerID] 
		            ,c.[BusinessUnitPK]
		            ,c.[IsActive]

		            ,cbu.[BusinessUnitPK] AS Id
		            ,cbu.[BusinessUnitPK]
		            ,cbu.[Name]

              FROM [dbo].[Customer] c
              JOIN [BusinessUnit] cbu ON c.[BusinessUnitPK] = cbu.[BusinessUnitPK]
             WHERE c.[isActive] = 1     
                    ");

            var customers =
                await connection
                    .QueryAsync<Customer, BusinessUnit, Customer>(
                        template.RawSql, map: (c, cbu) =>
                        {
                            c.BusinessUnit = cbu;
                            return c;
                        });

            return customers.ToList();
        }
    }
}