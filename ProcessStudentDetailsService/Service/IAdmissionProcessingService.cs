using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessStudentDetailsService.Service
{
    public interface IAdmissionProcessingService
    {
        Task Handle(IModel context, BasicDeliverEventArgs args);
    }
}
