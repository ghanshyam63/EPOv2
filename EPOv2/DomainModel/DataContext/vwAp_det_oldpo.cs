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
    
    public partial class vwAp_det_oldpo
    {
        public int documentID { get; set; }
        public int voucherID { get; set; }
        public int documentType { get; set; }
        public string fileName { get; set; }
        public string comments { get; set; }
        public bool authorised { get; set; }
        public bool Checked { get; set; }
        public string CheckedBy { get; set; }
        public string authoriser { get; set; }
        public string authoriserEmail { get; set; }
        public Nullable<System.DateTime> authorisedDate { get; set; }
        public string userID { get; set; }
        public System.DateTime created { get; set; }
        public System.DateTime updated { get; set; }
    }
}
