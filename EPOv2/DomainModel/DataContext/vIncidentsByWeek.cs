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
    
    public partial class vIncidentsByWeek
    {
        public long IncidentsByWeekId { get; set; }
        public Nullable<int> Week { get; set; }
        public Nullable<int> Month { get; set; }
        public string MonthName { get; set; }
        public Nullable<int> Year { get; set; }
        public string Site { get; set; }
        public Nullable<int> Count { get; set; }
    }
}
