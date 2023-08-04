using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessStudentDetailsService.EmailSender.EmailHelper
{
    public interface IEmailService
    {
        void SendEmail(string from,string to,string subject,string message);
    }
}
