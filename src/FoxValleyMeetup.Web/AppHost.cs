using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FoxValleyMeetup.Web.Configurations.c01_PingService;
using FoxValleyMeetup.Web.Configurations.c02_YamlFormat;
using FoxValleyMeetup.Web.Configurations.c03_SecurePingService;
using FoxValleyMeetup.Web.Configurations.c05_Bookmarks;
using Funq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ServiceStack;
using ServiceStack.Api.OpenApi;
using ServiceStack.Api.OpenApi.Specification;
using ServiceStack.Auth;
using ServiceStack.Caching;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Redis;
using ServiceStack.Text;

namespace FoxValleyMeetup.Web
{
    public class AppHost : AppHostBase
    {
        private IHostingEnvironment _env;
        private string _dataDir;

        public AppHost(IHostingEnvironment env): base(env.ApplicationName, typeof(AppHost).Assembly)
        {
            this._env = env;
            this._dataDir = Path.Combine(env.ContentRootPath, "App_Data");
            Directory.CreateDirectory(this._dataDir);
        }

        // public override async Task ProcessRequest(HttpContext context, Func<Task> next)
        // {
        //     await context.Response.WriteAsync("Hello World!");
        // }

        public override void Configure(Container container)
        {
        }
        public void ConfigureFake(Container container)
        {
            // JsConfig.DateHandler = DateHandler.ISO8601;

            // var hostConfig = new HostConfig
            // {
            //     DebugMode = false, // enables some debugging features
            //     EnableFeatures = Feature.All.Remove(Feature.Csv), // removes the CSV format
            //     HandlerFactoryPath = "/api" // moves the ServiceStack api surface under a sub-route
            // };
            // hostConfig.GlobalResponseHeaders["X-Powered-By"] = "FoxValley Power";
            // SetConfig(hostConfig);
            
            // Routes.Add(typeof(PingRequest), "/ping", "GET"); 
            
            // container.Register<IDbConnectionFactory>(c => new OrmLiteConnectionFactory(Path.Combine(_dataDir, "db.sqlite"), SqliteDialect.Provider));
            // var dbFactory = TryResolve<IDbConnectionFactory>();

            // Plugins.Add(new PostmanFeature());

            // Plugins.Add(new CorsFeature());
            
            // Plugins.Add(new YamlFormat(true));

            // (new [] { typeof(Authenticate), typeof(Register), typeof(GetApiKeys), typeof(RegenerateApiKeys) })
            //     .Each(t => t.AddAttributes(new TagAttribute("auth")));
            // (new [] { typeof(AssignRoles), typeof(UnAssignRoles) })
            //     .Each(t => t.AddAttributes(new TagAttribute("roles")));
            // (new [] { typeof(PingRequest), typeof(SecurePingRequest), typeof(AdminPingRequest) })
            //     .Each(t => t.AddAttributes(new TagAttribute("ping")));

            // var authProviders = new IAuthProvider[] {
            //     new CredentialsAuthProvider(),
            //     new BasicAuthProvider(),
            //     new ApiKeyAuthProvider(),
            //     new JwtAuthProvider() {
            //         RequireSecureConnection = false,
            //         HashAlgorithm = "RS256",
            //         PrivateKeyXml = GetPrivateKeyXml()
            //     },
            //     new DigestAuthProvider(this.AppSettings),
            //     new FacebookAuthProvider(this.AppSettings),
            //     new TwitterAuthProvider(this.AppSettings),
            //     new GithubAuthProvider(this.AppSettings)
            // };

            // Plugins.Add(new AuthFeature(() => new AuthUserSession(), authProviders) {
            //     SaveUserNamesInLowerCase = true,
            // });

            // Plugins.Add(new RegistrationFeature() {
            //     AtRestPath = "/register"
            // });

            // // CACHING
            // // container.Register<IRedisClientsManager>(c => new RedisManagerPool("localhost:6379"));            
            // if (container.TryResolve<IRedisClientsManager>() != null) // caching redis
            //     container.Register(c => c.Resolve<IRedisClientsManager>().GetCacheClient());
            // else if (dbFactory != null) // caching database
            //     container.Register<ICacheClient>(new OrmLiteCacheClient() { DbFactory = dbFactory });
            // else // caching in-memory
            //     container.Register<ICacheClient>(new MemoryCacheClient());
            // (TryResolve<ICacheClient>() as IRequiresSchema)?.InitSchema();

            // // AUTHENTICATION
            // IAuthRepository authRepository;
            // if (dbFactory != null)
            //     authRepository = new OrmLiteAuthRepository(dbFactory);
            // else 
            //     authRepository = new InMemoryAuthRepository();                
            // Register<IAuthRepository>(authRepository);
            // (TryResolve<IAuthRepository>() as IRequiresSchema)?.InitSchema();

            // var authRepo = TryResolve<IAuthRepository>();
            // if (authRepo != null)
            // {
            //     var adminUser = authRepo.GetUserAuthByUserName("admin");
            //     if (adminUser == null)
            //     {
            //         adminUser = authRepo.CreateUserAuth(
            //             new UserAuth { UserName = "admin", DisplayName = "Administrator", Email = "admin@company.com" }, "admin");
            //     }
            //     var roles = authRepo.GetRoles(adminUser);
            //     if (!roles.Contains("Admin"))
            //     {
            //         authRepo.AssignRoles(adminUser, new [] { "Admin" });
            //     }

            //     var guestUser = authRepo.GetUserAuthByUserName("guest");
            //     if (guestUser == null)
            //     {
            //         guestUser = authRepo.CreateUserAuth(
            //             new UserAuth { UserName = "guest", DisplayName = "Guest", Email = "guest@company.com" }, "guest");
            //     }
            // }

            // if (dbFactory != null)
            // {
            //     using(var db = dbFactory.OpenDbConnection())
            //     {
            //         db.CreateTableIfNotExists(typeof(Bookmark));

            //         var bkFile = Path.Combine(_dataDir, "bookmarks.csv");
            //         if (File.Exists(bkFile))
            //         {
            //             using (var stream = File.OpenRead(bkFile))
            //             {
            //                 var bookmarks = BookmarkUtils.ToBookmarks(stream);
            //                 foreach(var bookmark in bookmarks)
            //                 {
            //                     bookmark.CreatedBy = "admin";
            //                     bookmark.CreatedById = 1;
            //                     bookmark.CreatedOn = DateTime.UtcNow;
            //                     bookmark.ModifiedBy = "admin";
            //                     bookmark.ModifiedById = 1;
            //                     bookmark.ModifiedOn = DateTime.UtcNow;
            //                     try
            //                     {
            //                         db.Insert(bookmark);
            //                     }
            //                     catch {}
            //                 }
            //             }
            //             File.Move(bkFile, bkFile + ".old");
            //         }
            //     }
            //     var autoQuery = new AutoQueryFeature { MaxLimit = 100 };
            //     Plugins.Add(autoQuery);
            // }

            // Plugins.Add(new OpenApiFeature() {
            //     UseCamelCaseSchemaPropertyNames = true
            // });
        }

        private string GetPrivateKeyXml()
        {
            // TODO: you can do better than this obviously :-) 

            var privateKeyFile = "privateKey.xml".MapHostAbsolutePath();
            if (!File.Exists(privateKeyFile))
                File.WriteAllText(privateKeyFile, RsaUtils.CreatePrivateKeyParams(RsaKeyLengths.Bit2048).ToPrivateKeyXml());
            return  File.ReadAllText(privateKeyFile);
        }
    }
}
