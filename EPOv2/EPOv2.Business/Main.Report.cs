using System;
using System.Collections.Generic;
using System.Linq;

namespace EPOv2.Business
{
    using System.Data.Entity;
    using System.Web.Mvc;

    using EPOv2.Business.Interfaces;

    using PreMailer.Net;
    using ViewModels;

    public partial class Main
    {
        public void GenerateAndSendCCOwnerReport(Controller controller, string viewName, List<CostCentreViewModel> model, string receiver)
        {
            var body = RenderPartialViewToString(controller, viewName, model);
            var bodyText = PreMailer.MoveCssInline(body).Html;
            var subject = "Empty Cost Centre Owners";
            _output.SendReport(receiver, bodyText, subject);
        }

        public List<SearchVoucherResult> GetAuthorisedVouchersReport(List<SearchVoucherResult> model)
        {
            var list = new List<SearchVoucherResult>();
            foreach (var item in model)
            {
                try
                {
                    var voucher= GetVoucherInfoViewModel(Convert.ToInt32(item.VoucherNumber));
                    if (voucher!=null && !voucher.AutoApprove)
                    {
                        list.Add(item);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Report.GetAuthorisedVouchersReport(voucher#:{voucherNumber}, voucherId:{voucherId}",item.VoucherNumber,item.Id);
                }
            }
            return list;
        }

        public InvoiceExceedReportVM GetInvoiceExceedReport(InvoiceExceedReportFilterVM filter)
        {
            var model = new InvoiceExceedReportVM() {Items = new List<InvoiceExceedReportItemVM>(), ItemsWithoutOrder = new List<InvoiceExceedReportItemVM>()};
            var dateFrom = Convert.ToDateTime(filter.dateFrom);
            var dateTo = Convert.ToDateTime(filter.dateTo);
            dateTo=dateTo.AddDays(1);
            var orderList =
                _orderRepository.Get(
                    x =>
                    !x.IsDeleted && x.Status.Name != StatusEnum.Draft.ToString()
                    && x.Status.Name != StatusEnum.Pending.ToString() && x.Status.Name!=StatusEnum.Cancelled.ToString() && x.DateCreated >= dateFrom
                    && x.DateCreated <= dateTo).Include(x=>x.CostCentre).ToList();
            var orderIdList = orderList.Select(x => x.Id).ToList();
            var orderIdStrList = orderIdList.Select(x => x.ToString()).ToList();
            var vdList =
                _voucherDocumentRepository.Get(
                    x =>
                    orderIdStrList.Contains(x.Reference)
                    && x.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " ")
                    && !x.IsDeleted).Include(x=>x.Voucher).ToList();
            var vdOrderIdList = vdList.Select(x => Convert.ToInt32(x.Reference)).ToList();
            var filteredOrderList = orderList.Where(x => vdOrderIdList.Contains(x.Id)).ToList();
            foreach (var order in filteredOrderList)
            {
                var voucherList = vdList.Where(x => x.Reference == order.Id.ToString()).Select(x => x.Voucher).ToList();
                var vouchersTotals = Math.Round(voucherList.Sum(x => x.Amount));
                if ((vouchersTotals -Math.Round(order.Total,2))>=1) //exclude variance less that $1
                {
                    var item = new InvoiceExceedReportItemVM()
                                   {
                                       SupplierCode = GetSupplierCodeById(order.SupplierId),
                                       CostCentreCode = order.CostCentre.Code,
                                       OrderId = order.Id,
                                       OrderNumber = order.OrderNumber,
                                       OrderTotal = order.Total,
                                      InvoicesTotal = vouchersTotals,
                                      Variance = Math.Abs(order.Total - vouchersTotals)
                                   };
                    var i = 0;
                    foreach (var voucher in voucherList)
                    {
                        i++;
                        var authUser =
                            vdList.Where(x => x.Voucher.Id == voucher.Id).Select(x => x.Authoriser).FirstOrDefault();
                        if (authUser != null)
                        {
                            item.AuthoriserName += authUser.GetFullName();
                            if (voucherList.Count > 1 && i != voucherList.Count) item.AuthoriserName += ", ";
                        }
                    }
                    model.Items.Add(item);
                }
            }
            model.ItemsWithoutOrder = GetVouchersWithoutAttacherOrder(dateFrom,dateTo);
            model.Items = model.Items.OrderBy(x => x.CostCentreCode).ToList();
            return model;
        }

        private List<InvoiceExceedReportItemVM> GetVouchersWithoutAttacherOrder(DateTime dateFrom, DateTime dateTo)
        {
            var modelList = new List<InvoiceExceedReportItemVM>();
            var voucherList =
                _voucherRepository.Get(
                    x => !x.IsDeleted && x.DateCreated >= dateFrom && x.DateCreated <= dateTo).ToList();
            var voucherIdList = voucherList.Select(x => x.Id).ToList();
            var vdList =
                _voucherDocumentRepository.Get(
                    x =>
                    voucherIdList.Contains(x.Voucher.Id) && !x.IsDeleted
                    && x.DocumentType.Name != DocumentTypeEnum.GRNI_Invoice.ToString().Replace("_", " ")).Include(x=>x.Voucher).ToList();
            foreach (var vId in voucherIdList)
            {
                try
                {
                    var attacherPO =
                        vdList.FirstOrDefault(
                            x =>
                            x.Voucher.Id == vId 
                            && (x.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " ") || x.DocumentType.Name == DocumentTypeEnum.Purchase_Order_Scan.ToString().Replace("_", " ")));
                    var attachedInvoice = vdList.FirstOrDefault(
                        x =>
                        x.Voucher.Id == vId
                        && x.DocumentType.Name == DocumentTypeEnum.Invoice.ToString());
                    if (attachedInvoice != null && attacherPO == null)
                    {
                        var item = new InvoiceExceedReportItemVM()
                                       {
                                           AuthoriserName =
                                               attachedInvoice.Authoriser?.GetFullName()+" (attached by "+Ad.GetNameByLogin(attachedInvoice.CreatedBy)+" )",
                                           CostCentreCode = 0,
                                           SupplierCode = attachedInvoice.Voucher.SupplierCode,
                                           OrderNumber = attachedInvoice.Voucher.VoucherNumber,
                                           InvoicesTotal =
                                               voucherList.Where(x => x.Id == vId)
                                               .Select(x => x.Amount)
                                               .First(),
                                           Variance = 0,
                                           OrderId = 0,
                                       };
                        modelList.Add(item);
                        if(attachedInvoice.Authoriser==null) _logger.Warning("GetVouchersWithoutAttacherOrder -> Authoriser is null for voucherId:{voucherId}, voucher# {voucherNumber}",vId,attachedInvoice.Voucher.VoucherNumber);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Get Vouchers Without Attached Order(voucherId:{voucherId})",vId);
                }
            }
            return modelList;
        }
    }
}
