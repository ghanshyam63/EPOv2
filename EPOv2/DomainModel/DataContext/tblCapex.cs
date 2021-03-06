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
    
    public partial class tblCapex
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblCapex()
        {
            this.tblPODetails = new HashSet<tblPODetail>();
        }
    
        public int CapexID { get; set; }
        public Nullable<int> Revision { get; set; }
        public string CapexNumber { get; set; }
        public Nullable<int> PrevCapexID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal TotalExGST { get; set; }
        public Nullable<decimal> TotalGST { get; set; }
        public Nullable<decimal> TotalIncGST { get; set; }
        public int StatusID { get; set; }
        public string Author { get; set; }
        public string Owner { get; set; }
        public string FilePath { get; set; }
        public Nullable<int> EntityID { get; set; }
        public Nullable<int> CostCentreID { get; set; }
        public bool Active { get; set; }
        public string UserID { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPODetail> tblPODetails { get; set; }
    }
}
