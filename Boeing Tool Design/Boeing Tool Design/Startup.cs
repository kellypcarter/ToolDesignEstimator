using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Boeing_Tool_Design.Startup))]
namespace Boeing_Tool_Design
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
