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
    
    public partial class EmployeeTotalTime
    {
        public int EmployeeTotalTimeId { get; set; }
        public string Site { get; set; }
        public System.DateTime WorkDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmpNo { get; set; }
        public string Department { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public decimal TotalTime { get; set; }
        public bool Salary { get; set; }
        public string State { get; set; }
        public string OrgUnitName { get; set; }
        public string Award { get; set; }
        public string PayCode { get; set; }
        public string GroupDescription { get; set; }
    }
}