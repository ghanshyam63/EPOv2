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
    
    public partial class tblAPSubstitutionLog
    {
        public int id { get; set; }
        public string authoriser { get; set; }
        public string authoriserUsername { get; set; }
        public string substitution { get; set; }
        public string substitutionUsername { get; set; }
        public int documentID { get; set; }
        public Nullable<int> substituteID { get; set; }
        public System.DateTime created { get; set; }
    }
}