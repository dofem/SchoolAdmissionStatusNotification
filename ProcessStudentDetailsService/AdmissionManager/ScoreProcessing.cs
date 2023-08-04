using ProcessStudentDetailsService.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessStudentDetailsService.AdmissionManager
{
    public class ScoreProcessing
    {
        private AdmissionCutOff admissionCutoff;
        private List<string> southWestStates = new List<string> { "Lagos", "Ogun", "Ondo", "Ekiti", "Osun", "Oyo" };
        private List<string> southEastStates = new List<string> { "Abia", "Anambra", "Ebonyi", "Enugu", "Imo" };
        private List<string> southSouthStates = new List<string> { "Akwa Ibom", "Bayelsa", "Cross River", "Delta", "Edo", "Rivers" };
        private List<string> northCentralStates = new List<string> { "Benue", "Kogi", "Kwara", "Nasarawa", "Niger", "Plateau", "FCT" };
        //private double jambScoreToPercentage = 70.0 / 400.0;

        public ScoreProcessing()
        {
            admissionCutoff = new AdmissionCutOff();
        }

        public bool ProcessAdmission(StudentDetails studentDetails)
        {
            double jambScoreBy70Percent = (studentDetails.JambScore/400.0 * 70);

            double regionalBonus = GetRegionalBonusPercentage(studentDetails.StateOfOrigin);

            studentDetails.AdmissionScore = jambScoreBy70Percent + regionalBonus;

            int courseCutoff = admissionCutoff.CutoffMarks.ContainsKey(studentDetails.Course) ? admissionCutoff.CutoffMarks[studentDetails.Course] : 0;

            if(studentDetails.AdmissionScore >= courseCutoff)
            {
                return true;
            }
            return false;
        }

        private double GetRegionalBonusPercentage(string stateOfOrigin)
        {
            if (southWestStates.Contains(stateOfOrigin))
                return 30.0;
            else if (southEastStates.Contains(stateOfOrigin) || southSouthStates.Contains(stateOfOrigin))
                return 25.0;
            else if (northCentralStates.Contains(stateOfOrigin))
                return 20.0;
            else
                return 15.0;
        }
    }
}
