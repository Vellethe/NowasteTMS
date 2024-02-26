using Dapper;
using NowasteTms.Model;
using System.Data.SqlClient;
using System.Data;
using WMan.Data.ConnectionFactory;
public class OrderLineRepository : IOrderLineRepository
{
    private readonly IConnectionFactory connectionFactory;

    public OrderLineRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<OrderLine> Add(OrderLine line)
    {
        using (var conn = connectionFactory.CreateConnection())
        {
            await conn.ExecuteAsync(@"
                INSERT INTO [dbo].[OrderLine]
                           ([OrderLinePK]
                           ,[OrderPK]
                           ,[LineNumber]
                           ,[ItemPK]
                           ,[PalletQty]
                           ,[PalletTypeId]
                           ,[ItemQty]
                           ,[ItemName]
                           ,[TotalNetPrice])
                     VALUES
                           (@OrderLinePK
                           ,@OrderPK
                           ,@LineNumber
                           ,@ItemPK
                           ,@PalletQty
                           ,@PalletTypeId
                           ,@ItemQty
                           ,@ItemName
                           ,@TotalNetPrice)", line);
            return line;
        }
    }

    public async Task<IEnumerable<OrderLine>> Get()
    {
        using (var conn = connectionFactory.CreateConnection())
        {
            var lines = await conn.QueryAsync<OrderLine, Item, PalletType, OrderLine>(@"
                SELECT ol.[OrderLinePK]
                      ,ol.[OrderPK]
                      ,ol.[LineNumber]
                      ,ol.[ItemPK]
                      ,ol.[PalletQty]
                      ,ol.[PalletTypeId]
                      ,ol.[ItemQty]
                      ,ol.[ItemName]
                      ,ol.[TotalNetPrice]

                      ,i.[ItemPK] AS Id
                      ,i.[ItemPK]
                      ,i.[ItemID]
                      ,CASE WHEN i.[EditName] = 1 THEN ol.[ItemName] ELSE i.[Name] END AS [Name]
                      ,i.[TransportTemp]
                      ,i.[StorageTemp]
                      ,i.[Company]
                      ,i.[Weight]

                      ,pt.[Id]
                      ,pt.[Description]
                      ,pt.[FootPrint]
                      ,pt.[Stamp]

                  FROM [dbo].[OrderLine] ol
                LEFT JOIN [dbo].[Item] i ON i.[ItemPK] = ol.[ItemPK]
                JOIN [dbo].[PalletType] pt ON ol.[PalletTypeId] = pt.[Id]
                ", (line, item, palletType) =>
            {
                if (item.ItemPK != Guid.Empty)
                {
                    line.Item = item;
                }

                line.PalletType = palletType;

                return line;
            });

            return lines;
        }
    }

    public async Task<Dictionary<Guid, List<OrderLine>>> Get(HashSet<Guid> pks)
    {
        using var conn = connectionFactory.CreateConnection();
        conn.Open();

        await conn.ExecuteAsync(@"CREATE TABLE #tempIds([Id] uniqueidentifier not null primary key);");

        SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)conn);
        bulkCopy.DestinationTableName = "#tempIds";
        DataTable dataTable = new DataTable();
        dataTable.Columns.Add("[Id]", typeof(Guid));
        foreach (var id in pks)
            dataTable.Rows.Add(id);
        await bulkCopy.WriteToServerAsync(dataTable);

        var result = await conn.QueryAsync<OrderLine, Item, PalletType, OrderLine>(@"
                SELECT ol.[OrderLinePK]
                      ,ol.[OrderPK]
                      ,ol.[LineNumber]
                      ,ol.[ItemPK]
                      ,ol.[PalletQty]
                      ,ol.[PalletTypeId]
                      ,ol.[ItemQty]
                      ,ol.[ItemName]
                      ,ol.[TotalNetPrice]

                      ,i.[ItemPK] AS Id
                      ,i.[ItemPK]
                      ,i.[ItemID]
                      ,CASE WHEN i.[EditName] = 1 THEN ol.[ItemName] ELSE i.[Name] END AS [Name]
                      ,i.[TransportTemp]
                      ,i.[StorageTemp]
                      ,i.[Company]
                      ,i.[Weight]

                      ,pt.[Id]
                      ,pt.[Description]
                      ,pt.[FootPrint]
                      ,pt.[Stamp]

                  FROM [dbo].[OrderLine] ol
                    JOIN #tempIds ids on ids.Id = ol.OrderPK
                    LEFT JOIN [dbo].[Item] i ON i.[ItemPK] = ol.[ItemPK]
                    JOIN [dbo].[PalletType] pt ON ol.[PalletTypeId] = pt.[Id]
                ", (line, item, palletType) =>
        {
            if (item.ItemPK != Guid.Empty)
            {
                line.Item = item;
            }

            line.PalletType = palletType;

            return line;
        });

