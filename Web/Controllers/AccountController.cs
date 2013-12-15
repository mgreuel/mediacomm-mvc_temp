using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using MediaCommMvc.Web.Infrastructure;
using MediaCommMvc.Web.ViewModels.Account;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;

namespace MediaCommMvc.Web.Controllers
{
    public partial class AccountController : Controller
    {
        private const string XsrfKey = "XsrfId";

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        private AccountController(UserManager<ApplicationUser> userManager)
        {
            this.UserManager = userManager;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess, 
            SetPasswordSuccess, 
            RemoveLoginSuccess, 
            Error
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return this.HttpContext.GetOwinContext().Authentication;
            }
        }

        private UserManager<ApplicationUser> UserManager { get; set; }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            IdentityResult result = await this.UserManager.RemoveLoginAsync(this.User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));

            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }

            return this.RedirectToAction("Manage", new { Message = message });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, this.Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public virtual async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return this.RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await this.UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await this.SignInAsync(user, false);
                return this.RedirectToLocal(returnUrl);
            }

            // If the user does not have an account, then prompt the user to create an account
            this.ViewBag.ReturnUrl = returnUrl;
            this.ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            return this.View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Manage");
            }

            if (this.ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await this.AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return this.View("ExternalLoginFailure");
                }

                var user = new ApplicationUser { UserName = model.UserName };
                var result = await this.UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await this.UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await this.SignInAsync(user, false);
                        return this.RedirectToLocal(returnUrl);
                    }
                }

                this.AddErrors(result);
            }

            this.ViewBag.ReturnUrl = returnUrl;
            return this.View(model);
        }

        [AllowAnonymous]
        public virtual ActionResult ExternalLoginFailure()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, this.Url.Action("LinkLoginCallback", "Account"), this.User.Identity.GetUserId());
        }

        public virtual async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, this.User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return this.RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }

            var result = await this.UserManager.AddLoginAsync(this.User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return this.RedirectToAction("Manage");
            }

            return this.RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult LogOff()
        {
            this.AuthenticationManager.SignOut();
            return this.RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public virtual ActionResult Login(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await this.SignInAsync(user, model.RememberMe);
                    return this.RedirectToLocal(returnUrl);
                }

                this.ModelState.AddModelError(string.Empty, Resources.Login.UsernameOrPasswordWrongErrorMessage);
            }

            return this.View(model);
        }

        public virtual ActionResult Manage(ManageMessageId? message)
        {
            this.ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                    : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                          : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                                : message == ManageMessageId.Error ? "An error has occurred."
                                      : string.Empty;
            this.ViewBag.HasLocalPassword = this.HasPassword();
            this.ViewBag.ReturnUrl = this.Url.Action("Manage");
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = this.HasPassword();
            this.ViewBag.HasLocalPassword = hasPassword;
            this.ViewBag.ReturnUrl = this.Url.Action("Manage");
            if (hasPassword)
            {
                if (this.ModelState.IsValid)
                {
                    IdentityResult result = await this.UserManager.ChangePasswordAsync(this.User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }

                    this.AddErrors(result);
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = this.ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (this.ModelState.IsValid)
                {
                    IdentityResult result = await this.UserManager.AddPasswordAsync(this.User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }

                    this.AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        [AllowAnonymous]
        public virtual ActionResult Register()
        {
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName };
                var result = await this.UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await this.SignInAsync(user, false);
                    return this.RedirectToAction("Index", "Home");
                }

                this.AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        [ChildActionOnly]
        public virtual ActionResult RemoveAccountList()
        {
            var linkedAccounts = this.UserManager.GetLogins(this.User.Identity.GetUserId());
            this.ViewBag.ShowRemoveButton = this.HasPassword() || linkedAccounts.Count > 1;
            return this.PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.UserManager != null)
            {
                this.UserManager.Dispose();
                this.UserManager = null;
            }

            base.Dispose(disposing);
        }

        // Used for XSRF protection when adding external logins
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error);
            }
        }

        private bool HasPassword()
        {
            var user = this.UserManager.FindById(this.User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }

            return false;
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.RedirectToAction("Index", "Home");
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await this.UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            this.AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                this.LoginProvider = provider;
                this.RedirectUri = redirectUri;
                this.UserId = userId;
            }

            private string LoginProvider { get; set; }

            private string RedirectUri { get; set; }

            private string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = this.RedirectUri };
                if (this.UserId != null)
                {
                    properties.Dictionary[XsrfKey] = this.UserId;
                }

                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, this.LoginProvider);
            }
        }
    }
}