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
    
    public partial class tblStagingCostCentresDEV
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblStagingCostCentresDEV()
        {
            this.tblStagingAccCCs = new HashSet<tblStagingAccCC>();
        }
    
        public int StagingCostCentreID { get; set; }
        public int EntityID { get; set; }
        public int CostCentreCode { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
    
        public virtual tblEntity tblEntity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblStagingAccCC> tblStagingAccCCs { get; set; }
    }
}
