using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;

public class PalletReceiptRepository : IPalletReceiptRepository
{
    private readonly IConnectionFactory connectionFactory;

    // Match sorting and filtering keywords to column names
    private readonly Dictionary<string, string> columnMapping = new Dictionary<string, string>
        {
            {"CreatedDate", "[CreateStamp]"},
            {"Id", "pr.[Id]"},
            {"OrderNumber", "[OrderNumber]"},
            {"SenderName", "spa.[Name]"},
            {"TransporterName", "tpa.[Name]"},
            {"ReceiverName", "rpa.[Name]"},
            {"NumberOfPallets", "NumberOfPallets" }
        };

    public PalletReceiptRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    //public async Task<List<PalletReceiptItemViewModel>> GetPalletReceipts()
    //{
    //    using (var connection = connectionFactory.CreateConnection())
    //    {
    //        var palletReceipts = await connection.QueryAsync<PalletReceipt>(@"
    //            SELECT 
		  //           pr.[Id]
    //                ,pr.[CompanyId]
    //                ,pr.[IssuerPalletAccountId]
    //                ,pr.[TransporterPalletAccountId]
    //                ,pr.[PalletAccountId]
    //                ,pr.[OrderNumber]
    //                ,pr.[PalletTypeId]
    //                ,pr.[NumberOfPallets]
    //                ,pr.[CarPlateNumber]
    //                ,pr.[Comment]
    //                ,pr.[CreateTransactionId]
    //                ,pr.[CreateStamp]
    //                ,pr.[UtilizeTransactionId]
    //                ,pr.[UtilizeStamp]
    //                ,pr.[Stamp]
    //                ,pr.[UserId]
    //            FROM [PalletReceipt] pr
    //            LEFT JOIN [dbo].[PalletAccount] pa ON pa.[Id] = pr.[IssuerPalletAccountId] 
    //            WHERE pa.[IsActive] = 1
           
    //            ");

    //        List<PalletReceiptItemViewModel> result = new();

    //        foreach (var receipt in palletReceipts)
    //        {
    //            var receiptItemViewModel = new PalletReceiptItemViewModel
    //            {
    //                Id = receipt.Id,
    //                CompanyId = receipt.CompanyId,
    //                IssuerPalletAccountId = receipt.IssuerPalletAccountId,
    //                TransporterPalletAccountId = receipt.TransporterPalletAccountId,
    //                PalletAccountId = receipt.PalletAccountId,
    //                OrderNumber = receipt.OrderNumber,
    //                PalletTypeId = receipt.PalletTypeId,
    //                NumberOfPallets = receipt.NumberOfPallets,
    //                CarPlateNumber = receipt.CarPlateNumber,
    //                Comment = receipt.Comment,
    //                CreateTransactionId = receipt.CreateTransactionId,
    //                CreateStamp = receipt.CreateStamp,
    //                UtilizeTransactionId = receipt.UtilizeTransactionId,
    //                UtilizeStamp = receipt.UtilizeStamp,
    //                Stamp = receipt.Stamp,
    //                UserId = receipt.UserId,
    //            };
    //            result.Add(receiptItemViewModel);
    //        }
    //        return result;
    //    }
    //}

    public async Task<PalletReceipt> GetPalletReceipt(string searchString)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var palletReceipt = await connection.QueryFirstOrDefaultAsync<PalletReceipt>(@"
                SELECT [Id]
                      ,[CompanyId]
                      ,[IssuerPalletAccountId]
                      ,[TransporterPalletAccountId]
                      ,[PalletAccountId]
                      ,[OrderNumber]
                      ,[PalletTypeId]
                      ,[NumberOfPallets]
                      ,[CarPlateNumber]
                      ,[Comment]
                      ,[CreateTransactionId]
                      ,[CreateStamp]
                      ,[UtilizeTransactionId]
                      ,[UtilizeStamp]
                      ,[Stamp]
                      ,[UserId]
                 FROM [dbo].[PalletReceipt]
                 WHERE [OrderNumber] = @searchString OR [Id] = @searchString ",
                new
                {
                    searchString
                });

            if (palletReceipt == null)
                throw new Exception($"No palletReceipt record found for{searchString}");

