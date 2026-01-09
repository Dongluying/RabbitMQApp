using System.Text;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQSender_net10;
using BusinessLibrary.Models;
 
var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false);

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
	CustomerName = "Don Han",
	TotalAmount = 199.99m,
	ProductName = "Laptop",
	OrderDate = DateTime.UtcNow
};

sender.Send(order);

Console.WriteLine("Order sent to RabbitMQ.");
