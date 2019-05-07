namespace EPOv2.ViewModels
{
    using System.Collections.Generic;

    using EPOv2.ViewModels.Interfaces;

    public class CapexApproverCRUDViewModel
    {
        public int Id { get; set; }

        public string Role { get; set; }

        public int Level { get; set; }

        public double Limit { get; set; }

        public List<IUserViewModel> Users { get; set; }

        public string SelectedUser { get; set; }

        public List<DivsionViewModel> Divisions { get; set; }

        public int? SelectedDivision { get; set; }
    }
}
