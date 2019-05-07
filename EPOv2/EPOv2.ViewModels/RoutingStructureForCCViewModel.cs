namespace EPOv2.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class RoutingStructureForCCViewModel
    {
        public string CostCentreName { get; set; }

        public int CostCentreCode { get; set; }

        public string CostCentreOwner { get; set; }

        public int CostCentreOwnerLevel { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double CostCentreOwnerLimit { get; set; }

        public int CostCEntreOwnerEmpId { get; set; }

        public List<RoutingStructureElement> AboveList { get; set; }

        public List<RoutingStructureElement> BehindList { get; set; }
    }

    public class RoutingStructureElement
    {
        public int EmpId { get; set; }

        public int PseudoLevel { get; set; }

        public int Level { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Limit { get; set; }

        public string FullName { get; set; }

        public int ManagerEmpId { get; set; }
    }
}