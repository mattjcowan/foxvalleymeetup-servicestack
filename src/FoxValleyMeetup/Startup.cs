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
            // Licensing.RegisterLicense(@"");
            
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
