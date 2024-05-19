using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineClothStore.Startup))]
namespace OnlineClothStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
