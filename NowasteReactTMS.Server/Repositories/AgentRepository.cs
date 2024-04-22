using Dapper;
using NowasteReactTMS.Server.Models.AgentDTOs;
using NowasteTms.Model;
using System.Transactions;
using WMan.Data.ConnectionFactory;
public class AgentRepository : IAgentRepository
{
    private readonly IConnectionFactory connectionFactory;
    private readonly IBusinessUnitRepository businessUnitRepository;

    private readonly Dictionary<string, string> columnMapping = new Dictionary<string, string>
        {
            {"Id", "a.[AgentId]"},
            {"Name", "cbu.[Name]"},
            {"SelfBilling", "a.[isSelfBilling]"},
            {"Country", "ci.[Country]"},
            {"Currency", "cu.[ShortName]" }
        };



    public AgentRepository(IConnectionFactory connectionFactory, IBusinessUnitRepository businessUnitRepository)
    {
        this.connectionFactory = connectionFactory;
        this.businessUnitRepository = businessUnitRepository;
    }

    public async Task<List<Agent>> GetAgents(bool includeInactive = false)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var agents = connection.Query<Agent>(@"
                    SELECT a.[AgentPK] AS Id
                          ,a.[AgentPK]
                          ,a.[AgentID] 
                          ,a.[BusinessUnitPK]
                          ,a.[isSelfBilling]
                          ,a.[IsActive]
                      FROM [dbo].[Agent] a
                    WHERE a.[isActive] = 1 OR @includeInactive = 1",
                new
                {
                    includeInactive
                }).ToList();

            if (agents.Any())
            {
                var businessRepo = await businessUnitRepository.Get(agents.Select(a => a.BusinessUnitPK));
                foreach (var agent in agents)
                    agent.BusinessUnit = businessRepo.Single(b => b.BusinessUnitPK == agent.BusinessUnitPK);
            }
            return agents;
        }
    }

    public async Task<Agent> GetAgent(Guid id)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var agent = connection.QueryFirstOrDefault<Agent>(@"
                SELECT a.[AgentPK] AS Id
                      ,a.[AgentPK]
                      ,a.[AgentID] 
                      ,a.[BusinessUnitPK]
                      ,a.[isSelfBilling]
                      ,a.[IsActive]
                  FROM [dbo].[Agent] a
                WHERE a.[AgentPK] = @id", new { id });

            if (null == agent) return null;

            var bu = await businessUnitRepository.Get(agent.BusinessUnitPK);
            agent.BusinessUnit = bu;

            return agent;
        }
    }

    public async Task<AgentDTO> UpdateAgent(Guid id, AgentDTO agent)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [dbo].[Agent]
                SET         [AgentID] = @AgentID
                           ,[isSelfBilling] = @IsSelfBilling
                           ,[IsActive] = @IsActive
                WHERE [AgentPK] = @AgentPK", agent);

            agent.BusinessUnit = await businessUnitRepository.Update(agent.BusinessUnit);
            scope.Complete();
        }

        return agent;
    }

    public async Task<Agent> CreateAgent(Agent agent)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        using (var connection = connectionFactory.CreateConnection())
        {
            await businessUnitRepository.Create(agent.BusinessUnit);

            await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[Agent]
                            ([AgentPK]
                            ,[AgentID]
                            ,[BusinessUnitPK]
                            ,[isSelfBilling]
                           ,[IsActive])
                VALUES
                            (@AgentPK
                            ,@AgentID
                            ,@BusinessUnitPK
                            ,@IsSelfBilling
                            ,@IsActive)", agent);

            scope.Complete();
        }
        return agent;
    }

    public async Task<Guid> DeleteAgent(Guid id)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [dbo].[Agent]
                SET         [isActive] = 0
                WHERE [AgentPK] = @id",
                new { id }
            );
        }
        return id;
    }

    public async Task<List<Agent>> SearchAgents(SearchParameters parameters)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var builder = new SqlBuilder();
            var template = builder.AddTemplate(@"
          SELECT 
  		         a.[AgentPK] AS Id
		        ,a.[AgentPK]
		        ,a.[AgentID] 
		        ,a.[BusinessUnitPK]
                ,a.[isSelfBilling]

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

            FROM [dbo].[Agent] a
            JOIN [BusinessUnit] cbu ON a.[BusinessUnitPK] = cbu.[BusinessUnitPK]
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
      FETCH NEXT @limit ROWS ONLY
                        ",
                new
                {
                    offset = parameters.Offset,
                    limit = parameters.Limit
                }
            );

            //where clauses for placeholder /**where**/
            builder.Where("a.[AgentID] IS NOT NULL");
            builder.Where("a.[AgentID] <> ''");
            builder.Where("a.[isActive] = 1");

            foreach (var filter in parameters.Filters)
            {
                builder.Where($"{columnMapping[filter.Key]} LIKE '%{filter.Value}%'");
            }

            if (parameters.SortOrders != null && parameters.SortOrders.Any())
            {
                foreach (var column in parameters.SortOrders)
                    builder.OrderBy(columnMapping[column.Key] + " " + (column.Value ? "ASC" : "DESC"));
            }
            else
            {
                // Forced default sort, OFFSET won't work otherwise (for placeholder /**orderby**/)
                builder.OrderBy("[AgentID] ASC");
            }

            var agents =
                await connection
                    .QueryAsync<Agent, BusinessUnit, ContactInformation, FinanceInformation, Currency, Agent>(
                        template.RawSql, map: (a, cbu, ci, fi, cu) =>
                        {
                            a.BusinessUnit = cbu;
                            a.BusinessUnit.FinanceInformation = fi;
                            a.BusinessUnit.FinanceInformation.Currency = cu;
                            a.BusinessUnit.ContactInformations = new List<ContactInformation>() { ci };
                            return a;
                        }, template.Parameters);

            return agents.ToList();
        }
    }

    public async Task<int> SearchAgentsCount(SearchParameters parameters)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var builder = new SqlBuilder();
            var template = builder.AddTemplate(@"
          SELECT count(*)
            FROM [dbo].[Agent] a
            JOIN [BusinessUnit] cbu ON a.[BusinessUnitPK] = cbu.[BusinessUnitPK]
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
            builder.Where("a.[AgentID] IS NOT NULL");
            builder.Where("a.[AgentID] <> ''");
            builder.Where("a.[isActive] = 1");

            foreach (var filter in parameters.Filters)
                builder.Where($"{columnMapping[filter.Key]} LIKE '%{filter.Value}%'");

            var result = await connection.QueryAsync<int>(template.RawSql, template.Parameters);

            return result.FirstOrDefault();
        }
    }
}