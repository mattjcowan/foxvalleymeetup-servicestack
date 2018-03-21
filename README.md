# Getting started with ServiceStack

A presentation for the [Fox Valley .NET Web Development Meetup](https://www.meetup.com/Fox-Valley-NET-Web-Development-Meetup/).

Also, check out the [accompanying slides](http://slides.com/mattjcowan/foxvalleymeetup-servicestack/).

# Create empty aspnet repo

Create the 'app' directory:

```shell
mkdir app
cd app
export appdir=`pwd`
dotnet new web -n FoxValleyMeetup -o FoxValleyMeetup
cd FoxValleyMeetup
```

Add dotnet-watch, by opening the .csproj

```xml
<ItemGroup>
    <DotNetCliToolReference Include="Microsoft.DotNet.Watcher.Tools" Version="2.0.0" />
</ItemGroup>
```

Fire it up, and check for "Hello World!" at http://localhost:5000

```shell
cd FoxValleyMeetup
export ASPNETCORE_ENVIRONMENT=Development # use `set` if on windows
dotnet watch run
```

### Add ServiceStack

Create a new class library

```shell
cd $appdir
dotnet new classlib -n FoxValleyMeetup.Web
cd FoxValleyMeetup.Web
rm Class1.cs
```

Add ServiceStack packages (only the ServiceStack package is needed, but we're going to look at lots of features
so we'll bring in all the core packages for now to save us some time)

```shell
dotnet add package ServiceStack
dotnet add package ServiceStack.Server
dotnet add package ServiceStack.OrmLite
dotnet add package ServiceStack.OrmLite.SqLite
dotnet add package ServiceStack.OrmLite.SqlServer
dotnet add package ServiceStack.OrmLite.MySql
dotnet add package ServiceStack.OrmLite.Postgresql
dotnet add package ServiceStack.Api.OpenApi
dotnet restore
```

Reference the classlib in the FoxValleyMeetup project file

```shell
dotnet add $appdir/FoxValleyMeetup/FoxValleyMeetup.csproj reference FoxValleyMeetup.Web.csproj
```

Create a ServiceStack AppHost class. The AppHost is:
- A singleton
- The central nervous system with a quantity of overrides to extend the framework in every possible way

```csharp
public class AppHost : AppHostBase
{
    public AppHost(): base("FoxValleyMeetup", typeof(AppHost).Assembly)
    {
    }

    public override void Configure(Container container)
    {
    }
}
```

Hook it up in the FoxValleyMeetup Startup.cs class

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseServiceStack(new AppHost());
}
```

Move the "Hello World!" message to the ServiceStack AppHost

```csharp
public override async Task ProcessRequest(HttpContext context, Func<Task> next)
{
    await context.Response.WriteAsync("Hello World!");
}
```

Refresh the browser at http://localhost:5000

## Out-of-the-box features

Architecture: http://docs.servicestack.net/architecture-overview

### Order of operations

http://docs.servicestack.net/order-of-operations

- Incoming request
    - Route/params/content-type/verb recognition
    - RawHttpHandlers (IHttpHandler)
    - CatchAllHandler
    - PreRequestFilters (before request body is deserialized into request DTO)
    - Request binding to request DTOs
    - Request converters (substitute the request DTO for another)
    - Request filters (good place to do validation/authentication or populate request items bag)
- Service execution (w/ OnBeforeExecute, OnAfterExecute, HandleException)
- Outgoing response:
    - Response converters (substitute the response DTO for another)
    - Response filters (modify outgoing headers)
    - OnEndRequest, OnEndRequestCallbacks

### Services

*Comment out the ProcessRequest override in the AppHost file, which would short-circuit the ServiceStack pipeline.*

Let's create a basic service. Create a `PingService` class

```csharp
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
```

Notice the [Route] attribute, and the Any method. Navigate to http://localhost:5000/ping.

There are several ways of mapping request paths to their respective DTOs. The `RouteAttribute` attribute is probably the most
common and easiest to wire up. But sometimes you may want to configure the path with a more fluent interface. Use the Route table instead.

- In the apphost Configure method, use the Route table
```csharp
this.Routes.Add(typeof(PingRequest), "/ping", "GET,PUT,POST");
```

### Metadata

Go to http://localhost:5000/metadata

Integrating typed clients, without sharing the actual assemblies, is easy:
- http://localhost:5000/types.json

### Formats

Out of the box, you get a bunch of format support:
- xml
- json
- csv

Let's add another:
- yaml

Make sure YamlDotNet is added to the FoxValleyMeetup.Web project

```shell
dotnet add package YamlDotNet
```

Create a `YamlFormat` class

```csharp
public class YamlFormat : IPlugin
{
    public YamlFormat(bool usePlainText = false, bool useRequestTypeAsName = false)
    {
        UsePlainText = usePlainText;
        UseRequestTypeAsName = useRequestTypeAsName;
    }

    public bool UsePlainText { get; }
    public bool UseRequestTypeAsName { get; }

    public void Register(IAppHost appHost)
    {
        //Register the 'application/yaml' content-type and serializers (format is inferred from the last part of the content-type)
        appHost.ContentTypes.Register(MimeTypes.Yaml, YamlSerializer.SerializeToStream, YamlSerializer.DeserializeFromStream);

        //Add a response filter to add a 'Content-Disposition' header so browsers treat it natively as a .yaml file
        appHost.GlobalResponseFilters.Add((req, res, dto) =>
        {
            if (req.ResponseContentType == MimeTypes.Yaml)
            {
                if (UsePlainText)
                {
                    res.RemoveHeader("Content-Type");
                    res.AddHeader("Content-Type", MimeTypes.PlainText);
                }
                else
                {
                    if (UseRequestTypeAsName)
                    {
                        res.AddHeader(HttpHeaders.ContentDisposition, $"attachment;filename={req.OperationName}.yaml");
                    }
                }
            }
        });
    }
}

public class YamlSerializer
{
    public static object DeserializeFromStream(Type type, Stream fromStream)
    {
        using (var reader = new StreamReader(fromStream))
        {
            return YamlSerializer.SerializeFromReader(type, reader);
        }
    }

    private static object SerializeFromReader(Type type, StreamReader reader)
    {
        var serializer = new YamlDotNet.Serialization.Deserializer();
        return serializer.Deserialize(reader, type);
    }

    public static void SerializeToStream(IRequest requestContext, object request, Stream stream)
    {
        YamlSerializer.SerializeToStream(request, stream);
    }

    public static void SerializeToStream(object request, Stream stream)
    {
        using (var writer = new StreamWriter(stream))
        {
            YamlSerializer.SerializeToWriter(request, writer);
        }
    }

    private static void SerializeToWriter(object request, StreamWriter writer)
    {
        var serializer = new YamlDotNet.Serialization.Serializer();
        serializer.Serialize(writer, request);
    }
}
```

Notice that the class extends the IPlugin interface. This is the most common way of extending ServiceStack.

Also notice how we used a GlobalResponseFilter to manipulate the headers in the request.

Now add the YamlFormat to the application, by adding it in the AppHost class.

```csharp
// add yaml format
Plugins.Add(new YamlFormat(true));
```

Go to http://localhost:5000/metadata

- Notice the new YAML format

### Configuration

ServiceStack lets us configure several dozen core settings of the framework.

Inside the `AppHost.Configure` method, add the following as an example:

```csharp
SetConfig(new HostConfig
{
    DebugMode = true, // enables some debugging features
    EnableFeatures = Feature.All.Remove(Feature.Csv), // removes the CSV format
    HandlerFactoryPath = "/api" // moves the ServiceStack api surface under a sub-route
});
```

Now, navigate to http://localhost:5000/api/metadata and /api/ping instead of the previous routes.

Let's comment out the factory path before continuing to the next step.

### OpenApi/Swagger

Let's add a UI to start documenting our services, and also give us an easy way of interacting with the
services.

```shell
dotnet add package ServiceStack.Api.OpenApi
```

In the `AppHost` class, add the OpenApiFeature plugin.

```csharp
Plugins.Add(new OpenApiFeature() {
    UseCamelCaseSchemaPropertyNames = true
});
```

Now, browse to http://localhost:5000/swagger-ui/

### Authentication

http://docs.servicestack.net/authentication-and-authorization#auth-providers

- Wire up authentication inside the AppHost as a Plugin

```csharp
Plugins.Add(new AuthFeature(() => new AuthUserSession(), new IAuthProvider[] {
    new CredentialsAuthProvider(),
    new BasicAuthProvider(),
    new ApiKeyAuthProvider()
}) {
    IncludeAssignRoleServices = true,
    IncludeRegistrationService = true,
    SaveUserNamesInLowerCase = true
});
var authRepository = new InMemoryAuthRepository();
Register<IAuthRepository>(authRepository);
```

- Add authenticated user info to PingService

```csharp
public IAuthSession UserSession { get; set; } // use base.GetSession(true) to populate it
```

- Try registering a user, authenticating, and see validation in action in the OpenApi interface

- Organize the documentation by adding OpenApi tags using runtime attributes

```csharp
(new [] { typeof(Authenticate), typeof(Register), typeof(GetApiKeys), typeof(RegenerateApiKeys) }).Each(t => t.AddAttributes(new TagAttribute("auth")));
(new [] { typeof(AssignRoles), typeof(UnAssignRoles) }).Each(t => t.AddAttributes(new TagAttribute("roles")));
```

- create a users api

### Database

Add OrmLite to the project and SqLite (could also use SqlServer, MySql, Oracle, Postgres).
OrmLite is a database agnostic ORM

```shell
dotnet add package ServiceStack.OrmLite
dotnet add package ServiceStack.OrmLite.SqLite
dotnet add package ServiceStack.Server
dotnet restore
```

Now create a database registration in AppHost

```csharp
container.Register<IDbConnectionFactory>(c => { return new OrmLiteConnectionFactory("db.sqlite", SqliteDialect.Provider); });
var dbFactory = TryResolve<IDbConnectionFactory>();
```

Also, switch the auth repository to use the database

```csharp
var authRepository = new OrmLiteAuthRepository(dbFactory);
authRepository.InitSchema();
authRepository.InitApiKeySchema();
```

### Caching

```csharp
RegisterAs<OrmLiteCacheClient, ICacheClient>();
TryResolve<ICacheClient>().InitSchema();
```

### Securing an API

Now create an api that only lets authenticated users query the users in the system

```csharp
[Authenticate]
public class UsersService : Service
{
    public async Task<object> Get(UsersRequest request)
    {
        return (await Db.SelectAsync<UserAuth>(Db.From<UserAuth>().OrderBy(u => u.UserName).ThenBy(u => u.DisplayName)))
            .Map(u => u.To<UserInfo>() new UserInfo().PopulateWithNonDefaultValues(u));
    }
}

public class UserInfo
{
    public virtual int Id { get; set; }
    public virtual string UserName { get; set; }
    public virtual string Email { get; set; }
    public virtual string PhoneNumber { get; set; }
    public virtual string DisplayName { get; set; }
}

[Route("/users", "GET")]
public class UsersRequest: IReturn<List<UserInfo>>
{
}
```

### AutoQuery

```shell
Plugins.Add(new AutoQueryFeature { MaxLimit = 100 });
```
