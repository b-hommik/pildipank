using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Pildipank_ver2.Startup))]
namespace Pildipank_ver2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
