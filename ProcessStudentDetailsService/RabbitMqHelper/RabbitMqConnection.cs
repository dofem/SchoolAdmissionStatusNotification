using Microsoft.Extensions.Options;
using ProcessStudentDetailsService.Service;
using ProcessStudentDetailsService.Utils;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ProcessStudentDetailsService.RabbitMqHelper
{
    public class RabbitMqConnection : IRabbitMq
    {
        private readonly IAdmissionProcessingService _admissionProcessingService;
        private readonly RabbitMq _rabbitMq;
        private IConnection _connection;
        private IModel channel = null;

        public RabbitMqConnection(IAdmissionProcessingService admissionProcessingService,IOptions<RabbitMq> rabbitMq) 
        {
            _admissionProcessingService = admissionProcessingService;
            _rabbitMq = rabbitMq.Value;
        }

        public void listenForMessage()
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMq.HostName,
                Port = _rabbitMq.Port,
                UserName = _rabbitMq.UserName,
                Password = _rabbitMq.Password,
            };

            _connection = factory.CreateConnection();
            channel = _connection.CreateModel();

            channel.QueueDeclare(queue: "admission_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, args) =>
            {
                _ = Task.Run(() => { _admissionProcessingService.Handle(channel, args); });
            };
            var consumerTag = channel.BasicConsume(queue: "admission_queue", autoAck:true,consumer:consumer);
        }
    }
}
