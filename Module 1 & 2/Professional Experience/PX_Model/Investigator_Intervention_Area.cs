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
    
    public partial class Investigator_Intervention_Area
    {
        public int Id { get; set; }
        public Nullable<int> Investigator_Id { get; set; }
        public Nullable<int> Intervention_Area_Id { get; set; }
    
        public virtual Intervention_Area Intervention_Area { get; set; }
        public virtual Investigator Investigator { get; set; }
    }
}
