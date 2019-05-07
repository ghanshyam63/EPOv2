namespace EPOv2.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using DomainModel.Entities;


    public class OrderItemViewModel
    {
        public int itemId { get; set; }
        public int Line { get; set; }

        public int RevisionQty { get; set; }

        public List<Account> Accounts { get; set; }

        public List<AccountViewModel> AccountViewModels { get; set; }

        public int SelectedAccount { get; set; }

        public List<Account> SubAccounts { get; set; }

        public List<AccountViewModel> SubAccountViewModels { get; set; } 

        public int? SelectedSubAccount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public string DueDate { get; set; }

        public int? SelectedCapexId { get; set; }
        public List<CapexViewModel> Capexes { get; set; } 

        public string CostCentre { get; set; }
        public string Status { get; set; }

        public string Description { get; set; }

        public double Qty { get; set; }

        public double UnitPrice { get; set; }

        public List<Currency> Currencies { get; set; }

        public int SelectedCurrency { get; set; }

        public double CurrencyRate { get; set; }
         [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public double ConvertedTotalExTax { get; set; } //in AUD
         [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public double ConvertedTotalTax { get; set; }

         [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public double ConvertedTotal { get; set; }

        public bool IsGSTInclusive { get; set; }
         [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public bool IsTaxable { get; set; }
        public bool IsGSTFree { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double TotalExTax { get; set; }
         [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double TotalTax { get; set; }
         [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }

        public int OrderId { get; set; }
        public int TaxPercent { get; set; }

        public bool isCapex { get; set; }

        public bool IsRevision { get; set; }

        public bool IsItemKit { get; set; }

        public bool IsRestrictedEdit { get; set; }
    }

    public class OrderItemTableViewModel
    {
        public int Id { get; set; }

        public int Line { get; set; }

        public int RevisionQty { get; set; }

        public string Description { get; set; }

        public double Qty { get; set; }

        public string AccountName { get; set; }

        public string SubAccountName { get; set; }

        public string DueDate { get; set; }

        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public bool isForeignCurrency { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double UnitPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double TotalExTax { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double TotalTax { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double ConvertedTotal { get; set; }
        public string ReceviedDate { get; set; } //date of current matching session

        public double QtyReceived { get; set; } //received on current matching session

        public double QtyAlreadyReceived { get; set; } //already received

        public string OrderNumber { get; set; }

        public string Status { get; set; }

        public string Author { get; set; }

        public bool IsRestrictedEdit { get; set; }
    }

    public class QADOrderItemTableViewModel
    {
        public int Id { get; set; }

        public int Line { get; set; }

        public string Description { get; set; }

       
        public double QtyOrdered { get; set; }

        public double QtyRecieved { get; set; }

      
        public double QtyInvoiced { get; set; }

      
        public double QtyOpen { get; set; }

        public string AccountName { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double UnitPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double UnitPriceInvoiced { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double UnitPriceVariance { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double POLineTotal { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double VarianceLineTotal { get; set; }
        
        public string OrderNumber { get; set; }

        public string Status { get; set; }

        public string Author { get; set; }
    }
    

   

    public class CapexViewModel
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string FullName { get; set; }
        public List<SearchCapexResult> CapexView { get; set; } //list for inhereting partial view from Search

        public List<Status> StatusList { get; set; }

        public int SelectedStatus { get; set; }
    }

    public class AccountViewModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string FullName { get; set; }
    }

    public class OrderItemKitViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CostCentreId { get; set; }

        public string CostCentre { get; set; }

        public int AccountId { get; set; }

        public string Account { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

    }

    public class OrderItemKitCRUDViewModel : OrderItemKitViewModel
    {
        public int SelectedCostCentre { get; set; }

        public List<CostCentreViewModel> CostCentres { get; set; }

        public int SelectedAccount { get; set; }

        public List<AccountViewModel> Accounts { get; set; }

    }

    public class ItemKitDdlViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}