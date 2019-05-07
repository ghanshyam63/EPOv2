using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOv2.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    using DomainModel.Entities;

    public class BudgetViewModel
    {

    }

    public class BudgetReportEntryDataViewModel
    {
        public int SelectedEntity { get; set; }

        public List<Entity> Entities { get; set; }

        public List<CostCentreViewModel> CostCentres { get; set; }

        public int SelectedCostCenter { get; set; }

        public int SelectedCategory { get; set; }

        public List<AccountCategory> AccountCategories { get; set; }
    }

    public class BudgetReportResult
    {
        public List<BudgetReportItem> EstimateEPOSpend { get; set; }

        public List<BudgetReportItem> Reforecast { get; set; }

        public List<BudgetReportItem> Variance { get; set; }

        public List<BudgetReportItem> EPOApprovedNotReceipted { get; set; }

        public bool isError { get; set; }

        public BudgetReportEntryDataViewModel Filter { get; set; }
    }

   

    public class BudgetReportInputViewModel
    {
        public List<BudgetReportItem> EstimateEPOSpend { get; set; }

        public BudgetReportEntryDataViewModel Filter { get; set; }
    }

    public class BudgetReportItem
    {
        public string GLCode { get; set; }

        public string Category { get; set; }

        public string AccountName { get; set; }
        
        public double[] Period { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0;(#,##0)}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }
    }

    public class BudgetWithVoucherReportResult
    {
        public List<BudgetWithVoucherReportItem> EstimateEPOSpend { get; set; }

        public List<BudgetWithVoucherReportItem> Reforecast { get; set; }

        public List<BudgetWithVoucherReportItem> Variance { get; set; }

        public List<BudgetWithVoucherReportItem> Vouchers { get; set; }

        public bool isError { get; set; }

    }

    public class BudgetWithVoucherReportItem
    {
        //public string GLCode { get; set; }

        public string Category { get; set; }

        public int CategoryId { get; set; }

        //public string AccountName { get; set; }

        public double[] Period { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,##0;(#,##0)}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }
    }
}
