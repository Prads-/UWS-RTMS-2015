using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class AssignInvestigatorToTrialViewModel
    {
        public IEnumerable<PX_Model.Trial> Trials { get; set; }
        public IEnumerable<PX_Model.Investigator> Investigators { get; set; }

        [Required]
        [Display(Name = "Trial")]
        public int TrialId { get; set; }

        [Required]
        [Display(Name = "Investigator")]
        public int InvestigatorId { get; set; }

        [Required]
        [Display(Name = "Type")]
        public int Type { get; set; }
    }
}