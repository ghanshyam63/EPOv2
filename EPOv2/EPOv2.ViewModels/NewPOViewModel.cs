namespace EPOv2.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    using DomainModel.DataContext;
    using DomainModel.Entities;


    public class NewPOViewModel
    {
        public int Id { get; set; }

        public string Author { get; set; }

        public string OrderNumber { get; set; }

        public string TempOrderNumber { get; set; }

        public int RevisionQty { get; set; }

        public string OrderDate { get; set; }

        public string StatusName { get; set; }

        public CompanyBoxViewModel CompanyBox { get; set; }

        public DeliveryBoxViewModel DeliveryBox { get; set; }

        public SupplierBoxViewModel SupplierBox { get; set; }

        public DefaultOrderSettingsViewModel DefaultOrderSettingsViewModel { get; set; }

        public List<OrderItemTableViewModel> Items { get; set; }
        public List<OrderItemTableViewModel> MatchingItems { get; set; }

        public int EntityId { get; set; }

        public int CostCenterId { get; set; }
  
        public int GroupId { get; set; }

        public int SupplierId { get; set; }

        public int DeliveryAddressId { get; set; }

        public string UserId { get; set; }

        public int CapexId { get; set; }

        public string Transmission { get; set; }

        public string Comment { get; set; }

        public string InternalComment { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double TotalExGST { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double TotalGST { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double TotalOrder { get; set; }
        public int EditingItemId { get; set; }
        public int DeletingItemId { get; set; }

        public string SupplierEmail { get; set; }

        public bool IsLocked  { get; set; }

        public bool IsRestrictedEdit { get; set; }

        public bool IsMatching { get; set; }

        public bool isRevision { get; set; }

        public bool IsChangedByRevision { get; set; }


        public bool IsForeignCurrency { get; set; }

        public string CurrencyName { get; set; }

        public string CurrencySign { get; set; }

    }
    public class CompanyBoxViewModel
    {
        public List<Entity> Entities { get; set; }

        public int SelectedEntity { get; set; }

        public string ACN { get; set; }

        public string ABN { get; set; }
        public List<CostCentreViewModel> CostCentres { get; set; }

        public List<CapexViewModel> Capexes { get; set; }
        public int? SelectedCapexId { get; set; }
        public int SelectedCostCenter { get; set; }

        public List<Group> ReceiptGroups { get; set; }

        public int SelectedReceiptGroup { get; set; }

        public bool IsBlocked { get; set; }
        public bool isCapex { get; set; }

    }

    public class DeliveryBoxViewModel
    {
        public List<DeliveryAddress> DeliveryAddresses { get; set; }

        public int SelectedDeliveryAddress { get; set; }

        public List<User> Users { get; set; } //Attention

        public List<UserViewModel> UserViewModels { get; set; }//Attention
        
        public string SelectedUser { get; set; }
        public string CurrentUserId { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostCode { get; set; }
        public bool IsBlocked { get; set; }

    }

    public class SupplierBoxViewModel
    {
        public List<tblSupplier> Suppliers { get; set; }
        public List<SupplierViewModel> SupplierViewModels { get; set; }

        public int SelectedSupplier { get; set; }

        public List<SelectListItem> TransmissionMethod { get; set; }
        public string SelectedTransmission { get; set; }

        public string Contact { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostCode { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string EmailForSupplier { get; set; }
        public bool IsBlocked { get; set; }


    }

    public class DefaultOrderSettingsViewModel
    {
        public int DefaultEntityId { get; set; }

        public int DefaultCostCentreId { get; set; }

        public int DefaultGroupId { get; set; }

        public int DefaultDeliveryAddressId { get; set; }

        public int DefaultSupplierId { get; set; }
    }
    
}