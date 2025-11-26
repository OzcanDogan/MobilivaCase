using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using MimeKit;
using MailKit.Net.Smtp;


namespace MobilivaCase.MailWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            var factory = new ConnectionFactory()
            {
                HostName = _config["RabbitMq:Host"],
                Port = int.Parse(_config["RabbitMq:Port"]),  
                UserName = _config["RabbitMq:UserName"],
                Password = _config["RabbitMq:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: _config["RabbitMq:Queue"],
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _logger.LogInformation("MailWorker is started");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                _logger.LogInformation($"[MailWorker] Message Received: {json}");

                var mailData = JsonSerializer.Deserialize<MailDataDto>(json);

                await SendEmailAsync(mailData);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(
                queue: _config["RabbitMq:Queue"],
                autoAck: false,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        private async Task SendEmailAsync(MailDataDto data)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Mobiliva", _config["Mail:User"]));
            message.To.Add(new MailboxAddress(data.CustomerName, data.CustomerEmail));

            message.Subject = $"Mobiliva Siparişiniz #{data.OrderId}";

            message.Body = new TextPart("plain")
            {
                Text = $"Merhaba {data.CustomerName}, siparişiniz alınmıştır.\nToplam Tutar: {data.Total} TL"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["Mail:Host"], int.Parse(_config["Mail:Port"]), false);
            await smtp.AuthenticateAsync(_config["Mail:User"], _config["Mail:Pass"]);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation($"Mail sent to {data.CustomerEmail}");
        }
    }

    public class MailDataDto
    {
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public decimal Total { get; set; }
    }
}
