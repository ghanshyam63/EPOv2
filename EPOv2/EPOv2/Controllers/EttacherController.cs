using System;
using System.Web.Mvc;

namespace EPOv2.Controllers
{
    using System.IO;

    using EPOv2.Business;
    using EPOv2.Business.Interfaces;
    using EPOv2.ViewModels;

    public class EttacherController : Controller
    {

        #region Property

        private readonly IMain _main;

        private readonly IData _data;

        private readonly IRouting _routing;

        public EttacherController(IMain main, IData data, IRouting routing)
        {
            this._main = main;
            this._data = data;
            this._routing = routing;
        }

        #endregion

        // GET: Ettacher

        #region New Voucher

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FetchVoucherPanel()
        {
            var model = this._main.GetVoucherPanelViewModel();
            return this.PartialView("_VoucherPanel", model);
        }

        public ActionResult FetchVoucherInfoTable(int voucherNumber)
        {
            var model = this._main.GetVoucherInfoViewModel(voucherNumber);
            return this.PartialView("_VoucherInfo",model);
        }

        public ActionResult FetchVoucherAttachingForm(int voucherNumber, bool isGrni =false)
        {
            var model = this._main.GetVoucherAttachingForm(voucherNumber);

            return this.PartialView("_VoucherAttachingForm",model);
        }

        public ActionResult FetchVoucherAttachment(bool loadEPO=true, string supplierCode=null)
        {
            var model = this._main.GetAttachmentPanelViewModel(loadEPO, supplierCode);
            return this.PartialView("_VoucherAttachment",model);
        }

        public ActionResult FetchGrniVoucherPOInfo(int voucherNumber)
        {
            var model = this._main.GetVoucherGRNIInfoViewModel(voucherNumber);
            return this.PartialView("_VoucherGRNIInfo", model);
        }

        public ActionResult FetchVoucherRelatedDocuments(int voucherNumber)
        {
            var model = this._main.GetVoucherRelatedDocuments(voucherNumber);
            return this.PartialView("_VoucherRelatedDocuments", model);
        }


