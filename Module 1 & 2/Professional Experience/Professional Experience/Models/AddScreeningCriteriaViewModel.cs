using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class AddScreeningCriteriaViewModel
    {
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Options:")]
        public IEnumerable<string> Options { get; set; }
    }
}