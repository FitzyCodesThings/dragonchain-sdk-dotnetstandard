# Dragonchain .Net SDK

[![NuGet](https://img.shields.io/badge/nuget-v1.0.0.6--alpha-blue.svg)](https://www.nuget.org/packages/dragonchain-sdk-dotnet/)

Talk to your dragonchain.

### Installation
First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Dragonchain .Net SDK](https://www.nuget.org/packages/dragonchain-sdk-dotnet/) from the package manager console:

Pre -release
```
PM> Install-Package dragonchain-sdk-dotnet -Version 1.0.0-alpha
```

### Examples

### Import
```csharp
using dragonchain_sdk;
using dragonchain_sdk.Framework.Web;
```

#### GetBlock

```csharp
var myDcId = "3f2fef78-0000-0000-0000-9f2971607130";
var client = new DragonchainClient(myDcId);

const call = await client.getBlock('block-id-here');

try 
{
    var block = await client.GetBlock("block-id-here");
    var block = call.Response;
    Console.WriteLine("Successful call!");
    Console.WriteLine($"Block: {block.Header.BlockId}");
}
catch(DragonchainApiException exception)
{
    Console.WriteLine("Something went wrong!");
    Console.WriteLine($"HTTP status code from chain: {exception.Status}");
    Console.WriteLine($"Error response from chain: {exception.Message}");
}
```

#### QueryTransactions

```csharp
var searchResult = await client.QueryTransactions("tag=MyAwesomeTransactionTag");
var totalTransactionsCount = searchResult.Response.Total;
var transactions = searchResult.Response.Results;
```

#### Create Transaction Type

```csharp
var transactionTypeSimpleResponse = await _dragonchainLevel1Client.CreateTransactionType("NewType", 
    new List<CustomIndexStructure>
    {
        new CustomIndexStructure{ Key ="SomeKey", Path="SomePath" }
    }
});
```

#### Create a Transaction

```csharp
var transactionCreateResponse = await _dragonchainLevel1Client.CreateTransaction("apple", new {}, "pottery", "http://mycallbackUrl");
```
#### Inject into Asp.net Core Web App

##### Program.cs
Configure logging and configuration choices.

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("config.json", optional: true, reloadOnChange: true);
            })
            .UseStartup<Startup>()
            .Build();
}
```

##### Startup.cs

Add the dragonchain client service using the ConfigureServices method

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IDragonchainClient, DragonchainClient>();
    services.AddMvc();            
}
```

##### Controller.cs

Consume the service in controllers or middleware
```csharp
public class HomeController : Controller
{
    private IDragonchainClient _client;

    public HomeController(IDragonchainClient client)
    {
        _client = client;
    }

    public IActionResult Index()
    {
        var transactionTypes = _client.ListTransactionTypes();
        return View(transactionTypes);
    }
```

## Configuration

In order to use this SDK, you need to have an Auth Key as well as an Auth Key ID for a given dragonchain.
This can be loaded into the sdk in various ways using an IConfiguration provider:

1. The environment variables `AUTH_KEY` and `AUTH_KEY_ID` can be set with the appropriate values
2. Write a json, ini or xml file and use the required Configuration Builder extension like so:

### Environment Variables

```csharp
  var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();    
  var client = new DragonchainClient(myDcId, config);
```

### Json

```csharp
  var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();    
  var client = new DragonchainClient(myDcId, config);
```

```json
{
  "dragonchainId": "3f2fef78-0000-0000-0000-9f2971607130",
  "AUTH_KEY": "MyAuthKey",
  "AUTH_KEY_ID": "MyAuthKeyId"
 }
 ```
 or
 ```json
{
  "dragonchainId": "3f2fef78-0000-0000-0000-9f2971607130",
  "3f2fef78-0000-0000-0000-9f2971607130": {
    "AUTH_KEY": "MyAuthKey",
    "AUTH_KEY_ID": "MyAuthKeyId"
  }
 }
 ```

### INI

```csharp
  var config = new ConfigurationBuilder()
    .AddIniFile("config.ini")
    .Build();    
  var client = new DragonchainClient(myDcId, config);
```

```ini
dragonchainId=3f2fef78-0000-0000-0000-9f2971607130
AUTH_KEY=MyAuthKey
AUTH_KEY_ID=MyAuthKeyId
```
or
```ini
dragonchainId=3f2fef78-0000-0000-0000-9f2971607130
[3f2fef78-0000-0000-0000-9f2971607130]
AUTH_KEY=MyAuthKey
AUTH_KEY_ID=MyAuthKeyId
```

### XML

```csharp
  var config = new ConfigurationBuilder()
    .AddXmlFile("config.xml")
    .Build();    
  var client = new DragonchainClient(myDcId, config);
```

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <dragonchainId>3f2fef78-0000-0000-0000-9f2971607130</dragonchainId>
  <AUTH_KEY>MyAuthKey</AUTH_KEY>
  <AUTH_KEY_ID>MyAuthKeyId</AUTH_KEY_ID>
</configuration>
```
or
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>  
  <dragonchainId>3f2fef78-0000-0000-0000-9f2971607130</dragonchainId>
  <3f2fef78-0000-0000-0000-9f2971607130>
    <AUTH_KEY>MyAuthKey</AUTH_KEY>
    <AUTH_KEY_ID>MyAuthKeyId</AUTH_KEY_ID>  
  </3f2fef78-0000-0000-0000-9f2971607130>
</configuration>
```

## Logging

In order to get the logging output of the sdk, a logger must be set (by default all logging is thrown away).

In order to set the logger, simply inject a Microsoft.Extensions.Logging implementation. 
Read here for more information [Microsoft Logging Docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.2). 
For example, if you just wanted to log to the console you can set the logger like the following:

```csharp
var webHost = new WebHostBuilder()        
  .ConfigureLogging((hostingContext, logging) =>
  {
    logging.AddConsole();            
  })
  .UseStartup<Startup>()
  .Build();
```

You can also create your own implemnations of ILogger

```csharp
var logger = new MyLogger();
var client = new DragonchainClient(myDcId, config, logger);                
```

## Official Github
https://github.com/dragonchain-inc
