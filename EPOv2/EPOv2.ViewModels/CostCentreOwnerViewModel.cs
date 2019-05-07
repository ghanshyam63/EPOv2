namespace EPOv2.ViewModels
{
    using System.Collections.Generic;

    public class CostCentreOwnerViewModel
    {
        public string SelectedCostCenterId { get; set; }

        public List<CostCentreViewModel> CostCentres { get; set; }

        public string SelectedOwnerId { get; set; }

        public List<UserViewModel> Owners { get; set; }

    }
}