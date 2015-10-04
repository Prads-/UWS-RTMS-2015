using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class CreateAssessmentTypeQuestionViewModel
    {
        [Required]
        public string Question { get; set; }

        [Required]
        public int Type { get; set; }

        public bool AddToQuestion { get; set; }
        public List<string> Options { get; set; }
    }
}