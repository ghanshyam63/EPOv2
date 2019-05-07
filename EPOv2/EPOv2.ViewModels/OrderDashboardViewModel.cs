namespace EPOv2.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OrderDashboardViewModel
    {
        public int Id { get; set; }

        [Display(Name="Order #")]
        public string OrderNumber { get; set; }

        [Display(Name = "Temp #")]
        public string TempOrderNumber { get; set; }

        public string Date { get; set; }

        public string Supplier { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }

        public string Status { get; set; }
        public bool isEditLocked { get; set; }

        public bool isDeleteLocked { get; set; }
        public bool isPDFLocked { get; set; }

        public bool isReadyForClose { get; set; }

        public bool isReadyForMatch { get; set; }
    }

    public class DashboardViewModel
    {
        public int SelectedItem { get; set; }
    }

    public class DashboardIncoiceViewModel
    {
        public int Id { get; set; }

         [Display(Name = "Voucher #")]
        public int VoucherNumber { get; set; }

         [Display(Name = "Invoice #")]
        public string InvoiceNumber { get; set; }

        [Display(Name = "Attached")]
        public DateTime AttacheDateTime { get; set; }

        public string Supplier { get; set; }

        
    }
}