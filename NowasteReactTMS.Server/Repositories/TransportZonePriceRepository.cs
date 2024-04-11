using Dapper;
using NowasteReactTMS.Server.Models.TransportDTOs;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class TransportZonePriceRepository : ITransportZonePriceRepository
{
    private readonly IConnectionFactory connectionFactory;
    private readonly ITransportZonePriceLineRepository transportZonePriceLineRepository;

    public TransportZonePriceRepository(IConnectionFactory connectionFactory, ITransportZonePriceLineRepository transportZonePriceLineRepository)
    {
        this.connectionFactory = connectionFactory;
        this.transportZonePriceLineRepository = transportZonePriceLineRepository;
    }

    public async Task<List<TransportZonePrice>> GetAll(bool includeInactive = false)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportZonePrice = await connection.QueryAsync<TransportZonePrice, Currency, TransportZone, TransportZone, TransportType, TransportZonePrice>(@"
                SELECT tzp.[TransportZonePricePK]
                      ,tzp.[AgentPK]
                      ,tzp.[FromTransportZonePK]
                      ,tzp.[ToTransportZonePK]
                      ,tzp.[Description]
                      ,tzp.[Price]
                      ,tzp.[CurrencyPK]
                      ,tzp.[TransportTypePK]
                      ,tzp.[ValidFrom]
                      ,tzp.[ValidTo]

                      ,[cu].[CurrencyPK] AS Id
                      ,[cu].[Name]
                      ,[cu].[ShortName]

                      ,tz1.TransportZonePK AS Id
                      ,tz1.TransportZonePK
                      ,tz1.Name
                      ,tz1.Description

                      ,tz2.TransportZonePK AS Id
                      ,tz2.TransportZonePK
                      ,tz2.Name
                      ,tz2.Description

                      ,tt.TransportTypePK AS Id
                      ,tt.TransportTypePK
                      ,tt.Description
                      ,tt.PalletTypeId
                  FROM [dbo].[TransportZonePrice] tzp
                JOIN [dbo].[Currency] cu ON cu.CurrencyPK = tzp.CurrencyPK
                JOIN [dbo].[TransportZone] tz1 ON tz1.TransportZonePK = tzp.FromTransportZonePK
                JOIN [dbo].[TransportZone] tz2 ON tz2.TransportZonePK = tzp.ToTransportZonePK
                JOIN [dbo].[TransportType] tt ON tt.TransportTypePK = tzp.TransportTypePK
                WHERE tzp.[IsActive] = 1 OR @includeInactive = 1",
                (tzp, cu, tz1, tz2, tt) =>
                {
                    tzp.Currency = cu;
                    tzp.FromTransportZone = tz1;
                    tzp.ToTransportZone = tz2;
                    tzp.TransportType = tt;
                    // TODO: Include pallet type object

                    return tzp;
                },
                new
                {
                    includeInactive
                });

            return transportZonePrice.ToList();
        }
    }

    public async Task<int> Remove(Guid pk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                UPDATE [dbo].[TransportZonePrice]
                   SET [isActive] = 0
                 WHERE [TransportZonePricePK] = @pk",
                new
                {
                    pk
                });
        }
    }

    public async Task<int> Add(TransportPriceDTO dtoTransportPrice)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[TransportZonePrice]
                           ([TransportZonePricePK]
                           ,[AgentPK]
                           ,[FromTransportZonePK]
                           ,[ToTransportZonePK]
                           ,[Description]
                           ,[Price]
                           ,[CurrencyPK]
                           ,[TransportTypePK]
                           ,[ValidFrom]
                           ,[ValidTo])
                     VALUES
                           (
                            @TransportZonePricePK
                           ,@AgentPK
                           ,@FromTransportZonePK
                           ,@ToTransportZonePK
                           ,@Description
                           ,@Price
                           ,@CurrencyPK
                           ,@TransportTypePK
                           ,@ValidFrom
                           ,@ValidTo)",
                dtoTransportPrice);
        }
    }

    public async Task<TransportZonePrice> Get(Guid id)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var result = await connection.QueryAsync<TransportZonePrice, Currency, TransportZone, TransportZone, TransportType, TransportZonePrice>(@"
                SELECT tzp.[TransportZonePricePK]
                      ,tzp.[AgentPK]
                      ,tzp.[FromTransportZonePK]
                      ,tzp.[ToTransportZonePK]
                      ,tzp.[Description]
                      ,tzp.[Price]
                      ,tzp.[CurrencyPK]
                      ,tzp.[TransportTypePK]
                      ,tzp.[ValidFrom]
                      ,tzp.[ValidTo]

                      ,[cu].[CurrencyPK] AS Id
                      ,[cu].[Name]
                      ,[cu].[ShortName]

                      ,tz1.TransportZonePK AS Id
                      ,tz1.TransportZonePK
                      ,tz1.Name
                      ,tz1.Description

                      ,tz2.TransportZonePK AS Id
                      ,tz2.TransportZonePK
                      ,tz2.Name
                      ,tz2.Description

                      ,tt.TransportTypePK AS Id
                      ,tt.TransportTypePK
                      ,tt.Description
                      ,tt.PalletTypeId
                  FROM [dbo].[TransportZonePrice] tzp
                JOIN [dbo].[Currency] cu ON cu.CurrencyPK = tzp.CurrencyPK
                JOIN [dbo].[TransportZone] tz1 ON tz1.TransportZonePK = tzp.FromTransportZonePK
                JOIN [dbo].[TransportZone] tz2 ON tz2.TransportZonePK = tzp.ToTransportZonePK
                JOIN [dbo].[TransportType] tt ON tt.TransportTypePK = tzp.TransportTypePK
                WHERE [TransportZonePricePK] = @id",
                (tzp, cu, tz1, tz2, tt) =>
                {
                    tzp.Currency = cu;
                    tzp.FromTransportZone = tz1;
                    tzp.ToTransportZone = tz2;
                    tzp.TransportType = tt;
                    // TODO: Include pallet type object

                    return tzp;
                }
                , new
                {
                    id
                });

            var transportZonePrice = result.SingleOrDefault();
            if (transportZonePrice == null)
                return null;

            transportZonePrice.TransportZonePriceLines = (await transportZonePriceLineRepository.GetByTransportZonePricePk(transportZonePrice.TransportZonePricePK)).ToList();

            return transportZonePrice;
        }
    }

    public async Task<List<List<TransportZonePrice>>> Get(GetBestPricesQuery query)
    {
        if (query?.TransportZoneGuids == null || !query.TransportZoneGuids.Any())
            return null;

        var prices = new List<TransportZonePrice>();
        foreach (var zonePair in query.TransportZoneGuids)
        {
            prices.AddRange(await GetPrices(zonePair.Item1, zonePair.Item2));
        }

        var groupedPrices = prices.GroupBy(x => new { x.AgentPK, x.CurrencyPK });
        var priceOffers = new List<List<TransportZonePrice>>();
        foreach (var agentPrices in groupedPrices)
        {
            // Agent offers transport between all zones
            if (agentPrices.Count() == query.TransportZoneGuids.Count)
                priceOffers.Add(agentPrices.ToList());
        }

        return priceOffers;
    }

    public async Task<List<List<TransportZonePrice>>> GetGrouped(GetBestPricesQuery query)
    {
        if (query?.TransportZoneGuids == null || !query.TransportZoneGuids.Any())
            return null;

        var prices = new List<TransportZonePrice>();
        foreach (var zonePair in query.TransportZoneGuids)
            prices.AddRange(await GetGroupedPrices(zonePair.Item1, zonePair.Item2));

        var groupedPrices = prices.GroupBy(x => new { x.AgentPK, x.CurrencyPK });

        var priceOffers = new List<List<TransportZonePrice>>();
        foreach (var agentPrices in groupedPrices)
            priceOffers.Add(agentPrices.Select(x => x).ToList());

        return priceOffers;
    }

    public async Task<TransportZonePrice> Update(Guid id, TransportZonePrice transportZonePrice)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [dbo].[TransportZonePrice]
                   SET [TransportZonePricePK] = @TransportZonePricePK
                      ,[AgentPK] = @AgentPK
                      ,[FromTransportZonePK] = @FromTransportZonePK
                      ,[ToTransportZonePK] = @ToTransportZonePK
                      ,[Description] = @Description
                      ,[Price] = @Price
                      ,[CurrencyPK] = @CurrencyPK
                      ,[TransportTypePK] = @TransportTypePK
                      ,[ValidFrom] = @ValidFrom
                      ,[ValidTo] = @ValidTo 
                 WHERE [TransportZonePricePK] = @TransportZonePricePK",
                transportZonePrice);
        }
        foreach (var transportZonePriceLine in transportZonePrice.TransportZonePriceLines)
        {
            transportZonePriceLine.TransportZonePricePK = transportZonePrice.TransportZonePricePK;
            await transportZonePriceLineRepository.Update(transportZonePriceLine);
        }

        return transportZonePrice;
    }

    public async Task<IEnumerable<TransportZonePrice>> GetPrices(Guid fromTransportZonePK, Guid toTransportZonePK)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportZonePrices = await connection.QueryAsync<TransportZonePrice, Currency, TransportZone, TransportZone, TransportType, TransportZonePrice>(@"
                SELECT tzp.[TransportZonePricePK]
                      ,tzp.[AgentPK]
                      ,tzp.[FromTransportZonePK]
                      ,tzp.[ToTransportZonePK]
                      ,tzp.[Description]
                      ,tzp.[Price]
                      ,tzp.[CurrencyPK]
                      ,tzp.[TransportTypePK]
                      ,tzp.[ValidFrom]
                      ,tzp.[ValidTo]

                      ,[cu].[CurrencyPK] AS Id
                      ,[cu].[CurrencyPK]
                      ,[cu].[Name]
                      ,[cu].[ShortName]

                      ,tz1.TransportZonePK AS Id
                      ,tz1.TransportZonePK
                      ,tz1.Name
                      ,tz1.Description

                      ,tz2.TransportZonePK AS Id
                      ,tz2.TransportZonePK
                      ,tz2.Name
                      ,tz2.Description

                      ,tt.TransportTypePK AS Id
                      ,tt.TransportTypePK
                      ,tt.Description

                  FROM [dbo].[TransportZonePrice] tzp
                JOIN [dbo].[Currency] cu ON cu.CurrencyPK = tzp.CurrencyPK
                JOIN [dbo].[TransportZone] tz1 ON tz1.[TransportZonePK] = tzp.[FromTransportZonePK]
                JOIN [dbo].[TransportZone] tz2 ON tz2.[TransportZonePK] = tzp.[ToTransportZonePK]
                JOIN [dbo].[TransportType] tt ON tt.[TransportTypePK] = tzp.[TransportTypePK]
                JOIN [dbo].[Agent] a ON a.[AgentPK] = tzp.[AgentPK]
                WHERE tz1.[TransportZonePK] = @fromTransportZonePK
                  AND tz2.[TransportZonePK] = @toTransportZonePK
                  AND a.[isActive] = 1
                  AND tzp.[IsActive] = 1
                  AND tt.[TransportTypePK] = 'A42F861A-2314-4399-9266-65A98ED95011'
                ORDER BY tzp.[Price] ASC",
                (tzp, cu, tz1, tz2, tt) =>
                {
                    tzp.Currency = cu;
                    tzp.FromTransportZone = tz1;
                    tzp.ToTransportZone = tz2;
                    tzp.TransportType = tt;

                    return tzp;
                }
                , new
                {
                    fromTransportZonePK,
                    toTransportZonePK
                });

            return transportZonePrices;
        }
    }

    public async Task<IEnumerable<TransportZonePrice>> GetGroupedPrices(Guid fromTransportZonePK, Guid toTransportZonePK)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var transportZonePrices = await connection.QueryAsync<TransportZonePrice, Currency, TransportZone, TransportZone, TransportType, TransportZonePriceLine, TransportZonePrice>(@"
                SELECT tzp.[TransportZonePricePK]
                      ,tzp.[AgentPK]
                      ,tzp.[FromTransportZonePK]
                      ,tzp.[ToTransportZonePK]
                      ,tzp.[CurrencyPK]

                      ,[cu].[CurrencyPK] AS Id
                      ,[cu].[CurrencyPK]
                      ,[cu].[Name]
                      ,[cu].[ShortName]

                      ,tz1.TransportZonePK AS Id
                      ,tz1.TransportZonePK
                      ,tz1.Name
                      ,tz1.Description

                      ,tz2.TransportZonePK AS Id
                      ,tz2.TransportZonePK
                      ,tz2.Name
                      ,tz2.Description

                      ,tt.TransportTypePK AS Id
                      ,tt.TransportTypePK
                      ,tt.Description

                      ,tzpl.[TransportZonePricePK] AS Id
                      ,tzpl.[Price]
                      ,tzpl.[PalletTypeId] 

                  FROM [dbo].[TransportZonePrice] tzp
                JOIN [dbo].[Currency] cu ON cu.CurrencyPK = tzp.CurrencyPK
                JOIN [dbo].[TransportZone] tz1 ON tz1.[TransportZonePK] = tzp.[FromTransportZonePK]
                JOIN [dbo].[TransportZone] tz2 ON tz2.[TransportZonePK] = tzp.[ToTransportZonePK]
                JOIN [dbo].[TransportType] tt ON tt.[TransportTypePK] = tzp.[TransportTypePK]
                JOIN [dbo].[Agent] a ON a.[AgentPK] = tzp.[AgentPK]
                JOIN [dbo].[TransportZonePriceLine] tzpl ON tzpl.[TransportZonePricePK] = tzp.[TransportZonePricePK] 
                WHERE tz1.[TransportZonePK] = @fromTransportZonePK
                  AND tz2.[TransportZonePK] = @toTransportZonePK
                  AND a.[isActive] = 1
                  AND tzp.[IsActive] = 1
                  AND tt.[TransportTypePK] = 'A42F861A-2314-4399-9266-65A98ED95012'
                ORDER BY tzp.[Price] ASC",
                (tzp, cu, tz1, tz2, tt, tzpl) =>
                {
                    tzp.Currency = cu;
                    tzp.FromTransportZone = tz1;
                    tzp.ToTransportZone = tz2;
                    tzp.TransportType = tt;
                    tzp.TransportZonePriceLine = tzpl;

                    return tzp;
                }
                , new
                {
                    fromTransportZonePK,
                    toTransportZonePK
                });

            return transportZonePrices;
        }
    }
}