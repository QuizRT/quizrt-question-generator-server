using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using QuizRT.Models;

namespace TopicEngine.Services
{
	public class TopicBroadcaster
	{
		public TopicBroadcaster()
		{
		}

		public void Publish_All_Topics_as_deleted(List<string> all_topics)
		{
			//IGameContext context = new QuizRTContext();
			Console.WriteLine("--Broadcasting Topic for deletion--");
			var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "rabbitmq", Password = "rabbitmq" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
					queue: "Delete_Topic",
					durable: false,
					exclusive: false,
					autoDelete: false,
					arguments: null
                );

                // String jsonified = JsonConvert.SerializeObject(all_topics);
                // byte[] body = Encoding.UTF8.GetBytes(jsonified);
				// var json = JsonConvert.SerializeObject(obj);
                // return Encoding.ASCII.GetBytes(json);
                // var body = Encoding.UTF8.GetBytes();
				for(int i=0;i<all_topics.Count;i++)
				{
					PublishTopic_as_deleted(all_topics[i]);
				}

                // channel.BasicPublish(
				// 	exchange: "",
				// 	routingKey: "Delete_Topic",
				// 	basicProperties: null,
				// 	body: body
                // );
                Console.WriteLine("--{0} All Topics for deletion Queued--");
            }
		}

		public void PublishTopic_as_deleted(string Topic)
		{
			Console.WriteLine("--Broadcasting Topic for deletion--");
			var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "rabbitmq", Password = "rabbitmq" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
					queue: "Delete_Topic",
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
					routingKey: "Delete_Topic",
					basicProperties: null,
					body: body
                );
                Console.WriteLine("--{0} Topic for deletion Queued--", Topic);
            }	
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
