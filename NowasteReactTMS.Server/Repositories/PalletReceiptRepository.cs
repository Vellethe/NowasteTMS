using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;

public class PalletReceiptRepository: IPalletReceiptRepository
{
    private readonly IConnectionFactory connectionFactory;

    public PalletReceiptRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
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