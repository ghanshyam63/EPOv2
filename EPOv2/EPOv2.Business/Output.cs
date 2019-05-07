namespace EPOv2.Business
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Web.Mvc;

    using DomainModel.Entities;

    //using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.ViewModels;
    using PreMailer.Net;
    using iTextSharp.text;
    using iTextSharp.text.pdf;

    using Rotativa;
    using Rotativa.Options;

    using Serilog;

    public partial class Output : IOutput
    {
        private readonly ILogger _logger = Log.Logger;
        private const string MailFrom = "epo@oneharvest.com.au";
        private const int MaxItemPerPage = 16;
        //TODO: move paths to settings depends of enviroment
        //public const string fileDirectory = "E:\\";
        public const string fileDirectory = "E:\\WWWApps\\EPOv2\\ElectronicPO\\";
        //public const string fileDirectory = "E:\\WWWApps\\EPOv2-dev\\ElectronicPO\\";

        private readonly Dictionary<int, string> logosDictionary = new Dictionary<int, string>()
                                                                       {
                                                                           {
                                                                               2,"~/Content/logos/OneHarvest.jpg"
                                                                           },
                                                                           {
                                                                               3,"~/Content/logos/vegco.jpg"
                                                                           },
                                                                           {
                                                                               4,"~/Content/logos/HarvestFreshCuts.jpg"
                                                                           },
                                                                           {
                                                                               6,"~/Content/logos/HarvestFreshCuts.jpg"
                                                                           },
                                                                           {
                                                                               7,"~/Content/logos/hff.jpg"
                                                                           },
                                                                           {
                                                                               13,"~/Content/logos/_blank.jpg"
                                                                           }
                                                                       };

        private readonly IAd _ad;
        private readonly IData _data;
        public Order Order { get; set; }

        private int _orderId;

        public int OrderId
        {
            get
            {
                return _orderId;
            } 
            set
            {
                _orderId = value;
               this.Order = this._data.GetOrder(value);
            }
        }


        public pdfPOContainerViewModel PdfPOContainer { get; set; }

        public Output(IData data, IAd ad)
        {
            this._data = data;
            this._ad = ad;
        }

        public void CreatePDFContainer()
        {
            this.PdfPOContainer = new pdfPOContainerViewModel();
            int pageQty = this.Order.OrderItems.Count / MaxItemPerPage + 1;
            this.PdfPOContainer.PageQty = pageQty;
            this.PdfPOContainer.PdfPOPages= new List<pdfPOPage>();
            for (var i=1; i<=pageQty;i++)
            {
                this.PdfPOContainer.PdfPOPages.Add(this.CreatepdfPOPage(i,pageQty));
            }
        }

        public pdfPOPage CreatepdfPOPage(int pageNumber, int pageQty)
        {
            var last = pageNumber == pageQty ? true : false;
            var pdfPOpage = new pdfPOPage()
                                {
                                    PdfPOViewModel = this.GeneratePDFPO(MaxItemPerPage*(pageNumber-1)),
                                    PageQty = pageQty,
                                    Number = pageNumber,
                                    isLast = last
                                };
            return pdfPOpage;
        }
        public pdfPOViewModel GeneratePDFPO(int itemStart=0)
        {
            var authoriser =_data.GetAuthoriser(this.Order.Id);
            var supplier = _data.GetSupplier(this.Order.SupplierId);
            var model = new pdfPOViewModel()
                            {
                                Id = this.Order.Id,
                                PONumber = this.Order.OrderNumber.ToString(),
                                PODate = this.Order.OrderDate.ToShortDateString(),
                                LogoPath = this.logosDictionary[this.Order.Entity.CodeNumber],
                                Comment = this.Order.Comment,
                                TotalExGST = this.Order.TotalExGST,
                                TotalGST = this.Order.TotalGST,
                                TotalOrder = this.Order.Total,
                                PageNo = 1,
                                PageQty = 1,
                                RevisionQty = this.Order.RevisionQty,
                                Status = this.Order.Status.Name,
                                AuthoriserName = authoriser,
                                
                                EntityABN = this.Order.Entity.ABN,
                                EntityEmail = this.Order.Entity.Email,
                                EntityName = this.Order.Entity.Name,
                                EntityFax = this.Order.Entity.Fax,
                                EntityPhone = this.Order.Entity.Phone,
                                EntityId = this.Order.Entity.Id,
                                
                                Attention =
                                    this.Order.User.UserInfo.FirstName + " " + this.Order.User.UserInfo.LastName,
                                AttentionEmail = this.Order.User.Email,
                                AttentionPhone = this.Order.User.UserInfo.PhoneWork,
                                
                               
                                
                                SupplierName = supplier.SupplierName,
                                SupplierAddress = supplier.Address1,
                                SupplierCity = supplier.City,
                                SupplierContact = supplier.Contact,
                                SupplierEmail = this.Order.SupplierEmail,
                                SupplierFax = supplier.Fax,
                                SupplierId = this.Order.SupplierId,
                                SupplierPhone = supplier.Phone,
                                SupplierPostCode = supplier.PostCode,
                                SupplierState = supplier.State,
                                OrderItems = new List<OrderItemTableViewModel>()
                            };
            

            if (this.Order.DeliveryAddress != null)
            {
                model.DeliveryName = this.Order.DeliveryAddress.Name;
                model.DeliveryAddress = this.Order.DeliveryAddress.Address;
                model.DeliveryCity = this.Order.DeliveryAddress.City;
                model.DeliveryPostCode = this.Order.DeliveryAddress.PostCode;
                model.DeliveryState = this.Order.DeliveryAddress.State != null
                                          ? this.Order.DeliveryAddress.State.ShortName
                                          : string.Empty;
            }
             
            foreach (var oi in this.Order.OrderItems.Where(x=>x.LineNumber>=itemStart && x.LineNumber<itemStart+MaxItemPerPage))
            {
                var item = new OrderItemTableViewModel()
                               {
                                   Line = oi.LineNumber,
                                   RevisionQty = oi.RevisionQty,
                                   Qty = oi.Qty,
                                   AccountName = oi.Account.Code + " " + oi.Account.Name,
                                   Description = oi.Description,
                                   Total = oi.Total,
                                   TotalExTax = oi.TotalExTax,
                                   TotalTax = oi.TotalTax,
                                   DueDate = oi.DueDate.ToShortDateString(),
                                   UnitPrice = oi.UnitPrice,
                                   isForeignCurrency = oi.Currency.Id != 1,
                                   CurrencyName = oi.Currency.Name,
                                   ConvertedTotal = oi.CurrencyRate != 0 ? oi.Total * oi.CurrencyRate : oi.Total
                               };
                model.OrderItems.Add(item);
            }
            model.IsForeignCurrency = model.OrderItems[0].isForeignCurrency;
            model.CurrencyName = this.Order.OrderItems[0].Currency.Name;
            model.CurrencySign = model.CurrencyName.Substring(3, 1);

            return model;
        }


        public static string RemoveSpecialCharacters(string str)
        {
           StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '-' || c == '_' || c == '@')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string RemoveSpecialCharactersFromFileName(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || c == '-' || c == '.')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public void SendEmail(string filePath)
        {
            //TODO change magic string to const
            var mail = new MailMessage();
            mail.From = new MailAddress(MailFrom);
            if (!String.IsNullOrEmpty(this.Order.SupplierEmail))
            {
                mail.To.Add(new MailAddress(RemoveSpecialCharacters(this.Order.SupplierEmail))); 
            }
            mail.To.Add(new MailAddress(this.Order.Author.Email));
            //mail.Bcc.Add(new MailAddress("aleksei.kogtev@oneharvest.com.au"));
            if (this.Order.Author.Email != this.Order.User.Email) mail.To.Add(new MailAddress(this.Order.User.Email));
            var client = new SmtpClient();
            mail.Subject = "Electronic Purchase Order #"+this.Order.OrderNumber;
            mail.Attachments.Add(new Attachment(filePath));
            mail.Body = "Please have a look at the attached PO.";
            client.Send(mail);
            mail.Dispose();
        }

        public string CreatePDFFile(pdfPOContainerViewModel model, ControllerContext context)
        {
            var fileName = "PO" + model.PdfPOPages[0].PdfPOViewModel.PONumber + ".pdf";
            var filePath = Path.Combine(fileDirectory, fileName);
            var isFileExist = File.Exists(filePath);
            //if (!isFileExist)
            //{
              this.GeneratePOPDFFile(fileName, model, context);
           // }
            return filePath;
        }

        public byte[] GeneratePOPDFFile(string fileName, pdfPOContainerViewModel model, ControllerContext context)
        {
            var filePath = Path.Combine(fileDirectory, fileName);
            var res = new ViewAsPdf("PdfContainer", model)
            {
                FileName = fileName,
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
                PageMargins = { Left = 3, Right = 3, Bottom = 2, Top = 0 },
                SaveOnServerPath = filePath,
            };
            var binary = res.BuildPdf(context);
            return binary;
        }


        public void SaveInvoiceFile(VoucherAttachingFormViewModel model, VoucherDocument voucherDoc, string fileName, bool isGRNI=false)
        {
            if (!Directory.Exists(Main.WarehousePath + model.SupplierCode.ToUpper()))
            {
                Directory.CreateDirectory(Main.WarehousePath + model.SupplierCode.ToUpper());
            }
          
            var stream = new MemoryStream();
            var pdfReader = new PdfReader(Main.UnlinkedPath + model.SelectedDocument);
            PdfReader.unethicalreading = true; //For secured files
            
            using (var document = new Document())
            {

                using (var copy = new PdfCopy(document, stream))
                {
                    document.Open();
                    
                    

                        for (var pageIndex = model.PageFrom; pageIndex <= model.PageTo; pageIndex++)
                        {
                            var page = copy.GetImportedPage(pdfReader, pageIndex);
                            copy.AddPage(page);
                        }
                        pdfReader.Close();
                        document.Close();
                        File.WriteAllBytes(Main.WarehousePath + model.SupplierCode + "\\" + fileName, stream.ToArray());
                    
                }
                document.Close();
            }

            if (model.IsAuthorised)
            {
                try
                {
                    var watermarkText = "Authorised by " + voucherDoc.Authoriser.UserInfo.FirstName + " "
                                        + voucherDoc.Authoriser.UserInfo.LastName + "  On " + voucherDoc.AuthorisedDate.Value.ToShortDateString();
                    SetAuthorisedWatermarkOnInvoice(Main.WarehousePath + model.SupplierCode + "\\" + fileName, watermarkText);
                }
                catch (Exception e)
                {
                    _logger.Fatal(e, "SaveInvoiceFile.WaterMark. Filename:{filename}, VoucherId:{voucherId}, Voucher#:{voucherNumber}",fileName,voucherDoc.Voucher.Id,voucherDoc.Voucher.VoucherNumber);
                }
            }
            pdfReader.Dispose();
            DeleteUsedInvoice(Path.Combine(Main.UnlinkedPath, model.SelectedDocument));
        }

        private void SetAuthorisedWatermarkOnInvoice(string path, string text)
        {
            var pdfReader = new PdfReader(path);
            //var stream = new MemoryStream();
            try
            {
                FileStream stream = new FileStream(path.Replace(".pdf","_.pdf"), FileMode.Append);
                var pdfStamper = new PdfStamper(pdfReader, stream);
            
                for (var pageIndex = 1; pageIndex <= pdfReader.NumberOfPages; pageIndex++)
                {
                    //Rectangle class in iText represent geomatric representation... in this case, rectanle object would contain page geomatry
                    Rectangle pageRectangle = pdfReader.GetPageSizeWithRotation(pageIndex);
                    //pdfcontentbyte object contains graphics and text content of page returned by pdfstamper
                    PdfContentByte pdfData = pdfStamper.GetOverContent(pageIndex);
                    //create fontsize for watermark
                    pdfData.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 40);
                    //create new graphics state and assign opacity
                    PdfGState graphicsState = new PdfGState();
                    graphicsState.FillOpacity = 0.4F;
                    //set graphics state to pdfcontentbyte
                    pdfData.SetGState(graphicsState);
                    //set color of watermark
                    pdfData.SetColorFill(BaseColor.BLUE);
                    //indicates start of writing of text
                    pdfData.BeginText();
                    //show text as per position and rotation
                    pdfData.ShowTextAligned(Element.ALIGN_CENTER, text, pageRectangle.Width / 2, pageRectangle.Height / 2, 45);
                    //call endText to invalid font set
                    pdfData.EndText();
                }
                pdfStamper.Close();
                stream.Close();
                pdfReader.Close();
                // System.IO.File.Delete("E:/PO67.pdf");
                File.Replace(path.Replace(".pdf", "_.pdf"), path,null);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Output.SetAuthorisedWatermarkOnInvoice(path:{path})",path);
            }
        }

        public void SetCancelledStampOnPO(string filePath)
        {
            try
            {
                var fullName = _ad.GetCurrentFullName();
                var fileName = "PO" + Order.OrderNumber + ".pdf";
                //var filePath = Path.Combine(fileDirectory, fileName);
                var pdfReader = new PdfReader(filePath);
                var text = "Cancelled by " + fullName + " on " + DateTime.Now.ToShortDateString();
                //var stream = new MemoryStream();
                FileStream stream = new FileStream(filePath.Replace(".pdf", "_.pdf"), FileMode.Append);
                var pdfStamper = new PdfStamper(pdfReader, stream);

                for (var pageIndex = 1; pageIndex <= pdfReader.NumberOfPages; pageIndex++)
                {
                    //Rectangle class in iText represent geomatric representation... in this case, rectanle object would contain page geomatry
                    Rectangle pageRectangle = pdfReader.GetPageSizeWithRotation(pageIndex);
                    //pdfcontentbyte object contains graphics and text content of page returned by pdfstamper
                    PdfContentByte pdfData = pdfStamper.GetOverContent(pageIndex);
                    //create fontsize for watermark
                    pdfData.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 40);
                    //create new graphics state and assign opacity
                    PdfGState graphicsState = new PdfGState();
                    graphicsState.FillOpacity = 0.4F;
                    //set graphics state to pdfcontentbyte
                    pdfData.SetGState(graphicsState);
                    //set color of watermark
                    pdfData.SetColorFill(BaseColor.BLUE);
                    //indicates start of writing of text
                    pdfData.BeginText();
                    //show text as per position and rotation
                    pdfData.ShowTextAligned(Element.ALIGN_CENTER, text, pageRectangle.Width / 2, pageRectangle.Height / 2, 45);
                    //call endText to invalid font set
                    pdfData.EndText();
                }
                pdfStamper.Close();
                stream.Close();
                pdfReader.Close();
                // System.IO.File.Delete("E:/PO67.pdf");
                File.Replace(filePath.Replace(".pdf", "_.pdf"), filePath, null);
            }
            catch (Exception e)
            {
                _logger.Error(e, "SetCancelledStampOnPO(path:{path}, OrderId:{orderId})", filePath,Order.Id);
            }
            //return filePath;
        }

        public void SendEmailWithCancelledOrder(string filePath)
        {
            var bodyText = "Please have a look at the attached file.";
            var mail = new MailMessage();
            if (!String.IsNullOrEmpty(this.Order.SupplierEmail))
            {
                mail.To.Add(new MailAddress(RemoveSpecialCharacters(this.Order.SupplierEmail)));
            }
            mail.To.Add(new MailAddress(this.Order.Author.Email));
            mail.Bcc.Add(new MailAddress("sam.shah@oneharvest.com.au"));
            if (this.Order.Author.Email != this.Order.User.Email) mail.To.Add(new MailAddress(this.Order.User.Email));
            mail.From = new MailAddress("epo@oneharvest.com.au");
            var client = new SmtpClient();
            mail.Subject = "Order cancelation notice";
            mail.Attachments.Add(new Attachment(filePath));
            mail.IsBodyHtml = true;
            mail.Body = bodyText;
            client.Send(mail);
            mail.Dispose();
        }

        public void SendEmailForEttacher(string bodyText, string subjectText)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress("epo@oneharvest.com.au");
            mail.To.Add(new MailAddress("ap@oneharvest.com.au"));
            mail.To.Add(new MailAddress("Miles.Wydall@oneharvest.com.au"));
            mail.Bcc.Add(new MailAddress("sam.shah@oneharvest.com.au"));
            var client = new SmtpClient();
            mail.Subject = subjectText;
            mail.IsBodyHtml = true;
            client.Host = "mail";
            mail.Body = PreMailer.MoveCssInline(bodyText).Html; ;
            client.Send(mail);
            mail.Dispose();
        }

      public void AuthoriseInvoiceFile(VoucherDocument voucherDoc)
        {
          try
          {
              var watermarkText = "Authorised by " + voucherDoc.Authoriser.UserInfo.FirstName + " "
                                  // ReSharper disable once PossibleInvalidOperationException
                                  + voucherDoc.Authoriser.UserInfo.LastName +"  On "+ voucherDoc.AuthorisedDate.Value.ToShortDateString();
              var fileName = voucherDoc.Reference;//voucherDoc.Voucher.VoucherNumber + filepartName + voucherDoc.Voucher.InvoiceNumber + ".pdf";
              this.SetAuthorisedWatermarkOnInvoice(Main.WarehousePath + voucherDoc.Voucher.SupplierCode + "\\" + fileName, watermarkText);
          }
          catch (Exception e)
          {
                _logger.Fatal(e,"Output.AuthoriseInvoiceFile(voucherId:" + voucherDoc.Voucher.Id + ")", e);
            }
        }

        public void DeleteUsedInvoice(string file)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            File.Delete(file);
        }

        public void SendReport(string receiver, string bodyText, string subject)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress("epo@oneharvest.com.au");
            mail.To.Add(new MailAddress(receiver));
            mail.CC.Add(new MailAddress("sam.shah@oneharvest.com.au"));
            var client = new SmtpClient();
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = bodyText;
            client.Send(mail);
            mail.Dispose();
        }

        #region Dispose

        bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                }
            }
            //dispose unmanaged resources
            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        

        #endregion

    }
}