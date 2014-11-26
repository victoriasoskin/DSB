using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BootStrap.Startup))]
namespace BootStrap
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
