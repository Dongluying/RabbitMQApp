// This is a .NET 8.0 project. 
// The code uses the latest C# 12.0 features and .NET 8.0 capabilities.
// The code is a console application that sends an order message to RabbitMQ using dependency injection and configuration from appsettings.json.
// Also uses Serilog for logging to a text file configured from appsettings.json.

using BusinessLibrary.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQSender_net10;
using Serilog;
using Serilog.Events;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false);

//Configure Serilog from configuration (appsettings.json)
Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.Enrich.FromLogContext()
	.CreateLogger();

builder.Logging.ClearProviders(); // this need using Microsoft.Extensions.Logging;
builder.Logging.AddSerilog();

builder.Services.Configure<RabbitMqOptions>(
	builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton(sp =>
	sp.GetRequiredService<
		Microsoft.Extensions.Options.IOptions<RabbitMqOptions>>().Value);

builder.Services.AddSingleton<RabbitMqSender>();

using var host = builder.Build();

var sender = host.Services.GetRequiredService<RabbitMqSender>();

var order = new Order
{
	Id = 1,
	CustomerName = "John Smith",
	TotalAmount = 199.99m,
	ProductName = "Laptop",
	OrderDate = DateTime.UtcNow
};

sender.Send(order);

Log.Information("Order sent to RabbitMQ.");
