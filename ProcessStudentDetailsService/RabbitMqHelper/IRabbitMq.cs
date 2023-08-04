using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessStudentDetailsService.RabbitMqHelper
{
    public interface IRabbitMq
    {
        void listenForMessage();
    }
}
