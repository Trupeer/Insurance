using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HONGLI.Web.Startup))]
namespace HONGLI.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
