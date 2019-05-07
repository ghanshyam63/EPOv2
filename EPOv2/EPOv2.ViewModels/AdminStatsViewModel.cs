namespace EPOv2.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class AdminStatsViewModel
    {
        public int RaisedOrders { get; set; }

        public int ApptovedOrders { get; set; }

        public int MatchedOrders { get; set; }

        public int ClosedOrders { get; set; }

        public int ScannedVouchers { get; set; }

        public int AuthorisedVouchers { get; set; }

         [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double TotalOrdersAmount { get; set; }

         [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double AveOrdersAmount { get; set; }
         [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double OrdersPerDay { get; set; }
         [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public double OrdersPerWeek { get; set; }

        public string TopSpender { get; set; }

        public string TopApprover { get; set; }

        public string TopRaiser { get; set; }

        public string TopCostCentre { get; set; }

        public int TopSpenderQty { get; set; }

        public int TopRaiserQty { get; set; }

         [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double TopSpenderAmount { get; set; }

         [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
         public double TopRaiserAmount { get; set; }
    }
}