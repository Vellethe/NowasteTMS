using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class PalletInventoryRepository : IPalletInventoryRepository
{
    private readonly IConnectionFactory connectionFactory;

    public PalletInventoryRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<string> GetPalletTypeDescription(long? palletTypeId)
    {
        var palletTypes = await GetInventory();
        var palletTypeDescription = palletTypes.SingleOrDefault(x => x.Id == palletTypeId)?.Description;
        return palletTypeDescription ?? "<!-- No pallet type found for id " + palletTypeId + ". -->";
    }

    /// <summary>
    /// Gets a list of all pallet types.
    /// </summary>
    /// <returns></returns>
    public async Task<List<PalletType>> GetInventory()
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var palletType = await connection.QueryAsync<PalletType>(@"
                SELECT [Id]
                      ,[Description]
                      ,[Stamp]
                      ,[Footprint]
                      ,[ItemNo]
                      ,[ItemNoBuyer]
                      ,[ItemNoEan]
                      ,[ItemNoSupplier]
                FROM [dbo].[PalletType]");

            return palletType.ToList();
        }
    }

    public async Task<IEnumerable<PalletStock>> GetPreviousInventoryPalletStock(long palletAccountId, DateTime date)
    {
        var dateFormat = date.ToString("s");
        using (var connection = connectionFactory.CreateConnection())
        {
            var result = await connection.QueryAsync<PalletStock>(@"
                SELECT [Id]
                      ,[PalletAccountId]
                      ,[ps].[PalletTypeId]
                      ,[NumberOfPallets]
                      ,[StockCountStamp]
                      ,[Stamp]
                  FROM [dbo].[PalletStock] ps
                INNER JOIN (SELECT [PalletTypeId], MAX([StockCountStamp]) AS MaxCountStamp
                            FROM [dbo].[PalletStock]
                            WHERE [PalletAccountId] = @palletAccountId
                                AND [StockCountStamp] <= @dateFormat
                            GROUP BY [PalletTypeId]) AS ps2
                ON (ps.[PalletTypeId]= ps2.[PalletTypeId] AND ps.[StockCountStamp]= ps2.MaxCountStamp)				
                WHERE [PalletAccountId] = @palletAccountId
                ",
                new
                {
                    palletAccountId,
                    dateFormat
                });

            return result;
        }
    }

    //public async Task<int> PostInventory(InventoryViewModel viewModel)
    //{
    //    var palletStock = new PalletStock
    //    {
    //        PalletAccountId = viewModel.Inventory.PalletAccountId,
    //        PalletTypeId = viewModel.Inventory.PalletTypeId,
    //        NumberOfPallets = viewModel.Inventory.Quantity,
    //        StockCountStamp = DateTime.Now
    //    };

    //    using (var connection = connectionFactory.CreateConnection())
    //    {
    //        var returncode = await connection.ExecuteAsync(@"
    //            INSERT INTO [dbo].[PalletStock]
    //                       ([PalletAccountId]
    //                       ,[PalletTypeId]
    //                       ,[NumberOfPallets]
    //                       ,[StockCountStamp]
    //                       ,[Stamp])
    //                 VALUES
    //                       (@PalletAccountId
    //                       ,@PalletTypeId
    //                       ,@NumberOfPallets
    //                       ,@StockCountStamp
    //                       ,GETDATE())
    //                       ",
    //             palletStock);
    //        return returncode;
    //    }

    //}

    public async Task<IEnumerable<InventoryStock>> GetInventoryPalletStock(long palletAccountId, DateTime date)
    {
        var dateFormat = date.AddDays(1).ToString("s");
        using (var connection = connectionFactory.CreateConnection())
        {
            var result = await connection.QueryAsync<InventoryStock>(@"
                SELECT ps.[Id]
                      ,ps.[PalletAccountId]
                      ,ps.[PalletTypeId]
					  ,pa.[Name]
					  ,pt.[Description] AS [PalletTypeName]
                      ,ps.[NumberOfPallets] AS [CountedStock]
                      ,ps.[StockCountStamp]
				FROM [PalletStock] ps
				LEFT JOIN [PalletAccount] pa ON ps.[PalletAccountId] = pa.[Id]
				LEFT JOIN [PalletType] pt ON ps.[PalletTypeId] = pt.[Id] 
                INNER JOIN (SELECT [PalletTypeId], MAX([StockCountStamp]) AS MaxCountStamp
                            FROM [dbo].[PalletStock]
                            WHERE [PalletAccountId] = @palletAccountId
                                AND [StockCountStamp] <= @dateFormat
                            GROUP BY [PalletTypeId]) AS ps2
                ON (ps.[PalletTypeId]= ps2.[PalletTypeId] AND ps.[StockCountStamp]= ps2.MaxCountStamp)				
                WHERE [PalletAccountId] = @palletAccountId

                ", new
            {
                palletAccountId,
                dateFormat
            });

            return result;

        }
    }

}