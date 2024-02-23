using NowasteTms.Model;

public interface IPalletReceiptRepository
{
    //Task<List<PalletReceiptItemViewModel>> GetPalletReceipts(); //Viewmodel
    Task<PalletReceipt> GetPalletReceipt(string searchString);
    //Task<PalletReceiptItemViewModel> GetUtilizedPalletReceipt(string palletReceiptId); //Viewmodel
    Task UtilizeReceipt(string id, long transactionId);
    Task<long> PostPalletReceiptTransaction(PalletReceipt palletReceipt);
    Task<List<PalletReceipt>> SearchPalletReceipts(SearchParameters parameters, DateTime fromDate, DateTime toDate);
    Task<int> SearchPalletReceiptsCount(SearchParameters parameters, DateTime fromDate, DateTime toDate);
    Task<int> DeletePalletReceiptWithTransactionId(long createTransactionId);
    Task<PalletReceipt> GetPalletReceiptByCreateTransactionId(string createTransactionId);
}