using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MobilivaCase.Services
{
    public class RabbitMqService : IDisposable
    {
        private readonly IConnection _connection;

        public RabbitMqService(IConfiguration configuration)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMq:Host"],
                UserName = configuration["RabbitMq:UserName"],
                Password = configuration["RabbitMq:Password"],
                Port = int.Parse(configuration["RabbitMq:Port"])   
            };

            _connection = factory.CreateConnection();
        }

        public void Publish(string queueName, object message)
        {
            using var channel = _connection.CreateModel();

          
            channel.QueueDeclare(
                queue: queueName,
                durable: true,        
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: properties,
                body: body
            );
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
