

[assembly: HostingStartup(typeof(NowastePalletPortal.Areas.Identity.IdentityHostingStartup))]
namespace NowastePalletPortal.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}