using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using MimeKit;
using MailKit.Net.Smtp;
using MobilivaCase.Infrastructure.MessageQueue;
using MobilivaCase.Application.DTOs;
using MobilivaCase.Application.Interfaces;


namespace MobilivaCase.MailWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMqService _rabbitMQService;
        private IEmailService _emailService;


        public Worker(ILogger<Worker> logger,RabbitMqService rabbitMqService, IConfiguration config, IEmailService emailService)
        {
            _logger = logger;
            _rabbitMQService = rabbitMqService;
            _config = config;
            _connection = rabbitMqService.Connection;

            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: _config["RabbitMq:Queue"],
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            _logger.LogInformation("MailWorker is started");
            _emailService = emailService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                _logger.LogInformation($"[MailWorker] Message Received: {json}");

                var mailData = JsonSerializer.Deserialize<MailDataDTO>(json);

                await _emailService.SendEmailAsync(mailData);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(
                queue: _config["RabbitMq:Queue"],
                autoAck: false,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

 
       
    }


}
