using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class RandomiseViewModel
    {
        [Required]
        [Display(Name = "Trial")]
        public int TrialId { get; set; }
        
        [Required]
        [Display(Name = "How many participants do you want in the intervention group?")]
        public int NumberOfParticipantInIntervention { get; set; }

        public IEnumerable<PX_Model.Trial> Trials { get; set; }
    }
}