        public ActionResult GetMaxPagesOfFile(string fileName)
        {
            var result = this._main.GetMaxPageOFFile(fileName);
            return Json(result.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrderAuthoriserId(int orderId)
        {
            var result = this._main.GetOrderAuthoriserId(orderId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEPOAmount(int orderId)
        {
            var result = this._main.GetOrderTotalAmount(orderId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckIsPOFullyMatched(int orderId)
        {
            var result = this._main.CheckIsPOFullyMatched(orderId);
            return Json(result, JsonRequestBehavior.AllowGet );
        }

        public ActionResult AttachDocumentToVoucher(VoucherAttachingFormViewModel model)
        {
            var doesNeedsRouting = _routing.CheckAttachedDocumentStatus(model.SelectedDocumentTypeId);
            var result=doesNeedsRouting ? _routing.TryToStartVoucherRouting(model) : _main.SaveVoucherAttachForm(model);
            return Json(new { status = result.Status, msg = result.Message }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index");
        }

#endregion

        
        public ActionResult Invoice(DashboardViewModel model)
        {
            var m = this._main.GetInvoiceForAuthorisation(model.SelectedItem);
            return this.View("Invoice",m);
        }

        public ActionResult AuthoriseInvoice(InvoicePageViewModel model, string button)
        {
            if (button != "Cancel")
            {
                this._main.AuthoriseInvoice(model);
            }
            return RedirectToAction("Dashboard", "Home");
        }

        public ActionResult AuthoriseInvoiceExternal(int id)
        {
            return Invoice(new DashboardViewModel() { SelectedItem = id });
        }

        public ActionResult ViewDocument(InvoicePageViewModel model)
        {
            var voucherDocumentType = this._main.GetVoucherDocumentType(model.VoucherDocumentId);
            var document = this._data.GetVoucherDocument(model.VoucherDocumentId);
            if (model.VoucherDocumentType == DocumentTypeEnum.Voucher.ToString())
            {
                voucherDocumentType = DocumentTypeEnum.Voucher;
            }
            try
            {
                switch (voucherDocumentType)
                {
                    case DocumentTypeEnum.Invoice:
                        var fileStream =
                            new FileStream(
                                Main.WarehousePath + document.Voucher.SupplierCode + "\\" + document.Reference,
                                FileMode.Open,
                                FileAccess.Read);
                        var fsResult = new FileStreamResult(fileStream, "application/pdf");
                        return fsResult;

                    case DocumentTypeEnum.Purchase_Order:
                        return RedirectToAction(
                            "ViewOrder",
                            "PurchaseOrder",
                            new DashboardViewModel() { SelectedItem = Convert.ToInt32(document.Reference) });
                    case DocumentTypeEnum.GRNI_Invoice:
                        fileStream =
                            new FileStream(
                                Main.WarehousePath + document.Voucher.SupplierCode + "\\" + document.Reference,
                                FileMode.Open,
                                FileAccess.Read);
                        fsResult = new FileStreamResult(fileStream, "application/pdf");
                        return fsResult;

                    case DocumentTypeEnum.Voucher:
                        return RedirectToAction(
                            "ViewInvoice",
                            new DashboardViewModel() { SelectedItem = model.VoucherDocumentId });

                    case DocumentTypeEnum.Purchase_Order_Scan:
                        fileStream =
                            new FileStream(
                                Main.WarehousePath + document.Voucher.SupplierCode + "\\" + document.Reference,
                                FileMode.Open,
                                FileAccess.Read);
                        fsResult = new FileStreamResult(fileStream, "application/pdf");
                        return fsResult;

                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                ViewBag.FileName = document.Reference;
                ViewBag.FilePath = Main.WarehousePath + document.Voucher.SupplierCode + "\\";
                ViewBag.DocType = voucherDocumentType;
                ViewBag.Author = this._data._ad.GetADNamebyLogin(document.CreatedBy);
                return this.View("FileNotFoundView");
            }
        }

        public ActionResult SearchVoucher(SearchViewModel model)
        {
            var result = this._main.SearchVoucher(model);
            return this.PartialView("_SearchVoucherResult", result);
        }

        public ActionResult ViewInvoice(DashboardViewModel model)
        {
          
            var m = _main.GetInvoiceDetails(model.SelectedItem); //selected voucher ID
            return View(m);
        }

        public ActionResult ViewInvoiceExternal(int SelectedInvoice)
        {
            var m = _main.GetInvoiceDetails(SelectedInvoice); //selected voucher ID
            return View("ViewInvoice",m);
        }

        public ActionResult DeleteDocument(int documnetId)
        {
            _main.DeleteDocument(documnetId);//selected document ID
            return null;
        }


        

        [HttpGet]
        public ActionResult DeleteVoucher()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult DeleteVoucher(DeleteVoucherForm model)
        {
            this._main.DeleteVoucher(model);
            return RedirectToAction("DeleteVoucher");
        }

        public ActionResult FetchVoucherForDelete(int voucherNumber)
        {
            var model = this._main.GetVoucherDeleteFormModel(voucherNumber);
            return this.PartialView("_DeleteVoucherForm", model);
        }

        [HttpGet]
        public ActionResult ResubmitVoucher()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult ResubmitVoucher(ResubmitVoucherForm model)
        {
            try
            {
                _main.ResubmitVoucher(model);
                return Json(new { status = "Success", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "Fail", success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FetchVoucherForResubmit(int voucherNumber)
        {
            var model = this._main.GetVoucherResubmitFormModel(voucherNumber);
            return this.PartialView("_ResubmitVoucherForm", model);
        }

        #region Voucher Status

        public ActionResult VoucherStatus()
        {
            return this.View();
        }

        public ActionResult FetchVoucherForStatusChange(string VoucherNumber)
        {
            var model = this._main.GetVoucherViewModelForStatusChange(Convert.ToInt32(VoucherNumber));
            return this.PartialView("_VoucherStatusChangeForm", model);
        }

        public ActionResult VoucherStatusChange(ChangeVoucherStatusForm model)
        {
            this._main.ChangeVoucherStatus(model);
            return RedirectToAction("VoucherStatus");
        }

        #endregion


    }
}