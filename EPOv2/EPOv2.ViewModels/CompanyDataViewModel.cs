namespace EPOv2.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public class CompanyDataViewModel
    {
    }

    public class EntityViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DisplayName("Number")]
        public int CodeNumber { get; set; }

        public string Code { get; set; }

        public string Prefix { get; set; }

        public string ACN { get; set; }

        public string ABN { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        [DisplayName("Deleted")]
        public bool isDelete { get; set; }

        [DisplayName("Modified Date")]
        public string LastModifiedDate { get; set; }

        [DisplayName("Modified By")]
        public string LastModifiedBy { get; set; }

    }

    public class EntityDDLViewModel
    {
        public int SelectedEntity { get; set; }

        public List<EntityViewModel> Entities { get; set; }
    }

    public class CostCentreToEntityViewModel
    {
        public int Id { get; set; }
        public string Entity { get; set; }
        public string CostCentre { get; set; }
        public string LastModifiedDate { get; set; }

        public bool isDeleted { get; set; }
    }

    public class CostCentreToEntityAdding
    {
        public int SelectedEntity { get; set; } //TODO: spelling mistake

       public List<string> SelectedCostCentres { get; set; }

        public List<CostCentreViewModel> CostCentres { get; set; }
    }

    public class AccountToCostCentreViewModel
    {
        public int Id { get; set; }
        public string Account { get; set; }

        public string AccountType { get; set; }
        public string CostCentre { get; set; }
        public string LastModifiedDate { get; set; }

        public bool isDeleted { get; set; }
    }

    public class AccountToCostCentreAdding
    {
        public int SelectedCostCentre { get; set; }

        public CostCentreDDLViewModel CostCentres { get; set; }

        public List<string> SelectedAccounts { get; set; }

        public List<AccountViewModel> Accounts { get; set; }
    }
}