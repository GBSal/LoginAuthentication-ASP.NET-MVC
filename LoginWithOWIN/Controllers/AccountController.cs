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

namespace LoginWithOWIN.Controllers
{
    public class AccountController : Controller
    {

        UserService _userServicve;


       
        public AccountController()
        {


            _userServicve = new UserService();

        }

        // GET: Login
        [AllowAnonymous]
        public ActionResult LogOn(string message  = "")
        {

             if(Request.IsAuthenticated)
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

             if (result==2)
                ModelState.AddModelError("", "User alrady exists.");


            return RedirectToRoute(new { Controller = "Account", Action = "LogOn" });


            return View(model);

        }


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


    }

}
