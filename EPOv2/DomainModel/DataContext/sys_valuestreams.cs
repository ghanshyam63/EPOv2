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
    
    public partial class sys_valuestreams
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sys_valuestreams()
        {
            this.sys_subvaluestreams = new HashSet<sys_subvaluestreams>();
        }
    
        public string vsCode { get; set; }
        public string vsDescription { get; set; }
        public string updatedBy { get; set; }
        public System.DateTime updatedOn { get; set; }
        public string createdBy { get; set; }
        public System.DateTime createdOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sys_subvaluestreams> sys_subvaluestreams { get; set; }
    }
}