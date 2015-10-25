using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace Professional_Experience.Models
{
    public class AdminViewTrialsViewModel
    {
        public PagedList<PX_Model.Trial> Randomised { get; set; }
        public PagedList<PX_Model.Trial> NonRandomised { get; set; }
    }
}