//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DomainModel.DataContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblRoutingTable
    {
        public int RoutingID { get; set; }
        public int POLineID { get; set; }
        public int ListID { get; set; }
        public int ApprovalStatus { get; set; }
        public Nullable<System.DateTime> ApprovalDate { get; set; }
        public bool Active { get; set; }
        public string UserID { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
    
        public virtual tblRoutingList tblRoutingList { get; set; }
    }
}
