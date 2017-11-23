using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Learny.Startup))]
namespace Learny
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