            return palletReceipt;
        }
    }

    //public async Task<PalletReceiptItemViewModel> GetUtilizedPalletReceipt(string palletReceiptId)
    //{
    //    using (var connection = connectionFactory.CreateConnection())
    //    {
    //        var palletReceiptRecord = await connection.QueryFirstOrDefaultAsync<PalletReceipt>(@"
    //            SELECT [Id]
    //                  ,[CompanyId]
    //                  ,[IssuerPalletAccountId]
    //                  ,[TransporterPalletAccountId]
    //                  ,[PalletAccountId]
    //                  ,[OrderNumber]
    //                  ,[PalletTypeId]
    //                  ,[NumberOfPallets]
    //                  ,[CarPlateNumber]
    //                  ,[Comment]
    //                  ,[CreateTransactionId]
    //                  ,[CreateStamp]
    //                  ,[UtilizeTransactionId]
    //                  ,[UtilizeStamp]
    //                  ,[Stamp]
    //                  ,[UserId]
    //             FROM [dbo].[PalletReceipt]
    //             WHERE [Id] = @palletReceiptId",
    //            new
    //            {
    //                palletReceiptId
    //            });

    //        if (palletReceiptRecord == null)
    //            throw new Exception($"No palletReceipt record found for{palletReceiptId}");

    //        var palletReceipt = new PalletReceiptItemViewModel
    //        {
    //            NumberOfPallets = palletReceiptRecord.NumberOfPallets,
    //            PalletTypeId = palletReceiptRecord.PalletTypeId,
    //            Id = palletReceiptRecord.Id,
    //            OrderNumber = palletReceiptRecord.OrderNumber,
    //            UtilizeTransactionId = palletReceiptRecord.UtilizeTransactionId
    //        };

    //        return palletReceipt;
    //    }
    //}

    public async Task<long> PostPalletReceiptTransaction(PalletReceipt palletReceipt)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            palletReceipt.CreateStamp = DateTime.Now;
            palletReceipt.Stamp = palletReceipt.CreateStamp;

            var id = await connection.QueryFirstAsync<long>(@"
                INSERT INTO [dbo].[PalletReceipt]
                           ([CompanyId]
                           ,[IssuerPalletAccountId]
                           ,[TransporterPalletAccountId]
                           ,[PalletAccountId]
                           ,[OrderNumber]
                           ,[PalletTypeId]
                           ,[NumberOfPallets]
                           ,[CarPlateNumber]
                           ,[Comment]
                           ,[CreateTransactionId]
                           ,[CreateStamp]
                           ,[Stamp]
                           ,[UserId]
                            )
                    OUTPUT INSERTED.Id
                     VALUES
                           (@CompanyId
                           ,@IssuerPalletAccountId
                           ,@TransporterPalletAccountId
                           ,@PalletAccountId
                           ,@OrderNumber
                           ,@PalletTypeId
                           ,@NumberOfPallets
                           ,@CarPlateNumber
                           ,@Comment
                           ,@CreateTransactionId
                           ,@CreateStamp
                           ,@Stamp
                           ,@UserId
                            )
                            ",
                palletReceipt);
            return id;
        }
    }

    public async Task UtilizeReceipt(string id, long transactionId)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [dbo].[PalletReceipt]
                    SET [UtilizeTransactionId] = @TransactionId,
                        [UtilizeStamp] = @Now
                WHERE [Id] = @Id",
                new
                {
                    Id = id,
                    TransactionId = transactionId,
                    DateTime.Now,
                });
        }
    }

    public async Task<List<PalletReceipt>> SearchPalletReceipts(SearchParameters parameters, DateTime fromDate, DateTime toDate)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var builder = new SqlBuilder();
            var sqlTemplate = @"
                 SELECT pr.[Id]
                      ,pr.[CompanyId]
                      ,pr.[IssuerPalletAccountId]
                      ,spa.[Name] AS SenderName
                      ,pr.[TransporterPalletAccountId]
                      ,tpa.[Name] AS TransporterName
                      ,pr.[PalletAccountId] AS ReceiverPalletAccountId
                      ,rpa.[Name] AS ReceiverName
                      ,pr.[OrderNumber]
                      ,pr.[PalletTypeId]
                      ,pt.[Description] AS PalletTypeDescription
                      ,pr.[NumberOfPallets]
                      ,pr.[CarPlateNumber]
                      ,pr.[Comment]
                      ,pr.[CreateTransactionId]
                      ,pr.[CreateStamp]
                      ,pr.[UtilizeTransactionId]
                      ,pr.[UtilizeStamp]
                      ,pr.[Stamp]
                      ,pr.[UserId]
                 FROM [dbo].[PalletReceipt] pr
                 LEFT JOIN [dbo].[PalletAccount] spa ON spa.[Id] = pr.[IssuerPalletAccountId]
                 LEFT JOIN [dbo].[PalletAccount] rpa ON rpa.[Id] = pr.[PalletAccountId]
                 LEFT JOIN [dbo].[PalletAccount] tpa ON tpa.[Id] = pr.[TransporterPalletAccountId]
                 LEFT JOIN [dbo].[PalletType] pt ON pr.[PalletTypeId] = pt.[Id]
                 /**where**/
                 AND spa.[IsActive] = 1 
                 /**orderby**/";

            if (parameters.Limit < int.MaxValue)
            {
                sqlTemplate += @"
                        OFFSET @offset ROWS
                        FETCH NEXT @limit ROWS ONLY";
            }

            var template = builder.AddTemplate(sqlTemplate, new
            {
                offset = parameters.Offset,
                limit = parameters.Limit
            });

            builder.Where("([CreateStamp] BETWEEN @fromDate AND @toDate)", new { fromDate, toDate });
            foreach (var filter in parameters.Filters)
                builder.Where($"{columnMapping[filter.Key]} LIKE '%{filter.Value}%'");

            if (parameters.SortOrders != null && parameters.SortOrders.Any())
            {
                foreach (var column in parameters.SortOrders)
                    builder.OrderBy(columnMapping[column.Key] + " " + (column.Value ? "ASC" : "DESC"));
            }
            else // Forced default sort, OFFSET won't work otherwise
                builder.OrderBy("[Id] DESC");

            var result = await connection.QueryAsync<PalletReceipt>(template.RawSql, template.Parameters);

            return result.ToList();
        }
    }

    public async Task<int> SearchPalletReceiptsCount(SearchParameters parameters, DateTime fromDate, DateTime toDate)
    {
        using (var conn = connectionFactory.CreateConnection())
        {
            var builder = new SqlBuilder();
            var template = builder.AddTemplate(@"
                SELECT COUNT(*)
                FROM [dbo].[PalletReceipt] pr
                INNER JOIN [dbo].[PalletAccount] spa ON spa.[Id] = pr.[IssuerPalletAccountId]
                INNER JOIN [dbo].[PalletAccount] rpa ON rpa.[Id] = pr.[PalletAccountId]
                 /**where**/
                AND spa.[IsActive] = 1 AND rpa.[IsActive] = 1
                ");

            builder.Where("([CreateStamp] BETWEEN @fromDate AND @toDate)", new { fromDate, toDate });
            foreach (var filter in parameters.Filters)
                builder.Where($"{columnMapping[filter.Key]} LIKE '%{filter.Value}%'");

            var result = await conn.QueryAsync<int>(template.RawSql, template.Parameters);
            return result.First();
        }
    }

    public async Task<int> DeletePalletReceiptWithTransactionId(long createTransactionId)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var returnCode = await connection.ExecuteAsync(@"
                DELETE [dbo].[PalletReceipt]
                WHERE CreateTransactionId = @createTransactionId
                ", new
            {
                createTransactionId
            });

            return returnCode;
        }

    }
    public async Task<PalletReceipt> GetPalletReceiptByCreateTransactionId(string createTransactionId)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var palletReceipt = await connection.QueryFirstOrDefaultAsync<PalletReceipt>(@"
                SELECT [Id]
                      ,[CompanyId]
                      ,[IssuerPalletAccountId]
                      ,[TransporterPalletAccountId]
                      ,[PalletAccountId]
                      ,[OrderNumber]
                      ,[PalletTypeId]
                      ,[NumberOfPallets]
                      ,[CarPlateNumber]
                      ,[Comment]
                      ,[CreateTransactionId]
                      ,[CreateStamp]
                      ,[UtilizeTransactionId]
                      ,[UtilizeStamp]
                      ,[Stamp]
                      ,[UserId]
                 FROM [dbo].[PalletReceipt]
                 WHERE [CreateTransactionId] = @createTransactionId",
                new
                {
                    createTransactionId
                });

            if (palletReceipt == null)
                throw new Exception($"No palletReceipt record found for{createTransactionId}");

            return palletReceipt;
        }
    }
}