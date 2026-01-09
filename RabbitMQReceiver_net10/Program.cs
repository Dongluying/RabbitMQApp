using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQReceiver_net10;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false);

builder.Services.Configure<RabbitMqOptions>(
	builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton(sp =>
	sp.GetRequiredService<
		Microsoft.Extensions.Options.IOptions<RabbitMqOptions>>().Value);

builder.Services.AddHostedService<RabbitMqReceiver>();

using var host = builder.Build();
host.Run();
