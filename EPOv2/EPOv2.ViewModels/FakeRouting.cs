namespace EPOv2.ViewModels
{
    using System.Collections.Generic;

    using DomainModel.DataContext;

    public class FakeRouting
    {
        public List<tblEntity> Entities { get; set; }

        public List<tblCostCentre> CostCentres { get; set; }

        public List<tblAccount> Accounts { get; set; }

        public List<string> Authorisers { get; set; }

        public int SelectedEntity { get; set; }

        public int SelectedCostCentre { get; set; }

        public List<int> SelectedAccountsList { get; set; }

        public string SelectedAuthoriser { get; set; } 


    }
}