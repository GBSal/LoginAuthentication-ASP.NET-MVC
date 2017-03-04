using System;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;

using System.Web.Helpers;
using System.Security.Claims;
using System.Configuration;

//[assembly: OwinStartupAttribute(typeof(LoginWithOWIN.Startup))]
namespace LoginWithOWIN
{


    public partial class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            string facebookAppId = ConfigurationManager.AppSettings["FacebookAppId"].ToString();

            string facebookSecret = ConfigurationManager.AppSettings["FacebookSecretKey"].ToString();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/LogOn"),
                ExpireTimeSpan = TimeSpan.FromMinutes(15),
                SlidingExpiration = true,
                LogoutPath = new PathString("/Account/LogOut")

            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);



            //AppId = 183502812140023
            // App Secret = 1f7ff5da730e992f65d56b08fc6d0b2b 

            app.UseFacebookAuthentication(appId: "642708565924424",
               appSecret: "dbfb4df978c0436d43e73740c707e2fe");

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;



        }

    }


}