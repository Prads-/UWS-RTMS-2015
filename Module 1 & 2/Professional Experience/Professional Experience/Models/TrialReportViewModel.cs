using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class TrialReportViewModel
    {
        public string TrialName { get; set; }
        public int NumberOfMales { get; set; }
        public int NumberOfFemales { get; set; }
        public int NumberOfInterventionParticipant { get; set; }
        public int NumberOfExperimentalParticipant { get; set; }

        public class Option
        {
            public string Opt;
            public double Percentage;
        }

        public class Question
        {
            public string Q;
            public List<Option> Options;
        }

        public class BaselineAssessment
        {
            public string Name;
            public List<Question> Questions;
        }

        public List<BaselineAssessment> BaselineAssessments;
    }
}