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
    
    public partial class sys_zones
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sys_zones()
        {
            this.sys_distributionCentres = new HashSet<sys_distributionCentres>();
        }
    
        public string id { get; set; }
        public string state { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string updatedBy { get; set; }
        public System.DateTime updatedOn { get; set; }
        public string createdBy { get; set; }
        public System.DateTime createdOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sys_distributionCentres> sys_distributionCentres { get; set; }
        public virtual sys_states sys_states { get; set; }
    }
}
