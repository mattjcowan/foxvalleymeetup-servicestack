using System;
using Microsoft.AspNetCore.Hosting;
using ServiceStack;

namespace FoxValleyMeetup.Web.Configurations.c01_PingService
{
    public class PingService: Service
    {
        public IHostingEnvironment Env { get; set; } // aspnet core DI works out-of-the-box

        public object Any(PingRequest request)
        {
            return new PingResponse {
                ServerTime = DateTime.UtcNow,
                ApplicationName = Env.ApplicationName,
                EnvironmentName = Env.EnvironmentName };
        }
    }

    [Route("/ping", "GET,PUT,POST")] 
    public class PingRequest: IReturn<PingResponse>
    {
    }

    public class PingResponse { 
        public DateTime ServerTime { get; set; }
        public string ApplicationName { get; set; }
        public string EnvironmentName { get; set; }
    }
}