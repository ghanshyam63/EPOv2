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
    
    public partial class vFormStatu
    {
        public int FormHeaderId { get; set; }
        public string Site { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Modified { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedByEmpNo { get; set; }
        public string ModifiedByName { get; set; }
        public string ModifiedByEmpNo { get; set; }
        public string CurrentOwnerName { get; set; }
        public string CurrentOwnerEmpNo { get; set; }
        public string FormType { get; set; }
        public string Title { get; set; }
        public string FormStatus { get; set; }
    }
}