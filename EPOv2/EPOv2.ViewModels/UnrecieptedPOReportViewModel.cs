using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOv2.ViewModels
{
    public class UnrecieptedPOReportViewModel
    {
        public int SelectedItem { get; set; }
        public List<UnrecieptedPOItem>  Items { get; set; }
    }

    public class UnrecieptedPOItem
    {
        public int Id { get; set; }

        public int OrderNumber { get; set; }

        public string CostCentre { get; set; }

        public string SupplierName { get; set; }

        public double Total { get; set; }

        public string Author { get; set; }

        public string ReceiptGroup { get; set; }

        public List<string> Recievers { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime LastModified { get; set; }

        public string Approver { get; set; }

        public string Status { get; set; }


    }
}
