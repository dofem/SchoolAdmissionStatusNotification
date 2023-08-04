using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessStudentDetailsService.AdmissionManager
{
    public class AdmissionCutOff
    {
        public Dictionary<string, int> CutoffMarks { get; set; }

        public AdmissionCutOff()
        {
            CutoffMarks = new Dictionary<string, int>
            {
                { "ENGINEERING", 80 },
                { "MEDICINE", 90 },
                { "SCIENCE", 75 },
                { "MANAGEMENT", 70 },
                { "MATHEMATICS", 85 },
                { "AGRICULTURE", 60 },
                { "TECHNOLOGY", 80 }
            };
        }    
    }
}
