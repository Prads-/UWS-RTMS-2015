using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class AdminViewTrialsViewModel
    {
        public IEnumerable<PX_Model.Trial> Randomised { get; set; }
        public IEnumerable<PX_Model.Trial> NonRandomised { get; set; }
    }
}