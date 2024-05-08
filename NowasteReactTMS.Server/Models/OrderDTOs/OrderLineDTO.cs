using NowasteTms.Model;
using System.ComponentModel.DataAnnotations;

namespace NowasteReactTMS.Server.Models.OrderDTOs
{
    public class OrderLineDTO
    {
        [Key]
        public Guid ItemPK { get; set; }

        [Range(1, 12)] public int PalletTypeId { get; set; }

        [Range(1, int.MaxValue)] public int ItemQty { get; set; }

        [Range(1, 100)] public int PalletQty { get; set; }
    }
}
