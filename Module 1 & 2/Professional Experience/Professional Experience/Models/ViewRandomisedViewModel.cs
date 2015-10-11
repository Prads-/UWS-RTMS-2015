using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class ViewRandomisedViewModel
    {
        public PX_Model.Trial Trial { get; set; }
        public IEnumerable<PX_Model.Participant_Group> ParticipantGroups { get; set; }
    }
}