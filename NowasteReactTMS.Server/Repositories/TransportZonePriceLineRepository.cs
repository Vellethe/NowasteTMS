using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class TransportZonePriceLineRepository : ITransportZonePriceLineRepository
{
    private readonly IConnectionFactory connectionFactory;

    public TransportZonePriceLineRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<int> Add(TransportZonePriceLine transportZonePriceLine)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[TransportZonePriceLine]
                           ([TransportZonePriceLinePK]
                           ,[TransportZonePricePK]
                           ,[PalletTypeId]
                           ,[Price])
                     VALUES
                           (@TransportZonePriceLinePK
                           ,@TransportZonePricePK
                           ,@PallettypeId
                           ,@Price)",
               transportZonePriceLine);
        }
    }
    public async Task<int> Update(TransportZonePriceLine transportZonePriceLine)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                UPDATE [dbo].[TransportZonePriceLine]
                   SET [Price] = @Price
                 WHERE [TransportZonePriceLinePK] = @TransportZonePriceLinePK",
                transportZonePriceLine
            );
        }
    }

    public async Task<IEnumerable<TransportZonePriceLine>> GetByTransportZonePricePk(Guid pk)
    {
        using (var conn = connectionFactory.CreateConnection())
        {
            var lines = await conn.QueryAsync<TransportZonePriceLine, PalletType, TransportZonePriceLine>(@"
                SELECT tzl.[TransportZonePriceLinePK]
                      ,tzl.[TransportZonePricePK]
                      ,tzl.[PalletTypeId]
                      ,tzl.[Price]

                      ,pt.[Id]
                      ,pt.[Description]
                      ,pt.[Stamp]
                      ,pt.[Footprint]
                      ,pt.[ItemNo]
                      ,pt.[ItemNoBuyer]
                      ,pt.[ItemNoEan]
                      ,pt.[ItemNoSupplier]
                  FROM [dbo].[TransportZonePriceLine] tzl
                JOIN [dbo].[PalletType] pt ON tzl.[PalletTypeId] = pt.[Id]
                JOIN [dbo].[TransportZonePrice] tzp ON tzp.[TransportZonePricePK] = tzl.[TransportZonePricePK]
                WHERE tzp.[TransportZonePricePK] = @pk
                ", (l, palletType) =>
            {
                l.PalletType = palletType;

                return l;
            },
                new
                {
                    pk
                });

            return lines;
        }
    }
}