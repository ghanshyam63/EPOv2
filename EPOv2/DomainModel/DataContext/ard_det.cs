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
    
    public partial class ard_det
    {
        public string ard_nbr { get; set; }
        public string ard_acct { get; set; }
        public string ard_cc { get; set; }
        public Nullable<decimal> ard_amt { get; set; }
        public string ard_desc { get; set; }
        public string ard_ref { get; set; }
        public Nullable<decimal> ard_disc { get; set; }
        public string ard_type { get; set; }
        public string ard_user1 { get; set; }
        public string ard_user2 { get; set; }
        public string ard_tax { get; set; }
        public string ard_tax_at { get; set; }
        public string ard_entity { get; set; }
        public string ard__qad02 { get; set; }
        public Nullable<System.DateTime> ard__qad01 { get; set; }
        public string ard_project { get; set; }
        public Nullable<decimal> ard_cur_amt { get; set; }
        public Nullable<decimal> ard_cur_disc { get; set; }
        public Nullable<decimal> ard_ex_rate { get; set; }
        public string ard_tax_usage { get; set; }
        public string ard_taxc { get; set; }
        public string ard_dy_code { get; set; }
        public string ard_dy_num { get; set; }
        public Nullable<decimal> ard_ex_rate2 { get; set; }
        public string ard_ex_ratetype { get; set; }
        public Nullable<int> ard_ded_nbr { get; set; }
        public Nullable<int> ard_exru_seq { get; set; }
        public string ard_sub { get; set; }
        public string ard_domain { get; set; }
        public Nullable<decimal> oid_ard_det { get; set; }
    
        public virtual ar_mstr ar_mstr { get; set; }
    }
}