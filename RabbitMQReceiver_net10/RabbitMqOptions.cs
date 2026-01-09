using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQReceiver_net10
{
	public sealed class RabbitMqOptions
	{
		public string Host { get; init; } = default!;
		public string User { get; init; } = default!;
		public string Password { get; init; } = default!;
		public string Queue { get; init; } = default!;
	}
}
