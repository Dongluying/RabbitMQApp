using BusinessLibrary.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMQReceiver_net10
{
	public sealed class RabbitMqReceiver : BackgroundService
	{
		private readonly RabbitMqOptions _options;
		private readonly ILogger<RabbitMqReceiver> _logger;

		private IConnection? _connection;
		private IModel? _channel;

		public RabbitMqReceiver(
			RabbitMqOptions options,
			ILogger<RabbitMqReceiver> logger)
		{
			_options = options;
			_logger = logger;
		}

		public override Task StartAsync(CancellationToken cancellationToken)
		{
			var factory = new ConnectionFactory
			{
				HostName = _options.Host,
				UserName = _options.User,
				Password = _options.Password,
				DispatchConsumersAsync = true
			};

			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();

			_channel.QueueDeclare(
				queue: _options.Queue,
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null);

			_channel.BasicQos(0, 1, false); // one message at a time

			return base.StartAsync(cancellationToken);
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var consumer = new AsyncEventingBasicConsumer(_channel!);

			consumer.Received += async (sender, args) =>
			{
				try
				{
					var body = args.Body.ToArray();
					var message = Encoding.UTF8.GetString(body);

					var order = JsonSerializer.Deserialize<Order>(message);

					_logger.LogInformation($"Order {order!.Id} from {order.Id}");
					_logger.LogInformation($"Order {order!.CustomerName} from {order.CustomerName}");
					_logger.LogInformation($"Order {order!.OrderDate} from {order.OrderDate}");
					_logger.LogInformation($"Order {order!.ProductName} from {order.ProductName}");

					// Simulate work
					await Task.Delay(500, stoppingToken);

					_channel!.BasicAck(args.DeliveryTag, multiple: false);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error processing message");
					_channel!.BasicNack(args.DeliveryTag, false, requeue: true);
				}
			};

			_channel!.BasicConsume(
				queue: _options.Queue,
				autoAck: false,
				consumer: consumer);

			return Task.CompletedTask;
		}

		public override void Dispose()
		{
			_channel?.Close();
			_connection?.Close();
			base.Dispose();
		}
	}
}
