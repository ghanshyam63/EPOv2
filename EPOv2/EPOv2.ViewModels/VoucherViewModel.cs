namespace EPOv2.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Net.Http;

    using DomainModel.Entities;


    public class VoucherViewModel
    {
        public int Id { get; set; }

        public string VoucherNumber { get; set; }

        

    }

    public class VoucherInfoViewModel
    {
        public string InvoiceNumber { get; set; }

        public string VoucherNumber { get; set; }

        public string CostCentre { get; set; }

        public string Account { get; set; }

        public string Terms { get; set; }

        public string Supplier { get; set; }

        public string DueDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Amount { get; set; }

        public string Status { get; set; }

        public bool AutoApprove { get; set; }
    }

    public class VoucherGRNIInfoViewModel
    {
        public List<VoucherPOItemViewModel> VoucherPOs { get; set; }

        public List<VoucherPOItemDetailViewModel> VoucherPOItemDetails { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double TotalPO { get; set; }

    }

    public class VoucherPOItemViewModel
    {
        public string Voucher { get; set; }

        public string PO { get; set; }

        public string RA { get; set; }

         [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }
    }

    public class VoucherPOItemDetailViewModel
    {
        public string Site { get; set; }

        public int? Line { get; set; }

        public string Part { get; set; }

       
        public double? RecievedQty { get; set; }

      
        public double? InvoicedQty { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double? UnitCost { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double? UnitGST { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }
        
        
    }

    public class VoucherPanelViewModel
    {
        public VoucherSearchViewModel VoucherSearch { get; set; }
        public bool isGRNI { get; set; }

        
    }

    public class VoucherSearchViewModel<T>
    {
        public string VoucherNumber { get; set; }

        public List<SupplierViewModel<T>> Suppliers { get; set; }

        public int SelectedSupplierId { get; set; }

        public List<VoucherItemViewModel> Vouchers { get; set; }

        public int SelectedVoucherId { get; set; }

        public bool IncludeConfirmed { get; set; }
    }

    public class VoucherSearchViewModel : VoucherSearchViewModel<string> { }

    public class VoucherItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class VoucherFileInvoiceViewModel
    {
        public string Id { get; set; } //so far same as Name

        public string Name { get; set; }
    }

    public class VoucherAttachmentPanelViewModel
    {
        public string Filter { get; set; }

        public bool loadEPO { get; set; }

        public string SelectedFile { get; set; }

        public List<VoucherFileInvoiceViewModel> InvoiceList { get; set; }
    }

    public class VoucherRelatedDocumentsPanel
    {
        public int VoucherDocumentId { get; set; }
        public List<VoucherRelatedDocumentViewModel> RelatedDocuments { get; set; }
    }

    public class VoucherAttachingFormViewModel
    {
        public int VoucherNumber { get; set; }

        public string InvoiceNumber { get; set; }


        public string SelectedAuthoriser { get; set; }

        public List<UserViewModel> AuthoriserList { get; set; }

        public int SelectedDocumentTypeId { get; set; }

        public List<VoucherDocumentType> VoucherDocumentTypes { get; set; }

        public bool isEPO { get; set; }

        [Range(typeof(int), "1", "10")]
        public int PageFrom { get; set; }
        [Range(typeof(int), "1", "10")]
        public int PageTo { get; set; }

        public string Comment { get; set; }

        public bool ReplaceExistingFile { get; set; }

        public bool IsAuthorised { get; set; }

        public string Authoriser { get; set; }

        public string SupplierCode { get; set; }
        public string AccountCode { get; set; }

        public int MaxPagesInFile { get; set; }

        public string SelectedDocument { get; set; }

        public double Amount { get; set; }
        public int CostCentreCode { get; set; }
        public string DueDate { get; set; }
        public string Terms { get; set; }
        
    }

    public class ReturnResutViewModel
    {
        public string Status { get; set; }

        public string Message { get; set; }
    }

    public class VoucherRelatedDocumentViewModel
    {
        public string Type { get; set; }

        public string Author { get; set; }

        public string Number { get; set; }

        public string Authoriser { get; set; }

        public string Date { get; set; }

        public int DocumentId { get; set; }

        public string Reference { get; set; }//Link to File or EPO
        public bool IsMatchable { get; set; }

    }

    public class InvoiceViewModel
    {

    }

    public class InvoicePageViewModel
    {
        public int VoucherDocumentId { get; set; }

        public string VoucherDocumentType { get; set; }

        public VoucherInfoViewModel VoucherInfo { get; set; }

        public List<VoucherDetail> VoucherDetails { get; set; }

        public List<OrderItemTableViewModel> OrderItems { get; set; }

        public List<QADOrderItemTableViewModel> QadOrderItems { get; set; }

        public List<VoucherRelatedDocumentViewModel> RelatedDocuments { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double InvoiceTotal { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double OrderTotal { get; set; }
         [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double RelatedInvoiceTotal { get; set; }

        public string SelectedStatus { get; set; }

        public string Comment { get; set; }

        public string DisplayComment { get; set; }

        public string InvoiceNumber { get; set; }

        public string SupplierName { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Variance { get; set; }

        public bool isGRNI { get; set; }

        public bool isAuthorisable { get; set; }

        public bool IsVarianceNegative { get; set; }

    }

    public class ResubmitVoucherForm
    {
        public int VoucherId { get; set; }
        public int VoucherNumber { get; set; }

        public string SelectedAuthoriser { get; set; }

        public List<UserViewModel> AuthoriserList { get; set; }

        public string Comment { get; set; }

        public string DisplayComment { get; set; }
    }

    public class DeleteVoucherForm
    {
        public int VoucherId { get; set; }
        public int VoucherNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public string Supplier { get; set; }

        public double Amount { get; set; }

        public string Comment { get; set; }
    }

    public class ChangeVoucherStatusForm
    {
        public int VoucherId { get; set; }
        public int VoucherNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public string Supplier { get; set; }

        public double Amount { get; set; }

        public string Comment { get; set; }

        public List<VoucherStatus> StatusList { get; set; }

        public int SelectedStatus { get; set; }

        public List<SearchVoucherResult> VoucherList { get; set; }


    }

    public class VoucherDetail
    {
        public int Line { get; set; }

        public string Entity { get; set; }

        public string CostCentre { get; set; }

        public string AccountCode { get; set; }

        public string SubAccount { get; set; }

        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double  Amount { get; set; }
    }
}