namespace EPOv2.Business.Interfaces
{
    using System.Web.Mvc;

    using DomainModel.Entities;

    using EPOv2.Repositories.Interfaces;
    using EPOv2.ViewModels;

    public interface IOutput
    {
        Order Order { get; set; }

        int OrderId { get; set; }

        void SendReport();

        pdfPOContainerViewModel PdfPOContainer { get; set; }

        void CreatePDFContainer();

        pdfPOPage CreatepdfPOPage(int pageNumber, int pageQty);

        pdfPOViewModel GeneratePDFPO(int itemStart=0);

        void SendEmail(string filePath);

        string CreatePDFFile(pdfPOContainerViewModel model, ControllerContext context);

        byte[] GeneratePOPDFFile(string fileName, pdfPOContainerViewModel model, ControllerContext context);

        void SaveInvoiceFile(VoucherAttachingFormViewModel model, VoucherDocument voucherDoc, string fileName, bool isGRNI=false);

        void SetCancelledStampOnPO(string filePath);

        void SendEmailWithCancelledOrder(string filePath);

        void SendEmailForEttacher(string bodyText, string subjectText);

        void AuthoriseInvoiceFile(VoucherDocument voucherDoc);

        void Dispose();

        void MakeWatermarkOnPdf();

        void SaveInvoiceFilettTest();

        void DeleteUsedInvoice(string file);

        void SendReport(string receiver, string bodyText, string subject);

        void SendCapexNotification(User user, string body, string subject);
    }
}