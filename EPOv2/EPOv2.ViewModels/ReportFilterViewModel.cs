namespace EPOv2.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ReportEPOAccuralsFilterViewModel
    {
        public string SelectedFY { get; set; }

        public List<string> FinancialYearList { get; set; }

        public string SelectedFinancialPeriod { get; set; }

        public List<string> FinancialPeriodList { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public string ReceiptDate { get; set; }
    }
}