using Microsoft.AspNetCore.Mvc.Rendering;
using NowasteTms.Model;
using System.ComponentModel;


namespace NowasteReactTMS.Server.Models
{
    public class InventoryDTO
    {
        public InventoryAccountDTO Inventory { get; set; }

        public PalletType PalletType { get; set; }

        public IEnumerable<PalletType> PalletTypes { get; set; }

        public IEnumerable<SelectListItem> PalletTypesSelectListItem { get; set; }

        public IList<PalletAccountInventoryStockDTO> PalletAccountInventoryStocks { get; set; }

        public PalletAccountInventoryStockDTO PalletAccountInventoryStock { get; set; }

        [DisplayName("Inventory account:")]
        public IEnumerable<SelectListItem> InventoryPalletAccountNames { get; set; }
    }
}
