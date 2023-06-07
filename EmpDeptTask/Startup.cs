using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmpDeptTask.Startup))]
namespace EmpDeptTask
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
