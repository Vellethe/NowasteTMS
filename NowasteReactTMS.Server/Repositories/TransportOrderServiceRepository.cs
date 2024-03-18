using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class TransportOrderServiceRepository : ITransportOrderServiceRepository
{
    private readonly IConnectionFactory connectionFactory;

    public TransportOrderServiceRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<List<TransportOrderService>> GetAllTransportOrderServices(Guid? agentPk, bool includeInactive = false)
    {
        if (agentPk == Guid.Empty)
            agentPk = null;

        using (var connection = connectionFactory.CreateConnection())
        {
            var transportOrderService = await connection.QueryAsync<TransportOrderService, Currency, TransportOrderService>(@"
                   SELECT tos.[TransportOrderServicePK]
                          ,tos.[isActive]
                          ,tos.[Name]
                          ,tos.[Price]
                          ,tos.[CurrencyPK]
                          ,tos.[SucceedingVersionPK]
                          ,tos.[Timestamp]
                          ,tos.[AgentPK]

                          ,[cu].[CurrencyPK] AS Id
                          ,[cu].[CurrencyPK]
                          ,[cu].[Name]
                          ,[cu].[ShortName]
                    FROM [dbo].[TransportOrderService] tos
                    JOIN Currency cu ON cu.CurrencyPK = tos.CurrencyPK
                    LEFT OUTER JOIN Agent a ON a.AgentPK = tos.AgentPK 
                    WHERE (tos.[isActive] = 1 OR @includeInactive = 1)
                    AND (tos.[AgentPK] = @agentPK OR @agentPk is null)
                    ORDER BY tos.[Timestamp] DESC
            ", (tos, cu) =>
            {
                tos.Currency = cu;

                return tos;
            },
                new
                {
                    agentPk,
                    includeInactive
                });

            return transportOrderService.ToList();
        }
    }

    public async Task<List<TransportOrderService>> GetAllTransportOrderServices(bool includeInactive = false)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportOrderService = await connection.QueryAsync<TransportOrderService, Currency, TransportOrderService>(@"
            SELECT tos.[TransportOrderServicePK]
                   ,tos.[isActive]
                   ,tos.[Name]
                   ,tos.[Price]
                   ,tos.[CurrencyPK]
                   ,tos.[SucceedingVersionPK]
                   ,tos.[Timestamp]
                   ,tos.[AgentPK]

                   ,[cu].[CurrencyPK] AS Id
                   ,[cu].[CurrencyPK]
                   ,[cu].[Name]
                   ,[cu].[ShortName]
             FROM [dbo].[TransportOrderService] tos
             JOIN Currency cu ON cu.CurrencyPK = tos.CurrencyPK
             LEFT OUTER JOIN Agent a ON a.AgentPK = tos.AgentPK 
             WHERE (tos.[isActive] = 1 OR @includeInactive = 1)
             AND (tos.[AgentPK] IS NULL)
             ORDER BY tos.[Timestamp] DESC
        ", (tos, cu) =>
            {
                tos.Currency = cu;
                return tos;
            },
            new
            {
                includeInactive
            });

            return transportOrderService.ToList();
        }
    }

    public async Task<TransportOrderService> Get(Guid pk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportOrderService = await connection.QueryAsync<TransportOrderService, Currency, TransportOrderService>(@"
                SELECT tos.[TransportOrderServicePK]
                      ,tos.[isActive]
                      ,tos.[Name]
                      ,tos.[Price]
                      ,tos.[CurrencyPK]
                      ,tos.[SucceedingVersionPK]
                      ,tos.[Timestamp]
                      ,tos.[AgentPK]

                      ,[cu].[CurrencyPK] AS Id
                      ,[cu].[Name]
                      ,[cu].[ShortName]
                FROM [dbo].[TransportOrderService] tos
                JOIN Currency cu ON cu.CurrencyPK = tos.CurrencyPK
                WHERE tos.[TransportOrderServicePK] = @pk
            ", (tos, cu) =>
            {
                tos.Currency = cu;

                return tos;
            },
                new
                {
                    pk
                });

            return transportOrderService.FirstOrDefault();
        }
    }

    public async Task<int> Outdate(TransportOrderService service)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                UPDATE [dbo].[TransportOrderService]
                   SET [TransportOrderServicePK] = @TransportOrderServicePK
                      ,[isActive] = @isActive
                      ,[SucceedingVersionPK] = @SucceedingVersionPK
                      ,[Timestamp] = @Timestamp
                 WHERE [TransportOrderServicePK] = @TransportOrderServicePK",
                 service);
        }
    }

    public async Task<int> Add(TransportOrderService service)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[TransportOrderService]
                           ([TransportOrderServicePK]
                           ,[isActive]
                           ,[Name]
                           ,[Price]
                           ,[CurrencyPK]
                           ,[SucceedingVersionPK]
                           ,[Timestamp]
                           ,[AgentPK]
                            )
                     VALUES
                           (@TransportOrderServicePK
                           ,@isActive
                           ,@Name
                           ,@Price
                           ,@CurrencyPK
                           ,@SucceedingVersionPK
                           ,@Timestamp
                           ,@AgentPK)", service);
        }
    }

    public async Task<Guid> Delete(Guid id)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                  UPDATE [dbo].[TransportOrderService]
                  SET [isActive] = 0
                  WHERE [TransportOrderServicePK] = @id",
              new
              {
                  id
              });
        }

        return id;
    }

}