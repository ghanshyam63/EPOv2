namespace EPOv2.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class CapexDashboardViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Capex #")]
        public string CapexNumber { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        [Display(Name = "Total")]
        public double TotalExGST { get; set; }

        public string Entity { get; set; }

        public string CostCentre { get; set; }

        [Display(Name = "Type")]
        public string CapexType { get; set; }

        public string Owner { get; set; }

        [Display(Name = "Date")]
        public string DateCreated  { get; set; }

        public string Status { get; set; }

        public bool isEditLocked { get; set; }
        public bool isDeleteLocked { get; set; }

        public string URL { get; set; }
    }

   
}
