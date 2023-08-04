using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Configuration;
using ProcessStudentDetailsService;
using ProcessStudentDetailsService.AdmissionManager;
using ProcessStudentDetailsService.EmailSender.EmailHelper;
using ProcessStudentDetailsService.RabbitMqHelper;
using ProcessStudentDetailsService.Service;
using ProcessStudentDetailsService.Utils;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();
IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((hostContext,services) =>
    {
        services.AddHostedService<Worker>();
        services.Configure<EmailConfiguration>(hostContext.Configuration.GetSection("EmailConfiguration"));
        services.Configure<RabbitMq>(hostContext.Configuration.GetSection("RabbitMq"));
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<IRabbitMq,RabbitMqConnection>();
        services.AddSingleton<IAdmissionProcessingService, AdmissionProcessingService>();
        services.AddHostedService<Worker>();
        services.AddSingleton<ScoreProcessing>();
    })
    .Build();

  host.Run();
