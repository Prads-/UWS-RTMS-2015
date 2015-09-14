//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PX_Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Participant
    {
        public Participant()
        {
            this.Participant_Clinician = new HashSet<Participant_Clinician>();
            this.Participant_Screening_Criteria = new HashSet<Participant_Screening_Criteria>();
            this.Trial_Participant = new HashSet<Trial_Participant>();
        }
    
        public int Id { get; set; }
        public Nullable<int> Person_Id { get; set; }
        public Nullable<System.DateTime> Date_Of_Birth { get; set; }
        public string Gender { get; set; }
    
        public virtual ICollection<Participant_Clinician> Participant_Clinician { get; set; }
        public virtual ICollection<Participant_Screening_Criteria> Participant_Screening_Criteria { get; set; }
        public virtual Person Person { get; set; }
        public virtual ICollection<Trial_Participant> Trial_Participant { get; set; }
    }
}