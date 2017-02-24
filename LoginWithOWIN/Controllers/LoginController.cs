using LoginWithOWIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace LoginWithOWIN.Controllers
{
    public class LoginController : Controller
    {

        UserSerice _userServicve;
        public LoginController()
        {

        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {




                return RedirectToRoute(new { Controller = "Home", Action = "Index" });
            }


            return View("Index");

        }


        private void AuthenticateUser(UserModel user , bool isPersistence =false)
        {

            var claims = new[] {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, DefaultApplicationTypes.ApplicationCookie);

            var roles = _roleService.GetByUserId(user.Id).ToList();
            if (roles.Any())
            {
                var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r.Name));
                identity.AddClaims(roleClaims);
            }

            var context = Request.GetOwinContext();
            var authManager = context.Authentication;

            authManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistence }, identity);
        }

        private class UserSerice
        {
        }
    }

    }
}