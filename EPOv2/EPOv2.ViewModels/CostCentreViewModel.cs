namespace EPOv2.ViewModels
{
    using System.Collections.Generic;

    public class CostCentreViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Code { get; set; }

        public string FullName { get; set; }

        public string Owner { get; set; }
        public bool isOwnerFree { get; set; }

    }

    public class CostCentreDDLViewModel
    {
        public int SelectedCostCentre { get; set; }

        public List<CostCentreViewModel> CostCentres { get; set; }

        public bool IsSubAccount { get; set; }
    }
}