using Microsoft.AspNetCore.Identity;

namespace NowasteReactTMS.Server.Controllers
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; }
    }
}