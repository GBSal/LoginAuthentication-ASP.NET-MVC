using LoginWithOWIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using LoginWithOWIN.Services;
using Microsoft.AspNet.Identity;
using LoginWithOWIN.Extensions;
using System.Threading.Tasks;
using System.Configuration;

namespace LoginWithOWIN.Controllers
{
    public class AccountController : Controller
    {

        UserService _userServicve;
        private readonly string facebookAppId = string.Empty;
        private readonly string facebookSecret = string.Empty;


        public AccountController()
        {


            _userServicve = new UserService();

           



        }

        #region "Internal Login"

        // GET: Login
        [AllowAnonymous]
        public ActionResult LogOn(string message = "")
        {

            if (Request.IsAuthenticated)
                IdentitySignOut();

            if (!string.IsNullOrEmpty(message))
                ModelState.AddModelError("", message);

            var model = new LoginViewModel();


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOn(LoginViewModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
                return View("LogOn");

            var user = _userServicve.GetUserByEmail(model.Email);

            string pass = App.EncodePassword(model.Password, model.Email);
            if (user != null && (user.Password == pass))
            {
                IdentitySignIn(user, false);



                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToRoute(new { Controller = "Home", Action = "Index" });

            }

            ModelState.AddModelError("", "Invalid email or Password");
            return View("LogOn");



        }

        public ActionResult LogOff()
        {

            IdentitySignOut();
            return RedirectToAction("LogOn");

        }

        public ActionResult Register()
        {

            var model = new RegisterViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {

            if (!ModelState.IsValid)
                return View(model);

            var result = _userServicve.AddUser(model);

            if (result == 2)
                ModelState.AddModelError("", "User alrady exists.");


            return RedirectToRoute(new { Controller = "Account", Action = "LogOn" });


            return View(model);

        }

        #endregion

        #region "Open Login"

        // POST: /Account/ExternalLogin
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "~/";

            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
                return RedirectToAction("LogOn");

            // AUTHENTICATED!
            var providerKey = loginInfo.Login.ProviderKey;


            // Aplication specific code goes here.

            //var user = userBus.ValidateUserWithExternalLogin(providerKey);
            var user = _userServicve.GetUserByKey(providerKey);

            if (user == null)
            {
                return RedirectToAction("LogOn", new
                {
                    message = "Unable to log in with " + loginInfo.Login.LoginProvider +
                              ". " 
                });
            }
            

            // write the authentication cookie
            IdentitySignIn(user, isPersistent: true);

            return Redirect(returnUrl);
        }

        // Initiate oAuth call for external Login
        // GET: /Account/ExternalLinkLogin
        //[AllowAnonymous]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLinkLogin(string provider)
        //{
        //    var id = Request.Form["Id"];

        //    // create an empty AppUser with a new generated id
        //    AppUserState.UserId = id;
        //    AppUserState.Name = "";
        //    IdentitySignin(AppUserState, null);

        //    // Request a redirect to the external login provider to link a login for the current user
        //    return new ChallengeResult(provider, Url.Action("ExternalLinkLoginCallback"), AppUserState.UserId);
        //}

        // oAuth Callback for external login
        // GET: /Manage/LinkLogin
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> ExternalLinkLoginCallback()
        {
            // Handle external Login Callback
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, "1");
            if (loginInfo == null)
            {
               IdentitySignOut(); // to be safe we log out
                return RedirectToAction("Register", new { message = "Unable to authenticate with external login." });
            }

            // Authenticated!
            string providerKey = loginInfo.Login.ProviderKey;
            string providerName = loginInfo.Login.LoginProvider;

            var user = _userServicve.GetUserByKey(loginInfo.Login.ProviderKey);

            // Now load, create or update our custom user

            //// normalize email and username if available
            //if (string.IsNullOrEmpty(AppUserState.Email))
            //    AppUserState.Email = loginInfo.Email;
            //if (string.IsNullOrEmpty(AppUserState.Name))
            //    AppUserState.Name = loginInfo.DefaultUserName;

            //var userBus = new busUser();
            //User user = null;

            //if (!string.IsNullOrEmpty(AppUserState.UserId))
            //    user = userBus.Load(AppUserState.UserId);

            //if (user == null && !string.IsNullOrEmpty(providerKey))
            //    user = userBus.LoadUserByProviderKey(providerKey);

            //if (user == null && !string.IsNullOrEmpty(loginInfo.Email))
            //    user = userBus.LoadUserByEmail(loginInfo.Email);

            //if (user == null)
            //{
            //    user = userBus.NewEntity();
            //    userBus.SetUserForEmailValidation(user);
            //}

            //if (string.IsNullOrEmpty(user.Email))
            //    user.Email = AppUserState.Email;

            //if (string.IsNullOrEmpty(user.Name))
            //    user.Name = AppUserState.Name ?? "Unknown (" + providerName + ")";


            //Update the database with Provider key and login provider.
            //if (loginInfo.Login != null)
            //{
            //    user.OpenIdClaim = loginInfo.Login.ProviderKey;
            //    user.OpenId = loginInfo.Login.LoginProvider;
            //}
            //else
            //{
            //    user.OpenId = null;
            //    user.OpenIdClaim = null;
            //}

            // finally save user inf
           // bool result = userBus.Save(user);

            // update the actual identity cookie
          //  AppUserState.FromUser(user);
            IdentitySignIn(user, true);

            return RedirectToAction("Register");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalUnlinkLogin()
        //{
        //    var userId = AppUserState.UserId;
        //    var user = busUser.Load(userId);
        //    if (user == null)
        //    {
        //        ErrorDisplay.ShowError("Couldn't find associated User: " + busUser.ErrorMessage);
        //        return RedirectToAction("Register", new { id = userId });
        //    }
        //    user.OpenId = string.Empty;
        //    user.OpenIdClaim = string.Empty;

        //    if (busUser.Save())
        //        return RedirectToAction("Register", new { id = userId });

        //    return RedirectToAction("Register", new { message = "Unable to unlink OpenId. " + busUser.ErrorMessage });
        //}



        // **** Helpers 

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "CodePaste_$31!.2*#";

        public class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                    properties.Dictionary[XsrfKey] = UserId;

                var owin = context.HttpContext.GetOwinContext();
                owin.Authentication.Challenge(properties, LoginProvider);
            }
        }




        #endregion


        #region "Helper Methods"
        private void IdentitySignIn(UserModel user, bool isPersistent = false)
        {

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            var roles = _userServicve.GetByUserId(user.Id).ToList();
            if (roles.Any())
            {
                var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r.Name));
                identity.AddClaims(roleClaims);
            }


            AuthenticationManager.SignIn(new AuthenticationProperties()
            {
                AllowRefresh = false,
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(59)
            }, identity);
        }

        private void IdentitySignOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }
        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        #endregion

    }

}
