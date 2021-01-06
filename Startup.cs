using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Assignment1_v9.Startup))]
namespace Assignment1_v9
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
