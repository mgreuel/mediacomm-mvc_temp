using System;
using System.IO;

using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;

using Newtonsoft.Json;

using Owin;

namespace MediaCommMvc.Web.App_Start
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            AuthSettings authSettings = this.GetAuthSettings();

            if (authSettings.EnableGoogleAuthentication)
            {
                app.UseGoogleAuthentication();
            }

            if (authSettings.EnableMicrosoftAuthentication)
            {
                app.UseMicrosoftAccountAuthentication(authSettings.MicrosoftClientId, authSettings.MicrosoftClientSecret);
            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/Account/Login")
            });

            // Uncomment the following lines to enable logging in with third party login providers

            // app.UseTwitterAuthentication(
            // consumerKey: "",
            // consumerSecret: "");

            // app.UseFacebookAuthentication(
            // appId: "",
            // appSecret: "");
        }

        private AuthSettings GetAuthSettings()
        {
            string authSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_PrivateSettings\\auth.settings");

            if (!File.Exists(authSettingsPath))
            {
                return new AuthSettings();
            }

            var authSettings = JsonConvert.DeserializeObject<AuthSettings>(File.ReadAllText(authSettingsPath));
            return authSettings;
        }

        private class AuthSettings
        {
            public bool EnableGoogleAuthentication { get; set; }

            public bool EnableMicrosoftAuthentication { get; set; }

            public string MicrosoftClientId { get; set; }

            public string MicrosoftClientSecret { get; set; }
        }
    }
}