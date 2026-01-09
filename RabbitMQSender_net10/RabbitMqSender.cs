using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMQSender_net10
{
	public sealed class RabbitMqSender : IDisposable
	{
		private readonly IConnection _connection;
		private readonly IModel _channel;
		private readonly RabbitMqOptions _options;

		public RabbitMqSender(RabbitMqOptions options)
		{
			_options = options;

			var factory = new ConnectionFactory
			{
				HostName = options.Host,
				UserName = options.User,
				Password = options.Password,
				DispatchConsumersAsync = true
			};

			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();

			_channel.QueueDeclare(
				queue: options.Queue,
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null);
		}

		public void Send<T>(T message)
		{
			var jsonMessage = JsonSerializer.Serialize(message);
			var body = Encoding.UTF8.GetBytes(jsonMessage);

			var props = _channel.CreateBasicProperties();
			props.Persistent = true; // survive broker restart
			props.ContentType = "application/json";
			props.Type = typeof(T).FullName;

			_channel.BasicPublish(
				exchange: "",
				routingKey: _options.Queue,
				basicProperties: props,
				body: body);
		}

		public void Dispose()
		{
			_channel.Close();
			_connection.Close();
		}
	}
}
