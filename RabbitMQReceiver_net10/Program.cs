// This is a .NET 8.0 project. 
// The code uses the latest C# 12.0 features and .NET 8.0 capabilities.
// The code is a console application host worker service that receive an order message from RabbitMQ using dependency injection and configuration from appsettings.json.
// Also uses Serilog for logging to a text file configured from appsettings.json.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQReceiver_net10;
using Serilog;
using Serilog.Events;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false);

//Configure Serilog from configuration (appsettings.json)
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.Enrich.FromLogContext()
	.CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.Configure<RabbitMqOptions>(
	builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton(sp =>
	sp.GetRequiredService<
		Microsoft.Extensions.Options.IOptions<RabbitMqOptions>>().Value);

builder.Services.AddHostedService<RabbitMqReceiver>();

try
{
	Log.Information("Starting host");
	using var host = builder.Build();
	host.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}
