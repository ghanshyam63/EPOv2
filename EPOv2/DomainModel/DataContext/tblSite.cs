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
    
    public partial class tblSite
    {
        public int SiteID { get; set; }
        public string SiteCode { get; set; }
        public Nullable<int> EntityID { get; set; }
        public string SupplierPattern { get; set; }
        public string UserID { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
    
        public virtual tblEntity tblEntity { get; set; }
    }
}