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
    
    public partial class LabourHireCompany
    {
        public int LabourHireCompanyID { get; set; }
        public string SiteID { get; set; }
        public string tid_team { get; set; }
        public string EPOSupplierCode { get; set; }
        public Nullable<int> EPODeliveryID { get; set; }
        public Nullable<int> EPOGroupID { get; set; }
    }
}