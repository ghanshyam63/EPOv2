namespace EPOv2.ViewModels
{
    using System.Collections.Generic;

    public class pdfPOContainerViewModel
    {
        public List<pdfPOPage> PdfPOPages { get; set; }
        public int PageQty { get; set; }
    }

    public class pdfPOPage
    {
        public int Number { get; set; }

        public int PageQty { get; set; }

        public pdfPOViewModel PdfPOViewModel { get; set; }

        public bool isLast { get; set; } // show or do not Totals

    }
}