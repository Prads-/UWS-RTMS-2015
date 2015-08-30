using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Professional_Experience.Startup))]
namespace Professional_Experience
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
