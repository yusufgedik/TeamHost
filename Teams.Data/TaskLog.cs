//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Teams.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class TaskLog
    {
        public int ID { get; set; }
        public Nullable<int> TaskID { get; set; }
        public Nullable<int> ProgressID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<int> ModifyBy { get; set; }
    
        public virtual Progress Progress { get; set; }
        public virtual Task Task { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
