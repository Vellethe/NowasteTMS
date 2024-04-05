using Microsoft.AspNetCore.Identity;
using System;

namespace NowastePalletPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual long PalletAccountId { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual Guid? BusinessUnitId { get; set; }
        public virtual string DivisionId { get; set; }
    }
}
