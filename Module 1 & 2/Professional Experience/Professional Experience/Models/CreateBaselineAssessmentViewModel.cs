using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class CreateBaselineAssessmentViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Trial")]
        public int TrialId { get; set; }

        public List<CreateAssessmentTypeQuestionViewModel> Questions { get; set; }
        public IEnumerable<PX_Model.Trial> Trials { get; set; }
    }
}