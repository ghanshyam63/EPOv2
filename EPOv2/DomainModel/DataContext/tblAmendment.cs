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
    
    public partial class tblAmendment
    {
        public int AmendmentID { get; set; }
        public Nullable<int> POID { get; set; }
        public Nullable<int> AmendType { get; set; }
        public string Contact { get; set; }
        public string AmendDateTime { get; set; }
        public string RefNumber { get; set; }
        public string Comments { get; set; }
        public string UserID { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Updated { get; set; }
    }
}
