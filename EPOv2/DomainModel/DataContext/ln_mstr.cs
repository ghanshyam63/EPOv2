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
    
    public partial class ln_mstr
    {
        public string ln_line { get; set; }
        public string ln_site { get; set; }
        public string ln_desc { get; set; }
        public string ln_user1 { get; set; }
        public string ln_user2 { get; set; }
        public Nullable<decimal> ln_rate { get; set; }
        public Nullable<bool> ln_rate_base { get; set; }
        public Nullable<decimal> ln_shift1 { get; set; }
        public Nullable<decimal> ln_shift2 { get; set; }
        public Nullable<decimal> ln_shift3 { get; set; }
        public string ln__chr01 { get; set; }
        public string ln__chr03 { get; set; }
        public string ln__chr04 { get; set; }
        public string ln__chr05 { get; set; }
        public Nullable<decimal> ln__dec01 { get; set; }
        public Nullable<decimal> ln__dec02 { get; set; }
        public Nullable<decimal> ln__dec03 { get; set; }
        public Nullable<bool> ln__log01 { get; set; }
        public Nullable<decimal> ln_shift4 { get; set; }
        public string ln_schedule_code { get; set; }
        public string ln_rate_code { get; set; }
        public int ln_freeze_period { get; set; }
        public bool ln_kanban_receipts { get; set; }
        public string ln_domain { get; set; }
        public decimal oid_ln_mstr { get; set; }
    }
}
