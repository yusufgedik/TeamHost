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
    
    public partial class UserDevice
    {
        public int ID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string NotificationKey { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual User User { get; set; }
    }
}
