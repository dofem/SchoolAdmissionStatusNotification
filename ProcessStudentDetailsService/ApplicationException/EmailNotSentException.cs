using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessStudentDetailsService.ApplicationException
{
    public class EmailNotSentException : Exception
    {
        public EmailNotSentException(string message) : base(message)
        {
            
        }
    }
}
