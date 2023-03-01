using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PrintHouse.Startup))]
namespace PrintHouse
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
