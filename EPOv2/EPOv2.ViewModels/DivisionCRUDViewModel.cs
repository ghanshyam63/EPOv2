namespace EPOv2.ViewModels
{
    using System.Collections.Generic;

    using EPOv2.ViewModels.Interfaces;

    // using EPOv2.Interfaces;

    public class DivisionCRUDViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CostCentreCodeRangeFrom { get; set; }
        public int CostCentreCodeRangeTo { get; set; }
        public List<IUserViewModel> Users { get; set; }

        public string SelectedUser { get; set; }
    }
}
