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
    
    public partial class GLCodeMapping
    {
        public int MappingID { get; set; }
        public string SiteID { get; set; }
        public string tic_paycode { get; set; }
        public string EmployeeType { get; set; }
        public string CostCentre { get; set; }
        public string AccountCode { get; set; }
        public string SubAccountCode { get; set; }
        public Nullable<bool> Active { get; set; }
        public string UserID { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
    
        public virtual Site Site { get; set; }
    }
}
