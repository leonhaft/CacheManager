using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CacheManagerApp.Startup))]
namespace CacheManagerApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
