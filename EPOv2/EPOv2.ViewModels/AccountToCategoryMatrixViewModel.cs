using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOv2.ViewModels
{
    using DomainModel.Entities;

    public class AccountToCategoryMatrixViewModel
    {
        public int Id { get; set; }
        public int AccountId { get; set; }

        public int CategoryId { get; set; }

        public string AccountCode { get; set; }

        public string AccountName { get; set; }

        public string AccountFullName { get; set; }

        public string CategoryName { get; set; }

        public int SelectedAccount { get; set; }

        public int SelectedCategory { get; set; }
    }

    public class AccountCategoryDLLViewModel
    {
        public int SelectedCategory { get; set; }

        public List<AccountCategory> AccountCategories { get; set; }
    }

    public class AccountToCategoryAdding
    {
        public int SelectedCategory { get; set; }

        public AccountCategoryDLLViewModel AccountCategories { get; set; }

        public List<string> SelectedAccounts { get; set; }

        public List<AccountViewModel> Accounts { get; set; }
    }
}
