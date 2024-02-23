using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NowasteReactTMS.Server.Controllers;


namespace NowasteReactTMS.Server
{
    public class NowastePalletPortalContext : IdentityDbContext<ApplicationUser>
    {
        public NowastePalletPortalContext(DbContextOptions<NowastePalletPortalContext> options)
            : base(options)
        {
        }
    }
}
