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
    
    public partial class tblPerformanceLog
    {
        public int id { get; set; }
        public string username { get; set; }
        public string sessionid { get; set; }
        public string ip { get; set; }
        public string description { get; set; }
        public Nullable<System.DateTime> created { get; set; }
        public string system { get; set; }
    }
}
