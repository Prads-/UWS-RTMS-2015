using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class AddParticipantToTrialViewModel
    {
        public IEnumerable<PX_Model.Trial> Trials { get; set; }
        public IEnumerable<PX_Model.Participant> Participants { get; set; }

        [Required]
        [Display(Name = "Trial")]
        public int TrialId { get; set; }

        [Required]
        [Display(Name = "Participant")]
        public int ParticipantId { get; set; }
    }
}