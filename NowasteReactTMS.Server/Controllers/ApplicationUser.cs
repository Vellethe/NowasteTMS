using Microsoft.AspNetCore.Identity;

namespace NowasteReactTMS.Server.Controllers
{
    public class ApplicationUser : IdentityUser
    {
        public virtual long PalletAccountId { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual Guid? BusinessUnitId { get; set; }
        public virtual string DivisionId { get; set; }
    }
}