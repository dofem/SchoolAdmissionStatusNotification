using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessStudentDetailsService.ApplicationException
{
    public class AdmissionDetailsExtractionException : Exception
    {
        public AdmissionDetailsExtractionException(string message) : base(message)
        {
            
        }
    }
}
