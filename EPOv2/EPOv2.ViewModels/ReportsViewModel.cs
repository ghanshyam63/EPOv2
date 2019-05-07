namespace EPOv2.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ReportsViewModel
    {
    }

    public class EPOAccuralReportViewModel
    {
        public int OrderId { get; set; }

        public int OrderNumber { get; set; }

        public string EntityName { get; set; }

        public int CostCentreCode { get; set; }

        public string AccountCode { get; set; }

        public string SupplierName { get; set; }

        public double OrderQty { get; set; }

        public double ReceiptQty { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double TotalExGST { get; set; }

        public string Status { get; set; }

        public double AwaitingQty { get; set; }
        public string CurrencySym { get; set; }
        public double CurrencyRate { get; set; }


        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double AwaitingTotalExGST { get; set; }
    }

    public class OutstandingInvoicesReport
    {
        public string Receiver { get; set; }
        public string Subject { get; set; }

        public List<InvoiceReportViewModel> Invoices { get; set; } 
        
    }

    public class InvoiceReportViewModel
    {
        public int VoucherId { get; set; }
        public int OrderId { get; set; }
        public int VoucherNumber { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Date { get; set; }
        public string OrderNumber { get; set; }
        public string OrderStatus { get; set; }
        public string OrderAuthor { get; set; }
        public string OrderAuthoriser { get; set; }
        public string AuthrisationURL { get; set; }
        
    }

    
}