namespace EPOv2.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using DomainModel.Entities;
    using System;

    public class SearchViewModel
    {
        //share fields
        public int Id { get; set; }

        public bool isCapex { get; set; }

        public bool isVoucher { get; set; }

        public bool isPO { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public string DateFrom { get; set; } //Date Created

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public string DateTo { get; set; }//Date Created

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public string DateRecFrom { get; set; } //Date Receipt

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public string DateRecTo { get; set; }//Date Receipt

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public string DateDueFrom { get; set; } //Due Date

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public string DateDueTo { get; set; }//Due Date

        public int SelectedStatus { get; set; }

        public List<SupplierViewModel> Suppliers { get; set; }

        public int SelectedSupplier { get; set; }

        public string SelectedAuthoriser { get; set; }

        public List<UserViewModel> Authorisers { get; set; }//Authoriser

        public int SelectedItem { get; set; }

        public string SelectedAuthor { get; set; }

        public List<UserViewModel> Authors { get; set; }//Authors

        //PO fields
        public string OrderNumber { get; set; }

        public List<Entity> Entities { get; set; }

        public int SelectedEntity { get; set; }

        public List<CostCentreViewModel> CostCentres { get; set; }

        public int SelectedCostCenter { get; set; }

        public List<AccountViewModel> Accounts { get; set; }

        public int SelectedAccount { get; set; }

        public string Details { get; set; }

        public List<Status> Statuses { get; set; }

        public List<CapexViewModel> Capexes { get; set; }
        public int SelectedCapexId { get; set; }

        //Voucher fields
        public string VoucherNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public List<VoucherStatus> VoucherStatuses { get; set; }

        //Capex fields
        public string CapexNumber { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string SelectedOwner { get; set; }

        public List<UserViewModel> Owners { get; set; }//Owner
    }

    public class SearchEPOResult
    {
        public List<SearchEPOResultItem> SearchEpoResultItems { get; set; }

        public int OrderCount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }

        public List<string> UserRoles { get; set; } 
    }


    public class SearchEPOResultItem
    {
        public string Id { get; set; }

        public string OrderNumber { get; set; }

        public int OrderNumberInt { get; set; }
        public string TempNumber { get; set; }

        public DateTime Date { get; set; }

        public string Entity { get; set; }

        public string CostCentre { get; set; }

        public string Supplier { get; set; }

        public string Status { get; set; }

        public string Author { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }

        public bool isEditLocked { get; set; }


    }

    public class SearchVoucherResult
    {
        public string Id { get; set; }

        public string VoucherNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public string Date { get; set; }

        public string Supplier { get; set; }

        public string Authoriser { get; set; }

        public string AuthorisationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }
        public string Status { get; set; }

        public bool isEditLocked { get; set; }

        public string Comment { get; set; }
    }

    public class SearchCapexResult
    {
        public string Id { get; set; }

        public string CapexNumber { get; set; }

        public string Title { get; set; }
        public string Entity { get; set; }

        public string CostCentre { get; set; }

        public string Date { get; set; }

        public string Owner { get; set; }
        public string Author { get; set; }

        public string AuthorisationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }
        public string Status { get; set; }

        public bool isEditLocked { get; set; }
    }
}