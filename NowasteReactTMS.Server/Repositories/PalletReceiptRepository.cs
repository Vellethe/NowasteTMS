using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;

public class PalletReceiptRepository : IPalletReceiptRepository
{
    private readonly IConnectionFactory connectionFactory;

    public PalletReceiptRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public Task<int> DeletePalletReceiptWithTransactionId(long createTransactionId)
    {
        throw new NotImplementedException();
    }

    public Task<PalletReceipt> GetPalletReceipt(string searchString)
    {
        throw new NotImplementedException();
    }

    public Task<PalletReceipt> GetPalletReceiptByCreateTransactionId(string createTransactionId)
    {
        throw new NotImplementedException();
    }

    public Task<long> PostPalletReceiptTransaction(PalletReceipt palletReceipt)
    {
        throw new NotImplementedException();
    }

    public Task<List<PalletReceipt>> SearchPalletReceipts(SearchParameters parameters, DateTime fromDate, DateTime toDate)
    {
        throw new NotImplementedException();
    }

    public Task<int> SearchPalletReceiptsCount(SearchParameters parameters, DateTime fromDate, DateTime toDate)
    {
        throw new NotImplementedException();
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
}