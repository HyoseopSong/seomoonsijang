using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(seomoonsijang.Startup))]
namespace seomoonsijang
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
