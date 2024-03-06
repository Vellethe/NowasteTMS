using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NowasteReactTMS.Server.Controllers;
using NowasteTms.Model;


namespace NowasteReactTMS.Server
{
    public class NowastePalletPortalContext : IdentityDbContext<ApplicationUser>
    {
        public NowastePalletPortalContext(DbContextOptions<NowastePalletPortalContext> options)
            : base(options)
        {
        }
       //public DbSet<Order> Orders { get; set; }
    }
}
