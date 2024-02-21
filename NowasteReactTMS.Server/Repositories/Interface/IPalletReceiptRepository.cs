public interface IPalletReceiptRepository
{
    Task UtilizeReceipt(string id, long transactionId);
}