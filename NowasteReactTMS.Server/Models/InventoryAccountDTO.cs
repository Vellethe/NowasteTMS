using System.ComponentModel.DataAnnotations;

namespace NowasteReactTMS.Server.Models
{
    public class InventoryAccountDTO
    {


        [Key]
        public int Id { get; set; }


        public int PalletAccountId { get; set; }


        public int InventoryTypeId { get; set; }


        public string Warehouse { get; set; }


        public string Comment { get; set; }


        public int PalletTypeId { get; set; }


        public int Quantity { get; set; }

    }
    }
