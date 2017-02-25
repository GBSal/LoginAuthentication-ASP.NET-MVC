using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using System.Web.Helpers;
using System.Security.Claims;
using LoginWithOWIN.Models;

//[assembly: OwinStartupAttribute(typeof(LoginWithOWIN.Startup))]
namespace LoginWithOWIN
{

    
    public partial class Startup
    {

        public void Configuration(IAppBuilder app)
        {

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/LogOn"),
                ExpireTimeSpan = TimeSpan.FromMinutes(15),
                SlidingExpiration = true,
                LogoutPath = new PathString("/Account/LogOut")
            });

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            


        }

    }

   
}