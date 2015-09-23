using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class AddScreeningCriteriaToTrialViewModel
    {
        public IEnumerable<PX_Model.Trial> Trials { get; set; }
        public IEnumerable<PX_Model.Screening_Criteria> ScreeningCriteria { get; set; }

        [Required]
        [Display(Name = "Trial")]
        public int TrialId { get; set; }

        [Required]
        [Display(Name = "Screening Criteria")]
        public int ScreeningCriteriaId { get; set; }

        [Required]
        [Display(Name = "Operator")]
        public int OperatorType { get; set; }

        [Required]
        [Display(Name = "Value")]
        public string OperatorValue { get; set; }
    }
}