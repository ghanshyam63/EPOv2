namespace EPOv2.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    using EPOv2.ViewModels.Interfaces;

    public class CapexCRUDViewModel
    {
        public int Id { get; set; }
        public string CapexNumber { get; set; }

        public int RevisionQty { get; set; }

        public string Author { get; set; }

        public int AuthorEmpId { get; set; }
        
        public string DateCreated { get; set; }

        public string Status { get; set; }

        public bool IsLocked { get; set; }

        public bool IsEdit { get; set; }

        public CapexCompanyBox CapexCompanyBox { get; set; }

        //public CapexDetailBox CapexDetailBox { get; set; }

        public int EntityId { get; set; }

        public int CostCentreId { get; set; }

        public string OwnerId { get; set; }

        public string CapexType { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public HttpPostedFileBase SelectedFilePath { get; set; }
        public string Reference { get; set; }// file path

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double TotalExGST { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double TotalGST { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }

        public string Comment { get; set; }

        public bool IsDeleted { get; set; }

    }

    public class CapexCompanyBox
    {
        public int SelectedEntity { get; set; }

        public List<EntityViewModel> Entities { get; set; }

        public int SelectedCostCentre { get; set; }

        public List<CostCentreViewModel> CostCentres { get; set; }

        public string SelectedOwner { get; set; }

        public List<IUserViewModel> Users { get; set; }

        public string SelectedCapexType { get; set; }

        public List<string> CapexTypes { get; set; }

        public bool IsLocked { get; set; }
        public bool IsEdit { get; set; }
    }

    public class CapexDetailBox
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public HttpPostedFileBase SelectedFilePath { get; set; }
        public string Reference { get; set; }// file path

        public double TotalExGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double TotalGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public bool IsLocked { get; set; }

    }
}
