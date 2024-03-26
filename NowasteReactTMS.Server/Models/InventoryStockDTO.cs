public class InventoryStockDTO
{

    public int PalletAccountId { get; set; }

    public int PalletTypeId { get; set; }

    public string Name { get; set; }

    public string PalletTypeName { get; set; }

    public int CountedStock { get; set; }

    public DateTime StockCountStamp { get; set; }
}