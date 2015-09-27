using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Professional_Experience.Models
{
    public class ScreeningCriteriaViewModel
    {
        public int TrialScreeningCriteriaId { get; set; }
        public string Description { get; set; }
        public List<string> Options { get; set; }

        public string Answer { get; set; }
    }
}