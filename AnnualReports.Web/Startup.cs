using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AnnualReports.Web.Startup))]
namespace AnnualReports.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
