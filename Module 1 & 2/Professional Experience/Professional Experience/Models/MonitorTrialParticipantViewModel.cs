using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class MonitorTrialParticipantViewModel
    {
        public class Question
        {
            public string Ques { get; set; }
            public List<string> Answers { get; set; }
        }

        public class BaselineAssessment
        {
            public string Name;
            public List<Question> Questions { get; set; }
        }

        public List<BaselineAssessment> BaselineAssessments { get; set; }
        public PX_Model.Trial Trial { get; set; }
    }
}