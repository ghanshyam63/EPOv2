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
    
    public partial class FinancialCalendar
    {
        public System.DateTime Date { get; set; }
        public string FinancialYear { get; set; }
        public string FinancialPeriod { get; set; }
        public string FinancialPeriodYear { get; set; }
        public string FinancialWeek { get; set; }
        public string FinancialWeekYear { get; set; }
        public Nullable<System.DateTime> FinancialStartingDate { get; set; }
        public Nullable<System.DateTime> FinancialEndingDate { get; set; }
        public string DayOfWeek { get; set; }
    }
}