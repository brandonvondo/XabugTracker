using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(XabugTracker.Startup))]
namespace XabugTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
