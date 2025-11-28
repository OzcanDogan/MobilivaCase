using MobilivaCase.Application.Interfaces;
using MobilivaCase.Application.Services;
using MobilivaCase.Infrastructure.MessageQueue;
using MobilivaCase.MailWorker;

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<RabbitMqService>();
        services.AddSingleton<IEmailService,Emailservice>();
        services.AddHostedService<Worker>();
    })
    .Build()
    .Run();
