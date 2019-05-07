namespace EPOv2.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class pdfPOViewModel
    {
        public int Id { get; set; }

        public string PONumber { get; set; }

        public string PODate { get; set; }

        public string LogoPath { get; set; }
        public string Status { get; set; }

        public int RevisionQty { get; set; }

        public int PageQty { get; set; }

        public int PageNo { get; set; }

        public string Comment { get; set; }
         [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double TotalExGST { get; set; }
         [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double TotalGST { get; set; }
         [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double TotalOrder { get; set; }

        public string AuthoriserName { get; set; }

        public List<OrderItemTableViewModel> OrderItems { get; set; }

        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public string EntityABN { get; set; }

        public string EntityPhone { get; set; }

        public string EntityFax { get; set; }

        public string EntityEmail { get; set; }

        public int SupplierId { get; set; }

        public string SupplierName  { get; set; }

        public string SupplierAddress { get; set; }

        public string SupplierCity { get; set; }

        public string SupplierState { get; set; }

        public string SupplierPostCode{get;set;}

        public string SupplierContact { get; set; }

        public string SupplierPhone { get; set; }

        public string SupplierFax { get; set; }

        public string SupplierEmail { get; set; }

        public string DeliveryName { get; set; }

        public string DeliveryAddress { get; set; }

        public string DeliveryCity { get; set; }

        public string DeliveryState { get; set; }

        public int DeliveryPostCode { get; set; }

        public string Attention { get; set; }

        public string AttentionPhone { get; set; }

        public string AttentionEmail { get; set; }

        public bool IsForeignCurrency { get; set; }

        public string CurrencyName { get; set; }

        public string CurrencySign { get; set; }

    }
}