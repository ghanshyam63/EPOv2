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
    
    public partial class ih_hist
    {
        public string ih_nbr { get; set; }
        public string ih_cust { get; set; }
        public string ih_ship { get; set; }
        public Nullable<System.DateTime> ih_ord_date { get; set; }
        public Nullable<System.DateTime> ih_req_date { get; set; }
        public Nullable<System.DateTime> ih_due_date { get; set; }
        public string ih_rmks { get; set; }
        public string ih_po { get; set; }
        public string ih_inv_nbr { get; set; }
        public Nullable<bool> ih_invoiced { get; set; }
        public Nullable<System.DateTime> ih_inv_date { get; set; }
        public Nullable<System.DateTime> ih_ship_date { get; set; }
        public string ih_site { get; set; }
        public string ih_bill { get; set; }
        public string ih_cust_po { get; set; }
        public string ih__chr03 { get; set; }
        public Nullable<System.DateTime> ar_effdate { get; set; }
    }
}
