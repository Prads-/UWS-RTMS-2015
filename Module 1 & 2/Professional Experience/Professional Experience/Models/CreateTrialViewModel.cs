using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Professional_Experience.Models
{
    public class CreateTrialViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        [AllowHtml]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Objective")]
        [AllowHtml]
        public string Objective { get; set; }

        [Required]
        [Display(Name = "Hypothesis")]
        [AllowHtml]
        public string Hypothesis { get; set; }

        [Required]
        [Display(Name = "Outcome")]
        [AllowHtml]
        public string Outcome { get; set; }

        [Required]
        [Display(Name = "Terms and Conditions")]
        [AllowHtml]
        public string TermsAndConditions { get; set; }
    }
}