namespace EPOv2.Business
{
    using System.IO;

    using EPOv2.Business.Interfaces;

    using iTextSharp.text;
    using iTextSharp.text.pdf;

    public partial class Output: IOutput
    {
        public const string TestWarehousePath = @"E:\\";
        public const string TestSourcePath = @"G:\\";
        public void MakeWatermarkOnPdf()
        {
            PdfReader pdfReader = new PdfReader("E:/PO67.pdf");
            //create stream of filestream or memorystream etc. to create output file
            FileStream stream = new FileStream("E:/PO67_.pdf", FileMode.Append);
            //create pdfstamper object which is used to add addtional content to source pdf file
            PdfStamper pdfStamper = new PdfStamper(pdfReader, stream);
            //iterate through all pages in source pdf
            for (int pageIndex = 1; pageIndex <= pdfReader.NumberOfPages; pageIndex++)
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
                pdfData.ShowTextAligned(Element.ALIGN_CENTER, "Watermark Text", pageRectangle.Width / 2, pageRectangle.Height / 2, 45);
                //call endText to invalid font set
                pdfData.EndText();
            }
            //close stamper and output filestream

            pdfStamper.Close();
            stream.Close();
            pdfReader.Close();
            // System.IO.File.Delete("E:/PO67.pdf");
            File.Replace("E:/PO67_.pdf", "E:/PO67.pdf", "E:/PO67-backup.pdf");
        }

        public void SaveInvoiceFilettTest()
        {
            var stream = new MemoryStream(); //TestWarehousePath + "Test" + "\\" + "testfile1.pdf"
            //var pdfStamper = new PdfStamper(pdfReader, stream);

            using (var document = new Document())
            {
                
                using (var copy = new PdfCopy(document, stream))
                {
                    document.Open();
                    var pdfReader = new PdfReader(TestSourcePath + "testfile.pdf");
                    for (var pageIndex = 1; pageIndex <= 1; pageIndex++)
                    {
                        var page = copy.GetImportedPage(pdfReader, pageIndex);
                        copy.AddPage(page);
                    }
                    pdfReader.Close();
                    document.Close();
                    File.WriteAllBytes(TestWarehousePath + "Test" + "\\" + "testfile1.pdf", stream.ToArray());
                }
            }
        }

        
    }
}