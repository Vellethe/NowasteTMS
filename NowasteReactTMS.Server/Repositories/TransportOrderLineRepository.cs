using NowasteTms.Model;
using System.Data;
using Dapper;
using WMan.Data.ConnectionFactory;

public class TransportOrderLineRepository : ITransportOrderLineRepository
{
    private readonly IConnectionFactory connectionFactory;

    public TransportOrderLineRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<TransportOrderLine> Get(Guid pk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportOrderLine = await connection.QueryFirstAsync<TransportOrderLine>(@"
                SELECT [TransportOrderLinePK]
                      ,[TransportOrderPK]
                      ,[LineNumber]
                      ,[FromContactInformationPK]
                      ,[ToContactInformationPK]
                      ,[FromTransportZonePK]
                      ,[ToTransportZonePK]
                      ,[AgentPK]
                      ,[Price]
                      ,[ToCustomerName]
                      ,[FromSupplierName]
                  FROM [dbo].[TransportOrderLine]
                WHERE [TransportOrderLinePK] = @pk",
        new
        {
            pk
        });

            //
            // GetTransportOrder services
            //
            var serviceLinks = await GetTransportOrderServices(connection, new List<Guid> { transportOrderLine.TransportOrderLinePK });

            transportOrderLine.TransportOrderLineTransportOrderServices = serviceLinks;

            return transportOrderLine;
        }
    }

    public async Task<IEnumerable<TransportOrderLine>> GetLinesForTransportOrders(IEnumerable<Guid> transportOrderPKs)
    {
        if (!transportOrderPKs.Any())
            return new List<TransportOrderLine>();

        using (IDbConnection connection = connectionFactory.CreateConnection())
        {
            var transportOrderLines = await connection.QueryAsync<TransportOrderLine, ContactInformation, ContactInformation, TransportZone, TransportZone, Agent, BusinessUnit, TransportOrderLine>(@"
                SELECT TOP (SELECT COUNT(1) FROM dbo.[TransportOrderLine])
                     tol.TransportOrderLinePK AS Id
                    ,tol.[TransportOrderLinePK]
                    ,tol.[TransportOrderPK]
                    ,tol.[LineNumber]
                    ,tol.[FromContactInformationPK]
                    ,tol.[ToContactInformationPK]
                    ,tol.[FromTransportZonePK]
                    ,tol.[ToTransportZonePK]
                    ,tol.[AgentPK]
                    ,tol.[Price]
                    ,tol.[ToCustomerName]
                    ,tol.[FromSupplierName]

                    ,fci.ContactInformationPK AS Id
                    ,fci.*

                    ,tci.ContactInformationPK AS Id
                    ,tci.*

                    ,ftz.TransportZonePK AS Id
                    ,ftz.*
                    
                    ,ttz.TransportZonePK AS Id
                    ,ttz.*

                    ,a.[AgentPK] AS Id
                    ,a.[AgentPK]
                    ,a.[AgentID]
                    ,a.[BusinessUnitPK]

                    ,abu.[BusinessUnitPK] AS Id
                    ,abu.[BusinessUnitPK]
                    ,abu.[Name]
                    ,abu.[FinanceInformationPK]

              FROM [dbo].[TransportOrderLine] tol
                LEFT JOIN ContactInformation fci ON tol.FromContactInformationPK = fci.ContactInformationPK
                LEFT JOIN ContactInformation tci ON tol.ToContactInformationPK = tci.ContactInformationPK
                LEFT JOIN TransportZone ftz ON tol.FromTransportZonePK = ftz.TransportZonePK
                LEFT JOIN TransportZone ttz ON tol.ToTransportZonePK = ttz.TransportZonePK
                LEFT JOIN Agent a ON tol.AgentPK = a.AgentPK
                LEFT JOIN BusinessUnit abu ON abu.BusinessUnitPK = a.BusinessUnitPK
                WHERE tol.[TransportOrderPK] IN @transportOrderPKs"
      , (tol, fci, tci, ftz, ttz, a, abu) =>
      {
          // Checking PK values because of: https://github.com/StackExchange/Dapper/issues/222
          if (tol.FromContactInformationPK != null)
              tol.FromContactInformation = fci;

          if (tol.ToContactInformationPK != null)
              tol.ToContactInformation = tci;

          if (tol.FromTransportZonePK != null)
              tol.FromTransportZone = ftz;

          if (tol.ToTransportZonePK != null)
              tol.ToTransportZone = ttz;

          if (tol.AgentPK != null)
          {
              tol.Agent = a;
              tol.Agent.BusinessUnit = abu;
          }

          return tol;
      }
      , new
      {
          transportOrderPKs
      });

            //
            // GetTransportOrder services
            //
            var serviceLinks = await GetTransportOrderServices(connection, transportOrderLines.Select(x => x.TransportOrderLinePK));

            foreach (var line in transportOrderLines)
            {
                line.TransportOrderLineTransportOrderServices =
                    serviceLinks.Where(x => x.TransportOrderLinePK == line.TransportOrderLinePK);
            }

            return transportOrderLines.OrderBy(x => x.LineNumber);
        }
    }

    public async Task<IEnumerable<TransportOrderLine>> GetAll()
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportOrderLines =
                await connection
                    .QueryAsync<TransportOrderLine, ContactInformation, ContactInformation, TransportZone,
                        TransportZone, Agent, BusinessUnit, TransportOrderLine>(@"
                SELECT   tol.TransportOrderLinePK AS Id
                        ,tol.[TransportOrderLinePK]
                        ,tol.[TransportOrderPK]
                        ,tol.[LineNumber]
                        ,tol.[FromContactInformationPK]
                        ,tol.[ToContactInformationPK]
                        ,tol.[FromTransportZonePK]
                        ,tol.[ToTransportZonePK]
                        ,tol.[AgentPK]
                        ,tol.[Price]
                        ,tol.[ToCustomerName]
                        ,tol.[FromSupplierName]

                        ,fci.ContactInformationPK AS Id
                        ,fci.*

                        ,tci.ContactInformationPK AS Id
                        ,tci.*

                        ,ftz.TransportZonePK AS Id
                        ,ftz.*
                        
                        ,ttz.TransportZonePK AS Id
                        ,ttz.*

                        ,a.[AgentPK] AS Id
                        ,a.[AgentPK]
                        ,a.[AgentID]
                        ,a.[BusinessUnitPK]

                        ,abu.[BusinessUnitPK] AS Id
                        ,abu.[BusinessUnitPK]
                        ,abu.[Name]
                        ,abu.[FinanceInformationPK]

                  FROM [dbo].[TransportOrderLine] tol
                    LEFT JOIN ContactInformation fci ON tol.FromContactInformationPK = fci.ContactInformationPK
                    LEFT JOIN ContactInformation tci ON tol.ToContactInformationPK = tci.ContactInformationPK
                    LEFT JOIN TransportZone ftz ON tol.FromTransportZonePK = ftz.TransportZonePK
                    LEFT JOIN TransportZone ttz ON tol.ToTransportZonePK = ttz.TransportZonePK
                    LEFT JOIN Agent a ON tol.AgentPK = a.AgentPK
                    LEFT JOIN BusinessUnit abu ON abu.BusinessUnitPK = a.BusinessUnitPK"
                        , (tol, fci, tci, ftz, ttz, a, abu) =>
                        {
                            // Checking PK values because of: https://github.com/StackExchange/Dapper/issues/222
                            if (tol.FromContactInformationPK != null)
                                tol.FromContactInformation = fci;

                            if (tol.ToContactInformationPK != null)
                                tol.ToContactInformation = tci;

                            if (tol.FromTransportZonePK != null)
                                tol.FromTransportZone = ftz;

                            if (tol.ToTransportZonePK != null)
                                tol.ToTransportZone = ttz;

                            if (tol.AgentPK != null)
                            {
                                tol.Agent = a;
                                tol.Agent.BusinessUnit = abu;
                            }

                            return tol;
                        });

            //
            // GetTransportOrder services
            //
            var serviceLinks = await GetAllTransportOrderServices(connection);

            foreach (var line in transportOrderLines)
            {
                line.TransportOrderLineTransportOrderServices =
                    serviceLinks.Where(x => x.TransportOrderLinePK == line.TransportOrderLinePK);
            }

            return transportOrderLines.OrderByDescending(x => x.LineNumber);
        }
    }

    public async Task Add(IDbConnection conn, TransportOrderLine line)
    {
        await conn.ExecuteAsync(@"
            INSERT INTO [dbo].[TransportOrderLine]
                       ([TransportOrderLinePK]
                       ,[TransportOrderPK]
                       ,[LineNumber]
                       ,[FromContactInformationPK]
                       ,[ToContactInformationPK]
                       ,[FromTransportZonePK]
                       ,[ToTransportZonePK]
                       ,[AgentPK]
                       ,[Price]
                       ,[ToCustomerName]
                       ,[FromSupplierName])
                 VALUES
                        (@TransportOrderLinePK
                        ,@TransportOrderPK
                        ,@LineNumber
                        ,@FromContactInformationPK
                        ,@ToContactInformationPK
                        ,@FromTransportZonePK
                        ,@ToTransportZonePK
                        ,@AgentPK
                        ,@Price
                        ,@ToCustomerName
                        ,@FromSupplierName)", line);

        foreach (var serviceLink in line.TransportOrderLineTransportOrderServices)
        {
            await conn.ExecuteAsync(@"
                INSERT INTO [dbo].[TransportOrderLineTransportOrderService]
                           ([TransportOrderLinePK]
                           ,[TransportOrderServicePK])
                     VALUES
                           (@TransportOrderLinePK
                           ,@TransportOrderServicePK)", serviceLink);
        }
    }

    public async Task RemoveAll(IDbConnection conn, Guid transportOrderPK)
    {
        var transportOrderLinePKs = await conn.QueryAsync<Guid>(@"
                SELECT [TransportOrderLinePK]
                  FROM [dbo].[TransportOrderLine]
                      WHERE [TransportOrderPK] = @transportOrderPK",
            new
            {
                transportOrderPK
            });

        await conn.ExecuteAsync(@"
                DELETE FROM [dbo].[TransportOrderLineTransportOrderService]
                      WHERE [TransportOrderLinePK] IN @transportOrderLinePKs",
            new
            {
                transportOrderLinePKs
            });

        await conn.ExecuteAsync(@"
                DELETE FROM [dbo].[TransportOrderLine]
                      WHERE [TransportOrderPK] = @transportOrderPK",
            new
            {
                transportOrderPK
            });
    }

    private static async Task<IEnumerable<TransportOrderLineTransportOrderService>> GetTransportOrderServices(IDbConnection connection, IEnumerable<Guid> transportOrderLinePKs)
    {
        var serviceLinks = await connection.QueryAsync<TransportOrderLineTransportOrderService>(@"
                SELECT [TransportOrderLinePK]
                      ,[TransportOrderServicePK]
                  FROM [dbo].[TransportOrderLineTransportOrderService] toltos
                    WHERE toltos.[TransportOrderLinePK] IN @transportOrderLinePKs ",
            new
            {
                transportOrderLinePKs
            });

        return serviceLinks;
    }

    private static async Task<IEnumerable<TransportOrderLineTransportOrderService>> GetAllTransportOrderServices(IDbConnection connection)
    {
        var serviceLinks = await connection.QueryAsync<TransportOrderLineTransportOrderService>(@"
                SELECT [TransportOrderLinePK]
                      ,[TransportOrderServicePK]
                  FROM [dbo].[TransportOrderLineTransportOrderService] toltos");

        return serviceLinks;
    }
}