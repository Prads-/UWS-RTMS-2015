using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class TakeBaselineAssessmentViewModel
    {
        public int AssessmentTypeId { get; set; }

        public List<AnswerBaselineAssessmentQuestionViewModel> Answers { get; set; }
    }
}