using ProcessStudentDetailsService.RabbitMqHelper;

namespace ProcessStudentDetailsService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRabbitMq _rabbitMqConnection;

        public Worker(ILogger<Worker> logger, IRabbitMq rabbitMqConnection)
        {
            _logger = logger;
            _rabbitMqConnection = rabbitMqConnection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Listening for messages in Rabbit Mq");
            _rabbitMqConnection.listenForMessage();
            await Task.CompletedTask;
        }
    }
}