        return result
            .GroupBy(p => p.OrderPK)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.ToList());
    }

    public async Task<IReadOnlyCollection<OrderLine>> GetByOrder(Guid pk)
    {
        using (var conn = connectionFactory.CreateConnection())
        {
            var lines = await conn.QueryAsync<OrderLine, Item, PalletType, OrderLine>(@"
                SELECT ol.[OrderLinePK]
                      ,ol.[OrderPK]
                      ,ol.[LineNumber]
                      ,ol.[ItemPK]
                      ,ol.[PalletQty]
                      ,ol.[PalletTypeId]
                      ,ol.[ItemQty]
                      ,ol.[ItemName]
                      ,ol.[TotalNetPrice]

                      ,i.[ItemPK] AS Id
                      ,i.[ItemPK]
                      ,i.[ItemID]
                      ,CASE WHEN i.[EditName] = 1 THEN ol.[ItemName] ELSE i.[Name] END AS [Name]
                      ,i.[TransportTemp]
                      ,i.[StorageTemp]
                      ,i.[Company]
                      ,i.[Weight]

                      ,pt.[Id]
                      ,pt.[Description]
                      ,pt.[FootPrint]
                      ,pt.[Stamp]

                  FROM [dbo].[OrderLine] ol
                LEFT JOIN [dbo].[Item] i ON i.[ItemPK] = ol.[ItemPK]
                JOIN [dbo].[PalletType] pt ON ol.[PalletTypeId] = pt.[Id]
                JOIN [dbo].[Order] o ON o.[OrderPK] = ol.[OrderPK]
                WHERE o.[OrderPK] = @pk
                ", (l, item, palletType) =>
            {
                if (item.ItemPK != Guid.Empty)
                {
                    l.Item = item;
                }

                l.PalletType = palletType;

                return l;
            },
                new
                {
                    pk
                });

            return lines.AsList();
        }
    }

    public async Task<OrderLine> Get(Guid id)
    {
        using (var conn = connectionFactory.CreateConnection())
        {
            var line = await conn.QueryAsync<OrderLine, Item, PalletType, OrderLine>(@"
                SELECT ol.[OrderLinePK]
                      ,ol.[OrderPK]
                      ,ol.[LineNumber]
                      ,ol.[ItemPK]
                      ,ol.[PalletQty]
                      ,ol.[PalletTypeId]
                      ,ol.[ItemQty]
                      ,ol.[ItemName]
                      ,ol.[TotalNetPrice]

                      ,i.[ItemPK] AS Id
                      ,i.[ItemPK]
                      ,i.[ItemID]
                      ,CASE WHEN i.[EditName] = 1 THEN ol.[ItemName] ELSE i.[Name] END AS [Name]
                      ,i.[TransportTemp]
                      ,i.[StorageTemp]
                      ,i.[Company]
                      ,i.[Weight]

                      ,pt.[Id]
                      ,pt.[Description]
                      ,pt.[FootPrint]
                      ,pt.[Stamp]

                  FROM [dbo].[OrderLine] ol
                LEFT JOIN [dbo].[Item] i ON i.[ItemPK] = ol.[ItemPK]
                JOIN [dbo].[PalletType] pt ON ol.[PalletTypeId] = pt.[Id]
                WHERE ol.[OrderLinePK] = @id",
                (l, item, palletType) =>
                {
                    if (item.ItemPK != Guid.Empty)
                        l.Item = item;

                    l.PalletType = palletType;

                    return l;
                },
                new
                {
                    id
                });

            return line.FirstOrDefault();
        }
    }

    public async Task<int> Add(Guid id, OrderLine line)
    {
        using (var conn = connectionFactory.CreateConnection())
        {
            return await conn.ExecuteAsync(@"
                INSERT INTO [dbo].[OrderLine]
			                ([OrderLinePK],
			                [OrderPK],
			                [LineNumber],
			                [ItemPK],
			                [PalletQty],
			                [PalletTypeId],
			                [ItemQty],
			                [ItemName],
			                [TotalNetPrice],
			                [PalletPrice])
                VALUES
			                (@OrderLinePK,
			                @OrderPK,
			                @LineNumber,
			                @ItemPK,
			                @PalletQty,
			                @PalletTypeId,
			                @ItemQty,
			                @ItemName,
			                @TotalNetPrice,
			                @PalletPrice)", line);
        }
    }

    public async Task<int> Update(Guid id, OrderLine line)
    {
        using (var conn = connectionFactory.CreateConnection())
        {
            return await conn.ExecuteAsync(@"
                UPDATE [dbo].[OrderLine]
                   SET [OrderLinePK] = @OrderLinePK
                      ,[OrderPK] = @OrderPK
                      ,[LineNumber] = @LineNumber
                      ,[ItemPK] = @ItemPK
                      ,[PalletQty] = @PalletQty
                      ,[PalletTypeId] = @PalletTypeId
                      ,[ItemQty] = @ItemQty
                      ,[ItemName] = @ItemName
                      ,[TotalNetPrice] = @TotalNetPrice
                 WHERE [OrderLinePK] = @OrderLinePK", line);
        }
    }

    public async Task<OrderLine> Remove(Guid pk)
    {
        using (var conn = connectionFactory.CreateConnection())
        {
            var line = await Get(pk);

            await conn.ExecuteAsync(@"
                DELETE FROM [dbo].[OrderLine]
                      WHERE [OrderLinePK] = @pk",
                new
                {
                    pk
                });

            return line;
        }
    }
}