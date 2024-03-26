using NowasteReactTMS.Server.Models;
using NowasteTms.Model;

public interface IPalletInventoryRepository
{

    Task<List<PalletType>> GetInventory();
    Task<IEnumerable<PalletStock>> GetPreviousInventoryPalletStock(long id, DateTime date);
    //Task<int> PostInventory(InventoryDTO dto);
    Task<string> GetPalletTypeDescription(long? palletTypeId);
    Task<IEnumerable<InventoryStockDTO>> GetInventoryPalletStock(long palletAccountId, DateTime date);
}