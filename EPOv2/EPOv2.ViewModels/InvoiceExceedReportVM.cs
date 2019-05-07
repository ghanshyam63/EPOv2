using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOv2.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class InvoiceExceedReportVM
    {
        public List<InvoiceExceedReportItemVM> Items { get; set; }

        public List<InvoiceExceedReportItemVM> ItemsWithoutOrder { get; set; }
    }

    public class InvoiceExceedReportItemVM
    {
        public int OrderId { get; set; }

        public int OrderNumber { get; set; }

        public int CostCentreCode { get; set; }

        public string CostCentreFullName { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierFullName { get; set; }

        public string AuthoriserName { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double OrderTotal { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double InvoicesTotal { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Variance { get; set; }
    }

    public class InvoiceExceedReportFilterVM
    {
        public bool FullReport { get; set; }

        public bool CurrentFinancialYear { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public string dateFrom { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public string dateTo { get; set; }
    }
}
