using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text;
using InitiateAdmission.Model;
using RabbitMQ.Client;

namespace InitiateAdmission.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolDetailsController : ControllerBase
    {
        private readonly List<string> validStates = GetValidStates();
        private readonly List<string> AvailableCourses = new List<string> { "MEDICINE", "ENGINEERING", "TECHNOLOGY", "MANAGEMENT", "SCIENCE", "AGRICULTURE", "MATHEMATICS" };

        [HttpPost]
            public IActionResult Register(StudentDetails studentDetails)
            {
                   string course = studentDetails.Course.ToUpper();
                   string state = studentDetails.StateOfOrigin.ToUpper();
                try
                {
                    if (studentDetails == null)
                    {
                        return StatusCode(400, "student details field cannot be empty");
                    }
                    if(!AvailableCourses.Contains(course))
                    {
                         return StatusCode(400, $"The course {studentDetails.Course} is not offered in our school");
                    }
                    if(!validStates.Contains(state))
                    {
                        return StatusCode(400, $"This state {studentDetails.StateOfOrigin} is not a Nigerian State");
                    }
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "admission_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                        var message = JsonSerializer.Serialize(studentDetails);
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "", routingKey: "admission_queue", basicProperties: null, body: body);

                        return StatusCode(200, "User registration message sent to the admission queue");
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }



        private static List<string> GetValidStates()
        {
            List<string> validStates = new List<string>
            {
                "ABIA", "ADAMAWA", "AKWA IBOM", "ANAMBRA", "BAUCHI", "BAYELSA", "BENUE", "BORNO", "CROSS RIVER",
                "DELTA", "EBONYI", "EDO", "EKITI", "ENUGU", "GOMBE", "IMO", "JIGAWA", "KADUNA", "KANO", "KATSINA",
                "KEBBI", "KOGI", "KWARA", "LAGOS", "NASARAWA", "NIGER", "OGUN", "ONDO", "OSUN", "OYO", "PLATEAU",
                "RIVERS", "SOKOTO", "TARABA", "YOBE", "ZAMFARA"
            };
            return validStates;
        }
    }

}

