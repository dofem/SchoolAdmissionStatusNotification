using Microsoft.Extensions.Options;
using ProcessStudentDetailsService.AdmissionManager;
using ProcessStudentDetailsService.ApplicationException;
using ProcessStudentDetailsService.EmailSender.EmailHelper;
using ProcessStudentDetailsService.Model;
using ProcessStudentDetailsService.Utils;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProcessStudentDetailsService.Service
{
    public class AdmissionProcessingService : IAdmissionProcessingService
    {
        private readonly ScoreProcessing _scoreProcessing;
        private readonly IEmailService _emailService;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly ILogger<AdmissionProcessingService> _logger;

        public AdmissionProcessingService(ScoreProcessing scoreProcessing,
            IEmailService emailService,
            IOptions<EmailConfiguration> emailConfiguration,
            ILogger<AdmissionProcessingService> logger)
        {
            _scoreProcessing = scoreProcessing;
            _emailService = emailService;
            _emailConfiguration = emailConfiguration.Value;
            _logger = logger;
        }
        public async Task Handle(IModel context, BasicDeliverEventArgs args)
        {
            string message = string.Empty;
            bool acknowledgeMessage = true;
            try
            {
                var body = args.Body.ToArray();
                message = Encoding.UTF8.GetString(body);
                StudentDetails studentDetails = GetStudentDetails(message);
                if (studentDetails != null)
                {
                    if (studentDetails.Email != null && ValidateEmail(studentDetails.Email))
                    {
                        _ = Task.Run(() => ProcessEmail(studentDetails));
                    }
                }
            }
            catch(AdmissionDetailsExtractionException ex)
            {
                acknowledgeMessage = false;
            }
            catch (Exception ex)
            {
                if(string.IsNullOrEmpty(message))
                {
                    _logger.LogError(message);
                }
            }
            if(acknowledgeMessage) 
            {
                context.BasicAck(args.DeliveryTag, true);
            }                   
        }

        private static StudentDetails GetStudentDetails (string messageContent)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Model.StudentDetails>(messageContent);
        }


        private async Task ProcessEmail(StudentDetails studentDetails)
        {
            studentDetails.IsAdmitted = _scoreProcessing.ProcessAdmission(studentDetails);
            string subject = studentDetails.IsAdmitted ? "Congratulations! Your Admission Status" : "Admission Status Update";
            var emailAddress = studentDetails.Email;
            var mailBody = GenerateAdmissionMail(studentDetails);
            _emailService.SendEmail(_emailConfiguration.FromEmail,emailAddress,subject,mailBody);
        }


        private string GenerateAdmissionMail(StudentDetails studentDetails)
        {
            string universityName = "Makossa University";
            string mailBody = studentDetails.IsAdmitted ?
                $"Dear {studentDetails.Name},\n\n" +
                $"We are thrilled to inform you that your application for admission to {universityName} has been successful! " +
                $"On behalf of the admissions committee, I would like to extend my heartfelt congratulations to you for securing a place in the {studentDetails.Course} for the upcoming academic year.\n\n" +
                $"Your dedication and academic achievements have impressed us. Your score of {studentDetails.AdmissionScore} demonstrates your commitment to pursuing excellence. " +
                $"We believe that you will be a valuable addition to our esteemed institution.\n\n" +
                $"The {studentDetails.Course} will provide you with an exceptional learning experience and equip you with the necessary skills to thrive in your chosen field.\n\n" +
                $"We invite you to celebrate this exciting milestone in your academic journey. Our faculty and staff are committed to supporting your growth and success throughout your time at {universityName}.\n\n" +
                $"Should you have any questions or require further information, please do not hesitate to reach out to the admissions office. " +
                $"We are here to assist you every step of the way.\n\n" +
                $"Once again, congratulations on your admission! We look forward to welcoming you to our vibrant and inclusive community at {universityName}.\n\n" +
                $"Best regards,\n\n" +
                $"[Your Name]\n" +
                $"[Your Position]\n" +
                $"{universityName}" :
                $"Dear {studentDetails.Name},\n\n" +
                $"We appreciate your interest in pursuing studies at {universityName}. After careful consideration of your application, " +
                $"we regret to inform you that you have not been selected for admission to the {studentDetails.Course} for the upcoming academic year.\n\n" +
                $"Your dedication and academic achievements have been commendable. Your score of {studentDetails.AdmissionScore} reflects your commitment and efforts in the application process.\n\n" +
                $"However, due to limited availability and a highly competitive admissions process, we had to make difficult decisions.\n\n" +
                $"Please note that this outcome does not diminish your achievements and potential. We encourage you to explore other opportunities that align with your academic goals and interests. " +
                $"Many successful individuals have pursued alternative pathways and achieved great accomplishments.\n\n" +
                $"We appreciate your interest in our institution and wish you every success in your academic and personal endeavors. " +
                $"If you have any questions or need further guidance regarding other programs or educational opportunities, please do not hesitate to contact the admissions office.\n\n" +
                $"Thank you once again for considering {universityName}, and we wish you the very best in your future endeavors.\n\n" +
                $"Sincerely,\n\n" +
                $"[Your Name]\n" +
                $"[Your Position]\n" +
                $"{universityName}";

            return mailBody;
        }


        private bool ValidateEmail(string emailAddress)
        {
            bool isValid = false;
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            foreach (string email in emailAddress.Split(new char[] { ',', ';' })){
                Match match = regex.Match(email);
                if (match.Success) 
                {
                    isValid = true;
                    break;
                }
            }
            return isValid;
        }

    }
}
