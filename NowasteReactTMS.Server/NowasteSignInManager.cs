using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NowasteReactTMS.Server.Controllers;

namespace NowastePalletPortal.Extensions.Helpers
{
    public class NowasteSignInManager<TUser> : SignInManager<ApplicationUser> where TUser : class
    {

        public NowasteSignInManager(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            IAuthenticationSchemeProvider schemeProvider,
            IUserConfirmation<ApplicationUser> confirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemeProvider, confirmation)
        {
        }

        public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe, bool lockoutOnFailure)
        {
            var user = UserManager.FindByEmailAsync(userName).Result;

            if (user != null && !user.IsActive)
            {
                return Task.FromResult(SignInResult.Failed);
            }

            return base.PasswordSignInAsync(userName, password, rememberMe, lockoutOnFailure);
        }
    }
}
