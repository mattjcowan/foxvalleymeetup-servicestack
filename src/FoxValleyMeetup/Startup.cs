using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoxValleyMeetup.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;

namespace FoxValleyMeetup
{
    public class Startup
    {
        static readonly Guid RuntimeId = Guid.NewGuid();

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Licensing.RegisterLicense(@"5932-e1JlZjo1OTMyLE5hbWU6TUpDWm9uZSBJbmMuLFR5cGU6SW5kaWUsSGFzaDpXVFg4dWRIbXpzK0FBRHZCMy9UN1RIUktBR09JSkRqWjhrL0RrM1hwM3p1MFpROUI4clZYc3A4SzBHQnNjRWQzYk1GMjE4clo3UGV2aDNEME1TZzNTM2I0Y0hraEZjMWpjUVFmYWJCNk9ZTWhLOWE4Y2FiL0E0L3BSM3d3OWJEWllqR2J2Wm96NVR0ajJJeXY4eGw1VTRlMExESVBLQnRxUUUyWVdYek5qb0E9LEV4cGlyeToyMDE5LTAzLTE0fQ==");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceStack(new AppHost(env));
            
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("FoxValleyMeetup (runtime: " + RuntimeId + ")");
            });
        }
    }
}
