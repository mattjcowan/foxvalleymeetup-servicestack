// using System;
// using Microsoft.AspNetCore.Hosting;
// using ServiceStack;
// using ServiceStack.Auth;

// namespace FoxValleyMeetup.Web.Configurations.c03_SecurePingService
// {
//     [Authenticate]
//     public class SecurePingService: Service
//     {
//         public IHostingEnvironment Env { get; set; } // aspnet core DI works out-of-the-box

//         public object Any(SecurePingRequest request)
//         {
//             return new SecurePingResponse {
//                 ServerTime = DateTime.UtcNow,
//                 ApplicationName = Env.ApplicationName,
//                 EnvironmentName = Env.EnvironmentName,
//                 UserSession = base.GetSession(true)  };
//         }

//         public object Any(AdminPingRequest request)
//         {
//             return new SecurePingResponse {
//                 ServerTime = DateTime.UtcNow,
//                 ApplicationName = Env.ApplicationName,
//                 EnvironmentName = Env.EnvironmentName,
//                 UserSession = base.GetSession(true)  };
//         }
//     }

//     [Route("/sping", "GET")]
//     public class SecurePingRequest: IReturn<SecurePingResponse>
//     {
//     }

//     [RequiredRole("Admin")]
//     [Route("/aping", "GET")]
//     public class AdminPingRequest: IReturn<SecurePingResponse>
//     {
//     }

//     public class SecurePingResponse {
//         public DateTime ServerTime { get; set; }
//         public string ApplicationName { get; set; }
//         public string EnvironmentName { get; set; }
//         public IAuthSession UserSession { get; set; }
//     }
// }