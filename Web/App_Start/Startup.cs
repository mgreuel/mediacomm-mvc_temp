using MediaCommMvc.Web.App_Start;

using Microsoft.Owin;

using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace MediaCommMvc.Web.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
        }
    }
}
