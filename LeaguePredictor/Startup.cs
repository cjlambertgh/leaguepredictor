using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LeaguePredictor.Startup))]
namespace LeaguePredictor
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //LeaguePredictor.Models.SeedData.Initialize();
        }


    }
}
