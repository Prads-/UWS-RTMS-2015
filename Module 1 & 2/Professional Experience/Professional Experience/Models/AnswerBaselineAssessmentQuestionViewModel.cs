using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class AnswerBaselineAssessmentQuestionViewModel
    {
        public int QuestionId { get; set; }
        public int QuestionType { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public List<string> Options { get; set; }
    }
}