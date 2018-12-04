using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using RabbitMQ.Client;
using System.Text;

namespace TopicEngine.Services
{
	public class TopicBroadcaster
	{
		public TopicBroadcaster()
		{
		}

		public void BroadcastTopics(String Topic)
		{
			Console.WriteLine("--Broadcasting Topic--");
			var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "rabbitmq", Password = "rabbitmq" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
					queue: "Topic",
					durable: false,
					exclusive: false,
					autoDelete: false,
					arguments: null
                );

                // String jsonified = JsonConvert.SerializeObject(new List<string>());
                // byte[] someBuffer = Encoding.UTF8.GetBytes(jsonified);

                var body = Encoding.UTF8.GetBytes(Topic);

                channel.BasicPublish(
					exchange: "",
					routingKey: "Topic",
					basicProperties: null,
					body: body
                );
                Console.WriteLine("--{0} Topic Queued--", Topic);
            }
		}
	}
}
