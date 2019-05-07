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
    
    public partial class Cube2015_Fact_Yield
    {
        public int ID { get; set; }
        public string Parent { get; set; }
        public string Component { get; set; }
        public Nullable<int> WOLot { get; set; }
        public string Site { get; set; }
        public string WoLine { get; set; }
        public Nullable<decimal> TotalQtyCompleted { get; set; }
        public Nullable<decimal> ComponentQtyCompleted { get; set; }
        public Nullable<decimal> ComponentQtyIssued { get; set; }
        public Nullable<decimal> ComponentQtyBOM { get; set; }
        public Nullable<decimal> StandardQtyCompleted { get; set; }
        public Nullable<decimal> StdYield { get; set; }
        public Nullable<decimal> ActualYield { get; set; }
        public Nullable<decimal> StandardCost { get; set; }
        public Nullable<decimal> TotalVariance { get; set; }
        public Nullable<decimal> YieldVariance { get; set; }
        public Nullable<decimal> SubstitutionVariance { get; set; }
        public Nullable<decimal> OtherVariance { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
    }
}
