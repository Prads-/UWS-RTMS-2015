using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class EditTrialViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "Objective")]
        public string Objective { get; set; }

        [Required]
        [Display(Name = "Hypothesis")]
        public string Hypothesis { get; set; }

        [Required]
        [Display(Name = "Outcome")]
        public string Outcome { get; set; }

        [Required]
        [Display(Name = "Terms and Conditions")]
        public string TermsAndConditions { get; set; }
    }
}