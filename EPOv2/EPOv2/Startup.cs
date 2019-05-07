using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EPOv2.Startup))]
namespace EPOv2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
