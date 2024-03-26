namespace NowasteReactTMS.Server.Models
{
    public class PalletAccountInventoryStockDTO
    {

        public int PalletAccountId { get; set; }


        public int PalletTypeId { get; set; }



        public string PalletTypeName { get; set; }


        public int PreviousNumberOfPallets { get; set; }


        public int ExpectedNumberOfPallets { get; set; }


        public int PalletsDiffPeriod { get; set; }


        public DateTime StockDate { get; set; }
    }
}