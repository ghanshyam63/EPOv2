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
    
    public partial class tblPOHeader
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblPOHeader()
        {
            this.tblPODetails = new HashSet<tblPODetail>();
        }
    
        public int POID { get; set; }
        public int PONumber { get; set; }
        public int RevisionNumber { get; set; }
        public System.DateTime OrderDate { get; set; }
        public int SupplierID { get; set; }
        public int GroupID { get; set; }
        public int EntityID { get; set; }
        public int DeliveryID { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public Nullable<int> CostCentreID { get; set; }
        public string InternalComments { get; set; }
        public string Comments { get; set; }
        public int TransmissionMethod { get; set; }
        public int StatusID { get; set; }
        public string SupplierContact { get; set; }
        public string SupplierFax { get; set; }
        public string SupplierEmail { get; set; }
        public string Author { get; set; }
        public string receiptor { get; set; }
        public string Authoriser { get; set; }
        public string DecliningComments { get; set; }
        public bool Active { get; set; }
        public string UserID { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblPODetail> tblPODetails { get; set; }
    }
}
