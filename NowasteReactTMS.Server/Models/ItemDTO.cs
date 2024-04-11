using System.ComponentModel.DataAnnotations;

namespace NowasteReactTMS.Server.Models
{
    public class ItemDTO
    {
        [Key]
        public Guid ItemPK { get; set; }

        public string ItemID { get; set; }

        public string Name { get; set; }

        public decimal Weight { get; set; }

        public int Volume { get; set; }

        public int TransportTemp { get; set; }

        public int StorageTemp { get; set; }

        public string Company { get; set; }

        public bool IsActive { get; set; } = true;


        public bool EditName { get; set; }
    }
}
