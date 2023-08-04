using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessStudentDetailsService.Model
{
    public class StudentDetails
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Course { get; set; }
        public double JambScore { get; set; }
        public string StateOfOrigin { get; set; }
        public bool IsAdmitted { get; set; }
        public int StudentId { get; set;}
        public double AdmissionScore { get; set; }
        public double CutOffMark { get; set; }
    }
}
