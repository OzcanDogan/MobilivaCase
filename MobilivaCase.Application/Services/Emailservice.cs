using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MobilivaCase.Application.DTOs;
using MobilivaCase.Application.Interfaces;
using System;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MobilivaCase.Application.Services
{
    public class Emailservice : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<Emailservice> _logger;
        public Emailservice(IConfiguration config, ILogger<Emailservice> logger)
        {
            _config = config;
            _logger = logger;
        }
        public async Task SendEmailAsync(MailDataDTO data)
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
}
