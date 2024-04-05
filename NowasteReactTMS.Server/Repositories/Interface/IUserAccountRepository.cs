using NowastePalletPortal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NowastePalletPortal.Extensions.Helpers
{
    public interface IUserAccountRepository
    {
        Task UpdateUser(ApplicationUser user);
        Task SetUserPalletAccount(string userId, long palletAccountId);
        Task SetUserRole(string userId, List<string> roleId);
        Task<IEnumerable<Division>> GetDivisions();
    }
}
