using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(mvc_project.Startup))]
namespace mvc_project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
