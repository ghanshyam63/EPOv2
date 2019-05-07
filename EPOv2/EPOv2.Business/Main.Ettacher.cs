namespace EPOv2.Business
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Validation;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;

    using PreMailer.Net;
    using DomainModel.DataContext.QADRealTableAdapters;
    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.ViewModels;

    using iTextSharp.text.pdf;

    using Voucher = DomainModel.Entities.Voucher;
    using VoucherDocument = DomainModel.Entities.VoucherDocument;
    using VoucherDocumentType = DomainModel.Entities.VoucherDocumentType;

    public partial class Main
    {
        //Ettacher

        #region Property

     //   public readonly OldPurchaseOrderContext _oldDb2 = new OldPurchaseOrderContext();
        //public const string WarehousePath = @"\\Wcludp1\\apdatawarehouse-bk\";
        public const string WarehousePath = @"\\dnas01\\APDatawarehouse\";
        //public const string WarehousePath = @"E:\\";

        public const string UnlinkedPath = @"\\dnas01\\APDatawarehouse\Temp\";
       // public const string UnlinkedPath = @"E:\\Temp\";

        private DirectoryInfo unlinkeDirectoryInfo = new DirectoryInfo(UnlinkedPath);

     //   public enum DocumentTypeEnum {Invoice, Purchase_Order, GRNI_Invoice}

        #endregion


        public VoucherPanelViewModel GetVoucherPanelViewModel()
        {
            var model = new VoucherPanelViewModel()
                            {
                                VoucherSearch =
                                    new VoucherSearchViewModel()
                                        {
                                            Suppliers = GetSuppliersFromQADBasedOnVouchers(),
                                            Vouchers = GetVoucherItemViewModels()
                                        }
                                       
                            };
            return model;
        }

        private List<VoucherItemViewModel> GetVoucherItemViewModels()
        {
            var model = new List<VoucherItemViewModel>();
            return model;
        }

        /// <summary>
        /// Supplier list from QADLive(vsql1) based on active/closed vouchers
        /// </summary>
        /// <param name="isShowConfirmed"></param>
        /// <param name="isFullList"></param>
        /// <returns></returns>
        public List<SupplierViewModel<string>> GetSuppliersFromQADBasedOnVouchers(bool isShowConfirmed = false, bool isFullList = false)
        {
            List<SupplierViewModel<string>> model;
            if (isFullList)
            {
                model = Queryable.Distinct((from vo in _qadLive.vo_mstr
                                                                                                                    join ap in _qadLive.ap_mstr on vo.vo_ref equals ap.ap_ref
                                                                                                                    join adm in _qadLive.ad_mstr on ap.ap_vend equals adm.ad_addr
                                                                                                                    join admE in _qadLive.ad_mstr_extra on adm.ad_addr equals admE.ad_addr
                                                                                                                    where !admE.IsDeleted
                                                                                                                    select new { Code = adm.ad_addr, FullName = adm.ad_addr + " - " + adm.ad_name }))
                    .Select(x => new SupplierViewModel<string>() { Id = x.Code, Code = x.Code, FullName = x.FullName })
                    .ToList();
               
            }
            else
            {
                model = Queryable.Distinct((from vo in _qadLive.vo_mstr
                                                                                                                    join ap in _qadLive.ap_mstr on vo.vo_ref equals ap.ap_ref
                                                                                                                    join adm in _qadLive.ad_mstr on ap.ap_vend equals adm.ad_addr
                                                                                                                    join admE in _qadLive.ad_mstr_extra on adm.ad_addr equals admE.ad_addr
                                                                                                                    where vo.vo_confirmed == isShowConfirmed && ap.ap_open == true && !admE.IsDeleted
                                                                                                                    select new { Code = adm.ad_addr, FullName = adm.ad_addr + " - " + adm.ad_name }))
                       .Select(
                           x => new SupplierViewModel<string>() { Id = x.Code, Code = x.Code, FullName = x.FullName })
                       .ToList();
            }

            return model;
        }

        public VoucherInfoViewModel GetVoucherInfoViewModel(int voucherNumber)
        {
            //var voucher = qadLive.vo_mstr
            var qadVoucherInfoTA = new QADRealVoucherInfoTableAdapter();
            var qadVoucherInfoDT = qadVoucherInfoTA.GetData(voucherNumber.ToString());
            foreach (var viRow in qadVoucherInfoDT)
            {
                var model = new VoucherInfoViewModel()
                                {
                                    CostCentre = viRow.ap_cc,
                                   // Account = viRow.vod_acct,
                                    DueDate = viRow.vo_due_date.ToShortDateString(),
                                    InvoiceNumber = viRow.vo_invoice,
                                    Terms = viRow.vo_cr_terms,
                                    VoucherNumber = voucherNumber.ToString(),
                                    Amount = Convert.ToDouble(viRow.ap_amt),
                                    Supplier = _qadLive.ad_mstr.Where(x=>x.ad_addr.ToUpper()==viRow.ap_vend.ToUpper()).Select(x=> x.ad_addr).FirstOrDefault(),
                                    Status = viRow.vo_confirmed ? "Confirmed" : "Unconfirmed",
                                    AutoApprove = viRow.vo_confirmed
                                };
                return model;
            }
            return new VoucherInfoViewModel();
        }

        public VoucherAttachmentPanelViewModel GetAttachmentPanelViewModel(bool loadEPO = true, string supplierCode = null)
        {
            var model = new VoucherAttachmentPanelViewModel()
                            {
                                InvoiceList =loadEPO ? GetEPOAsFileInvoiceViewModelList(supplierCode) : GetVoucherFileInvoiceViewModelList(),
                                loadEPO = loadEPO
                            };
            return model;
        }

        public List<VoucherFileInvoiceViewModel> GetVoucherFileInvoiceViewModelList()
        {
            var files = GetFileInvoiceList();
            return files.Select(file => new VoucherFileInvoiceViewModel { Id = file.Name, Name = file.Name.Replace(file.Extension,"") }).ToList();
        }

        public List<VoucherFileInvoiceViewModel> GetEPOAsFileInvoiceViewModelList(string supplierCode=null)
        {
            var epos =
                _orderRepository.Get(x => !x.IsDeleted && x.Status!=null && x.Status.Name != StatusEnum.Draft.ToString() && x.Status.Name != StatusEnum.Pending.ToString() && x.Status.Name != StatusEnum.Closed.ToString()).OrderBy(x => x.OrderNumber).ToList();
            if(supplierCode!=null)
            {
                var supplierId =
                    oldDb.tblSuppliers.Where(x => x.SupplierCode == supplierCode)
                        .Select(x => x.SupplierID)
                        .FirstOrDefault();
                epos = epos.Where(x => x.SupplierId == supplierId).ToList();
            }
            return epos.Select(epo => new VoucherFileInvoiceViewModel { Id = epo.Id.ToString(), Name = epo.OrderNumber.ToString()}).ToList();
        }

        public List<FileInfo> GetFileInvoiceList()
        {
            return unlinkeDirectoryInfo.EnumerateFiles().Where(x => x.Extension.ToUpper() == ".PDF").OrderBy(x=>x.Name).ToList();
        }

        public VoucherAttachingFormViewModel GetVoucherAttachingForm(int voucherNumber)
        {
           // var voucherInfo = GetVoucherInfoViewModel(voucherNumber);
            var authoriser = GetAuthoriserForInvoice(voucherNumber);
            var model = new VoucherAttachingFormViewModel()
                            {
                                SelectedAuthoriser = authoriser,
                                AuthoriserList = GetUserViewModels(),
                                VoucherNumber = voucherNumber,
                               // SupplierCode = voucherInfo.Supplier,
                                VoucherDocumentTypes = GetVoucherDocumentTypes()   ,
                                PageFrom = 1,
                                PageTo = 1
                            };
            return model;
        }

        private string GetAuthoriserForInvoice(int voucherNumber)
        {
            var doc = _voucherDocumentRepository.FirstOrDefault(x => !x.IsDeleted && x.Voucher.VoucherNumber == voucherNumber);
            if (doc != null)
            {
                return doc.Authoriser != null ? doc.Authoriser.Id : string.Empty;
            }
            return string.Empty;
        }

        public List<VoucherDocumentType> GetVoucherDocumentTypes()
        {
            var list = _voucherDocumentTypeRepository.Get(x => !x.IsDeleted).ToList();
            return list;
        }

        public int GetMaxPageOFFile(string fileName)
        {
            var pdfReader = new PdfReader(UnlinkedPath+fileName);
            return pdfReader.NumberOfPages;
        }

        public ReturnResutViewModel SaveVoucherAttachForm(VoucherAttachingFormViewModel model)
        {
            var fileName = GenerateFileName(model);
            try
            {
                var voucher = _voucherRepository.FirstOrDefault(x => x.VoucherNumber == model.VoucherNumber);
                if (voucher != null)
                {
                    voucher.SupplierCode = model.SupplierCode;
                    voucher.Amount = model.Amount;
                    voucher.Comment = model.Comment;
                    voucher.DueDate = Convert.ToDateTime(model.DueDate);
                    voucher.Status = model.IsAuthorised
                                         ? _voucherStatusRepository.FirstOrDefault(
                                             x => x.Name == StatusEnum.Authorised.ToString())
                                         : _voucherStatusRepository.FirstOrDefault(
                                             x => x.Name == StatusEnum.Pending.ToString());
                    voucher.LastModifiedBy = _curUser;
                    voucher.LastModifiedDate = DateTime.Now;
                    voucher.CostCentre =_costCentreRepository.FirstOrDefault(x => x.Code == model.CostCentreCode && !x.IsDeleted);
                    voucher.InvoiceNumber = model.InvoiceNumber;
                    voucher.Terms = model.Terms;
                }
                else
                {
                    voucher = new Voucher()
                    {
                        Account =
                                          _accountRepository.FirstOrDefault(
                                              x => x.Code == model.AccountCode && !x.IsDeleted),
                        Amount = model.Amount,
                        Comment = model.Comment,
                        CostCentre =
                                          _costCentreRepository.FirstOrDefault(
                                              x => x.Code == model.CostCentreCode && !x.IsDeleted),
                        InvoiceNumber = model.InvoiceNumber,
                        SupplierCode = model.SupplierCode,
                        VoucherNumber = model.VoucherNumber,
                        CreatedBy = _curUser,
                        LastModifiedBy = _curUser,
                        DateCreated = DateTime.Now,
                        LastModifiedDate = DateTime.Now,
                        DueDate = Convert.ToDateTime(model.DueDate),
                        Terms = model.Terms,
                        Status =
                                          model.IsAuthorised
                                              ? _voucherStatusRepository.FirstOrDefault(
                                                  x => x.Name == StatusEnum.Authorised.ToString())
                                              : _voucherStatusRepository.FirstOrDefault(
                                                  x => x.Name == StatusEnum.Pending.ToString())
                    };
                }
                var voucherDocument = CreateUpdateVoucherDocument(model, fileName, voucher);

                if (voucherDocument.DocumentType.Name == DocumentTypeEnum.Invoice.ToString()
                    || voucherDocument.DocumentType.Name == DocumentTypeEnum.GRNI_Invoice.ToString().Replace("_", " ")
                    || voucherDocument.DocumentType.Name
                    == DocumentTypeEnum.Purchase_Order_Scan.ToString().Replace("_", " "))
                {
                    voucherDocument.Reference = fileName;
                    if (!File.Exists(Path.Combine(UnlinkedPath, model.SelectedDocument)))
                    {
                        _logger.Error("SaveVoucherAttachForm(document:{document})",model.SelectedDocument);
                        return new ReturnResutViewModel()
                        {
                            Status = "Error",
                            Message = "File not exist, please check folder."
                        };
                    }
                    if (voucherDocument.DocumentType.Name == DocumentTypeEnum.GRNI_Invoice.ToString().Replace("_", " "))
                    {
                        _output.SaveInvoiceFile(model, voucherDocument, fileName, true);
                        // _output.DeleteUsedInvoice(Path.Combine(UnlinkedPath, model.SelectedDocument));
                    }
                    else
                    {
                        _output.SaveInvoiceFile(model, voucherDocument, fileName);
                        //_output.DeleteUsedInvoice(Path.Combine(UnlinkedPath, model.SelectedDocument));
                    }
                }
                else //electronic PO
                {
                    if (voucherDocument.IsAuthorised) Data.ChangeOrderStatus(new OrderViewModel() { OrderId = Convert.ToInt32(voucherDocument.Reference), SelectedStatus = 11 }); //Set status Close for PO
                }

                if (voucherDocument.Id == 0) _voucherDocumentRepository.Add(voucherDocument);
                db.SaveChanges();

            }
            catch (DbEntityValidationException dbEx)
            {
                LogError("Main.Ettacher.SaveVoucherAttachForm(voucher#:" + model.VoucherNumber + ", Ref:" + model.SelectedDocument + ")", dbEx);
            }
            return new ReturnResutViewModel() { Status = "Success", Message = "Attached successfully" };
        }

        public static string GenerateFileName(VoucherAttachingFormViewModel model)
        {
            var filepartName = "-Invoice";
            if (model.SelectedDocumentTypeId == 3) //is GRNI
            {
                filepartName = "-GRNI Invoice";
            }
            if (model.SelectedDocumentTypeId == 4) //is PO Scan
            {
                filepartName = "-POScan";
            }
            var fileName = model.VoucherNumber + filepartName + model.InvoiceNumber + ".pdf";

            fileName = Output.RemoveSpecialCharactersFromFileName(fileName);
            return fileName;
        }

        public VoucherDocument CreateUpdateVoucherDocument(VoucherAttachingFormViewModel model, string fileName, Voucher voucher)
        {
            var voucherDocument =
                _voucherDocumentRepository.FirstOrDefault(
                    x =>
                    (x.Reference == model.SelectedDocument || x.Reference == fileName)
                    && x.Voucher.VoucherNumber == model.VoucherNumber
                    && x.DocumentType.Id == model.SelectedDocumentTypeId);
            var authoriser = Data.CheckAuthoriserForSubstitution(model.SelectedAuthoriser);
            if (voucherDocument != null)
            {
                voucherDocument.LastModifiedBy = _curUser;
                voucherDocument.LastModifiedDate = DateTime.Now;
                voucherDocument.Authoriser = authoriser;
                voucherDocument.AuthorisedDate = model.IsAuthorised ? DateTime.Now : (DateTime?)null;
                voucherDocument.IsAuthorised = model.IsAuthorised;
                voucherDocument.IsDeleted = false;
                voucherDocument.Reference = model.SelectedDocument;
                voucherDocument.DocumentType = _voucherDocumentTypeRepository.Find(model.SelectedDocumentTypeId);
            }
            else
            {
                if (VadidateVoucherDocumentType(model))
                {
                    model.SelectedDocumentTypeId = 1;
                }
                voucherDocument = new VoucherDocument()
                {
                    Voucher = voucher,
                    Authoriser =authoriser,
                    AuthorisedDate =model.IsAuthorised ? DateTime.Now : (DateTime?)null,
                    //Convert.ToDateTime("1900:01:01"),
                    IsAuthorised = model.IsAuthorised,
                    CreatedBy = _curUser,
                    DateCreated = DateTime.Now,
                    LastModifiedBy = _curUser,
                    LastModifiedDate = DateTime.Now,
                    Reference = model.SelectedDocument,
                    DocumentType = _voucherDocumentTypeRepository.Find(model.SelectedDocumentTypeId)
                };
            }

            return voucherDocument;
        }

        /// <summary>
        /// Double check Document type. Try to prevent user input error.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool VadidateVoucherDocumentType(VoucherAttachingFormViewModel model)
        {
            return !model.SelectedDocument.Contains(".pdf") && model.SelectedDocumentTypeId != 2;
        }

        public InvoicePageViewModel GetInvoiceForAuthorisation(int voucherNumber)
        {
            //voucherNumber = 625385;
            _logger.Information($"Authorise Invoice button from EPO.Dashboard, id{voucherNumber}");
            var model = new InvoicePageViewModel();
            try
            {
                var voucher = _voucherRepository.FirstOrDefault(x => x.VoucherNumber == voucherNumber && !x.IsDeleted);
                var relatedVouchers=new List<Voucher>();
                var voucherDocs = _voucherDocumentRepository.Get(x => x.Voucher.Id == voucher.Id && !x.IsDeleted).ToList();
                //EPO
                var orderId = voucherDocs.Where(x => x.Voucher.Id == voucher.Id && !x.IsDeleted && x.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_"," "))
                                        .Select(x=>x.Reference)
                                        .FirstOrDefault();
                if (!string.IsNullOrEmpty(orderId))
                {
                    var ordId = 0;
                    var isNumber = int.TryParse(orderId,out ordId);
                    if (isNumber)
                    {
                        var order = _orderRepository.Find(ordId);
                        model.OrderItems = GetItemTableViewModelListForInvoicePage(order);
                        model.OrderTotal = order.Total;
                        if ((order.Status.Name == StatusEnum.Receipt_Partial.ToString().Replace("_", " "))
                            || (order.Status.Name == StatusEnum.Receipt_in_Full.ToString().Replace("_", " "))) model.isAuthorisable = true;
                        relatedVouchers = FindRelatedVoucher(orderId, voucher.Id);
                        model.RelatedInvoiceTotal = relatedVouchers.Select(x => x.Amount).Sum();
                    }
                }
                else //Check for GRNI
                {
                    model.isGRNI = CheckIsVoucherGrniType(voucherDocs);
                    model.isAuthorisable = !model.isGRNI;
                }

                model.VoucherInfo = GetVoucherInfoViewModel(voucherNumber);
                model.VoucherDetails = GetVoucherDetails(voucherNumber);
                model.InvoiceTotal = model.VoucherInfo?.Amount ?? 0;
                //model.RelatedInvoiceTotal += model.InvoiceTotal;
                //model.Comment = voucher.Comment;
                model.DisplayComment = voucher.Comment;
                model.VoucherDocumentId = voucherDocs.Where(x=>(x.DocumentType.Name == DocumentTypeEnum.Invoice.ToString() || x.DocumentType.Name == DocumentTypeEnum.GRNI_Invoice.ToString().Replace("_"," ")))
                                                     .Select(x => x.Id)
                                                     .FirstOrDefault();
               
                    model.InvoiceNumber = voucher.InvoiceNumber.ToString();
            
                model.SupplierName = GetSupplierFullNameByCode(voucher.SupplierCode);
                model.Variance =  model.OrderTotal-model.RelatedInvoiceTotal;
                if (model.isGRNI)
                {
                    model.QadOrderItems = GetQadOrderItemWithVariance(voucherNumber);
                    model.OrderTotal = model.QadOrderItems.Sum(x => x.POLineTotal);
                    model.Variance = model.QadOrderItems.Sum(x => x.VarianceLineTotal);
                    if (model.OrderTotal < model.RelatedInvoiceTotal)
                    {
                        // model.IsVarianceNegative = true;
                        model.Variance= Math.Round(model.Variance);
                    }
                    model.isAuthorisable = true;

                }

                model.RelatedDocuments = GetRelateVoucherDocument(voucher);
                if (relatedVouchers.Count > 0)
                {
                    var list = ConvertVouchersToRelatedDocuments(relatedVouchers);
                    model.RelatedDocuments.AddRange(list);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e,$"GetInvoiceForAuthorisation:voucher# = {voucherNumber}");
                throw;
            }

            //TODO E003: fully fill VM 


            return model;
        }

        private static bool CheckIsVoucherGrniType(IEnumerable<VoucherDocument> voucherDocs)
        {
            return voucherDocs.Any(x => x.DocumentType.Name == DocumentTypeEnum.GRNI_Invoice.ToString().Replace("_", " "));
        }

        private List<VoucherRelatedDocumentViewModel> ConvertVouchersToRelatedDocuments(List<Voucher> relatedVouchers)
        {
            var list = new List<VoucherRelatedDocumentViewModel>();
            foreach (var voucher in relatedVouchers)
            {
                var relDoc = new VoucherRelatedDocumentViewModel()
                {
                    DocumentId = voucher.Id,
                    Type = "Voucher",
                    //TODO E002: Make proper link to file/order
                    //Reference = doc.Reference
                };
                var voucherAuthoriser =
                    _voucherDocumentRepository.Get(
                        x =>
                        x.Voucher.Id == voucher.Id && x.Voucher.Status.Name == StatusEnum.Authorised.ToString()
                        && !x.IsDeleted).Select(x => x.Authoriser).FirstOrDefault();
                if (voucherAuthoriser != null)
                {
                    relDoc.Authoriser = voucherAuthoriser.UserInfo.FirstName + " " + voucherAuthoriser.UserInfo.LastName;
                }
                list.Add(relDoc);
            }
            return list;
        }

        private double GetRelatedInvoiceTotal(string orderId, int voucherId)
        {
            var list= FindRelatedVoucher(orderId, voucherId);
            return list.Select(x => x.Amount).Sum();
        }

        public List<Voucher> FindRelatedVoucher(string orderId, int voucherId)
        {
            var voucherList =
                _voucherDocumentRepository.Get(
                    x =>
                   !x.IsDeleted
                    && x.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " ")
                    && x.Reference == orderId).Select(x => x.Voucher).ToList();
            return voucherList;
        }

        private List<VoucherDetail> GetVoucherDetails(int voucherNumber)
        {
            var list = new List<VoucherDetail>();
            var qadVoucherDetailsTA = new QADRealVoucherDetailTableAdapter();
            var qadVoucherDetailsDT = qadVoucherDetailsTA.GetData(voucherNumber.ToString());
            foreach (var vdRow in qadVoucherDetailsDT)
            {
                var item = new VoucherDetail()
                               {
                                   Entity = vdRow.vod_entity,
                                   Line = vdRow.vod_ln,
                                   CostCentre = vdRow.vod_cc,
                                   Description = vdRow.vod_desc,
                                   AccountCode = vdRow.vod_acct,
                                   SubAccount = vdRow.vod_sub,
                                   Amount = Convert.ToDouble(vdRow.vod_amt)
                               };
                list.Add(item);
            }
            return list;
        }

        public InvoicePageViewModel GetInvoiceDetails(int voucherId)
        {
            var vNumber = _voucherRepository.Get(x => x.Id == voucherId).Select(x => x.VoucherNumber).First();
            var model = GetInvoiceForAuthorisation(vNumber);
            return model;
        }

        private List<QADOrderItemTableViewModel> GetQadOrderItemWithVariance(int voucherNumber)
        {
            var list = new List<QADOrderItemTableViewModel>();
            var raList = GetQADPObyVoucher(voucherNumber).Select(x => x.RA).ToList();
            var qadVITA = new QADPOVarianceInfoTableAdapter();
            foreach (var ra in raList)
            {
                var qadVIDT = qadVITA.GetData(voucherNumber.ToString(),ra);
                foreach (var vi in qadVIDT)
                {
                    var item = new QADOrderItemTableViewModel()
                                   {
                                       OrderNumber = vi.prh_nbr,
                                       Line = vi.prh_line,
                                       Description = vi.prh_part,
                                       POLineTotal = Convert.ToDouble(vi.LINETOTAL),
                                       QtyInvoiced = Convert.ToDouble(vi.pvo_vouchered_qty),
                                       QtyOpen = Math.Abs(Convert.ToDouble(vi.QTYOPEN)),
                                       QtyOrdered = Convert.ToDouble(vi.prh_qty_ord),
                                       QtyRecieved = Convert.ToDouble(vi.prh_cum_rcvd),
                                       UnitPrice = Convert.ToDouble(vi.prh_pur_cost),
                                       UnitPriceInvoiced = Convert.ToDouble(vi.vph_inv_cost),
                                       UnitPriceVariance = Math.Abs(Convert.ToDouble(vi.UNITVAR)),
                                       VarianceLineTotal = Math.Abs(Convert.ToDouble(vi.LINETOTALVAR))
                                   };
                    list.Add(item);
                }
                qadVIDT.Dispose();
            }
            return list.OrderBy(x=>x.OrderNumber).ThenBy(x=>x.Line).ToList();
        }

        public List<VoucherRelatedDocumentViewModel> GetRelateVoucherDocument(Voucher voucher)
        {
            var list = new List<VoucherRelatedDocumentViewModel>();
            var docList = _voucherDocumentRepository.Get(x => x.Voucher.Id == voucher.Id && !x.IsDeleted).ToList();
            foreach (var doc in docList)
            {
                var relDoc = new VoucherRelatedDocumentViewModel()
                                 {
                                     DocumentId = doc.Id,
                                     Authoriser = doc.Authoriser!=null ?
                                         doc.Authoriser.UserInfo.FirstName + " "
                                         + doc.Authoriser.UserInfo.LastName : string.Empty,
                                     Type = doc.DocumentType.Name,
                                     //TODO E002: Make proper link to file/order
                                     Reference = doc.Reference
                                 };
                if (doc.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " "))
                {
                    var ordId=0;
                    var isNumber = int.TryParse(doc.Reference, out ordId);
                    if (isNumber)
                    {
                        relDoc.Authoriser = Data.GetAuthoriser(ordId);
                        relDoc.IsMatchable = CheckOrderForMatching(ordId);
                    }
                }
                list.Add(relDoc);
            }
            return list;
        }

        private bool CheckOrderForMatching(int orderId)
        {
            var order = _orderRepository.Get(x => x.Id == orderId).Include(x=>x.Status).Include(x=>x.ReceiptGroup).FirstOrDefault();
            if (order == null){return false;}
            if (order.Status.Name != StatusEnum.Receipt_Partial.ToString().Replace("_", " ")
                && order.Status.Name != StatusEnum.Receipt_in_Full.ToString().Replace("_", " "))
            {
                var myReceiptGroups = _groupMemberRepository.Get(x => x.User.UserName == _curUser && !x.IsDeleted)
                    .Select(x => x.Group.Name)
                    .ToList();
                return myReceiptGroups.Contains(order.ReceiptGroup.Name);
            }
            return false;
        }
        public Order GetOrderToModifyDueDate(int orderId)
        {
            var routelist = _routeRepository.Get(x => x.Order.OrderNumber == orderId).OrderBy(x => x.Number).Select(x => x.Approver.User.UserName).ToList();
            var order = _orderRepository.Get(x => x.OrderNumber == orderId && !x.IsDeleted).Include(x => x.OrderItems).Include(x => x.ReceiptGroup).FirstOrDefault();
            var ReceiptGroup = _groupMemberRepository.Get(x => x.Group.Id == order.ReceiptGroup.Id).Select(x => x.User.UserName).ToList();

            if (routelist.Contains(_curUser))
            {
                order = _orderRepository.Get(x => x.OrderNumber == orderId && !x.IsDeleted).Include(x => x.OrderItems).FirstOrDefault();
            }
            else if (ReceiptGroup.Contains(_curUser))
            {
                order = _orderRepository.Get(x => x.OrderNumber == orderId && !x.IsDeleted).Include(x => x.OrderItems).FirstOrDefault();
            }
            else
            {
                order = _orderRepository.Get(x => x.OrderNumber == orderId && !x.IsDeleted && x.User.UserName==_curUser).Include(x => x.OrderItems).FirstOrDefault();
            }
            if (order == null) { return null; }
           
            return order;
        }
        public Order SaveOrderToModifyDueDate(Order m)
        {
          
            foreach (var Oitem in m.OrderItems)
            {
                var orderItemToUpdate = _orderItemRepository.Get(x => x.Id == Oitem.Id).FirstOrDefault();
                orderItemToUpdate.DueDate = Oitem.DueDate;
                orderItemToUpdate.LastModifiedDate = DateTime.Now.Date;
                orderItemToUpdate.LastModifiedBy = _curUser;
              _orderItemRepository.Update(orderItemToUpdate);
            }
           
            db.SaveChanges();
            return m;
        }

        public List<OrderItemTableViewModel> GetItemTableViewModelListForInvoicePage(Order order)
        {
            var list = GetItemTableViewModelList(order);
            foreach (var item in list)
            {
                item.OrderNumber = order.OrderNumber.ToString();
                item.Status = order.Status.Name;
                item.Author =order.Author.UserInfo.FirstName + " "+ order.Author.UserInfo.LastName;
            }
            return list;
        }

        public void AuthoriseInvoice(InvoicePageViewModel model)
        {
            var voucherDoc = _voucherDocumentRepository.Find(model.VoucherDocumentId);
            switch (model.SelectedStatus)
            {
                case "Authorised":
                    FinaliseAuthorisationInvoice(voucherDoc, model);
                    if (Math.Abs(model.Variance) < 1)
                    {
                        TryToCloseOrder(voucherDoc);
                    }
                    break;
                case "Declined":
                    RejectAuthorisationInvoice(voucherDoc, model);
                    break;
            }
        }

        private void TryToCloseOrder(VoucherDocument voucherDoc)
        {
            var orderId =
                _voucherDocumentRepository.Get(
                    x =>
                    x.Voucher.VoucherNumber == voucherDoc.Voucher.VoucherNumber
                    && x.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " ") && !x.IsDeleted)
                    .Select(x => x.Reference)
                    .FirstOrDefault();
            var refCnt =
                _voucherDocumentRepository.Get(
                    x => x.Reference == orderId && x.Voucher.Status.Name != StatusEnum.Authorised.ToString()).Count();
            if (orderId == null)
            {
                return;
            }
            try
            {
                var order = _orderRepository.Find(Convert.ToInt32(orderId));
                if (order == null)
                {
                    return;
                }
                switch (order.Status.Name)
                {
                    case "Receipt Partial":
                        break; //Maybe add status "Partial Receipt, Invoiced"
                    case "Receipt in Full":
                        if(refCnt==0 ) CloseOrder(order);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                LogError("Main.Ettacher.TryToCloseOrder(voucher#:"+voucherDoc.Voucher.VoucherNumber +"; " + "orderID:"+orderId+")", exception);
            }
        }

        private void CloseOrder(Order order)
        {
            Data.GenerateOrderLog(order,OrderLogSubject.Ettacher);
            order.Status = _statusRepository.Get(x => x.Name == StatusEnum.Closed.ToString()).First();
            _orderRepository.Update(order);
            //order.LastModifiedBy = this._curUser;
            //order.LastModifiedDate = DateTime.Now;
            db.SaveChanges();
        }

        private void RejectAuthorisationInvoice(VoucherDocument voucherDoc, InvoicePageViewModel  model)
        {
            voucherDoc.IsAuthorised = false;
            _voucherDocumentRepository.Update(voucherDoc);
            var voucher = voucherDoc.Voucher;
            voucher.Status = _voucherStatusRepository.FirstOrDefault(x => x.Name == StatusEnum.Declined.ToString());
            voucher.Comment += CreateCommentRecordForVoucher(model.Comment,true);
            _voucherRepository.Update(voucher);
            db.SaveChanges();
        }

        private void FinaliseAuthorisationInvoice(VoucherDocument voucherDoc, InvoicePageViewModel model)
        {
            try
            {
                var voucher = _voucherRepository.Find(voucherDoc.Voucher.Id);
                AuthoriseVoucher(voucher, voucherDoc, model.Comment);
            }
            catch (Exception e)
            {
                LogError("Main.Ettacher.FinaliseAuthorisationInvoice(voucherDocId: " + voucherDoc.Id, e);
            }
        }

        private void AuthoriseVoucher( Voucher voucher, VoucherDocument voucherDoc, string comment="")
        {
            var vDocList = _voucherDocumentRepository.Get(x => x.Voucher.Id == voucher.Id && !x.IsDeleted).ToList();
            var voucherRoutes = _voucherRouteRepository.Get(x => !x.IsDeleted && x.Voucher.Id == voucher.Id).Include(x=>x.Approver).Include(x=>x.Approver.User).ToList();
            if (voucherRoutes.Count > 0)
            {
                var currRoute = voucherRoutes.FirstOrDefault(x => x.Approver.User.EmployeeId == CurEmpId);
                if (currRoute?.Approver != null)
                {
                    if (voucherRoutes.Count > 1)
                    {
                        currRoute.Approver.IsDeleted = true;
                        currRoute.IsDeleted = true;
                        _voucherRouteRepository.Update(currRoute);
                        //_approverRepository.Update(currRoute.Approver);
                        db.SaveChanges();
                        SetAuthoriserForVoucherAndChangeStatus(voucher,vDocList, comment);
                    }
                    else
                    {//Last approver
                        currRoute.Approver.IsDeleted = true;
                        currRoute.IsDeleted = true;
                        _voucherRouteRepository.Update(currRoute);
                        //_approverRepository.Update(currRoute.Approver);
                        VoucherAuthorisation(voucher,voucherDoc,comment,vDocList);
                    }
                }

            }
            else
            {
                VoucherAuthorisation(voucher, voucherDoc, comment, vDocList);
            }
        }

        private void AuthoriseVoucherManually(Voucher voucher, VoucherDocument voucherDoc)
        {
            var vDocList = _voucherDocumentRepository.Get(x => x.Voucher.Id == voucher.Id && !x.IsDeleted).ToList();
            var voucherRoutes = _voucherRouteRepository.Get(x => !x.IsDeleted && x.Voucher.Id == voucher.Id).Include(x => x.Approver).Include(x => x.Approver.User).ToList();
            if (voucherRoutes.Count > 0)
            {
                foreach (var voucherRoute in voucherRoutes)
                {
                    voucherRoute.IsDeleted = true;
                    _voucherRouteRepository.Update(voucherRoute);
                }
            }
            VoucherAuthorisation(voucher, voucherDoc, string.Empty, vDocList);
        }

        //TODO Why we have list of docs to authorise and just 1 doc to stamp?
        private void VoucherAuthorisation(Voucher voucher, VoucherDocument voucherDoc, string comment, List<VoucherDocument> vDocList)
        {
            foreach (var vDoc in vDocList)
            {
                vDoc.IsAuthorised = true;
                vDoc.AuthorisedDate = DateTime.Now;
                _voucherDocumentRepository.Update(vDoc);
            }
            voucher.Status = _voucherStatusRepository.FirstOrDefault(x => x.Name == StatusEnum.Authorised.ToString());
            voucher.Comment +=CreateCommentRecordForVoucher(comment);
            _voucherRepository.Update(voucher);
            db.SaveChanges();
            _output.AuthoriseInvoiceFile(voucherDoc);
        }

        public void SetAuthoriserForVoucherAndChangeStatus(Voucher voucher, List<VoucherDocument> vDocList, string comment)
        {
            voucher.Status = _voucherStatusRepository.FirstOrDefault(x => x.Name == StatusEnum.Pending.ToString());
            voucher.Comment += CreateCommentRecordForVoucher(comment);
            _voucherRepository.Update(voucher);
            var authoriser = _voucherRouteRepository.Get(x => !x.IsDeleted && x.Voucher.Id == voucher.Id)
                        .OrderBy(x => x.Number)
                        .Select(x => x.Approver.User)
                        .First();
            foreach (var voucherDocument in vDocList)
            {
                voucherDocument.IsAuthorised = false;
                voucherDocument.Authoriser = authoriser;
                _voucherDocumentRepository.Update(voucherDocument);
            }
            db.SaveChanges();
        }

        private static string CreateCommentRecordForVoucher(string comment, bool isRejected=false, bool isResubmit=false)
        {
            
            var result = string.Empty;
            if (string.IsNullOrWhiteSpace(comment)) return result;

            result += Ad.GetCurrentFullName() + "(" + DateTime.Now.ToString("g") + "): " + comment;
            if (isRejected) result += "(rej)";
            if (isResubmit) result += "(re-submit)";
            result+=Environment.NewLine;
            return result;
        }

        public DocumentTypeEnum GetVoucherDocumentType(int voucherDocumentId)
        {
            var type =
                _voucherDocumentRepository.Get(x => x.Id == voucherDocumentId)
                    .Select(x => x.DocumentType.Name)
                    .FirstOrDefault();
            switch (type)
            {
                case "Invoice" : return DocumentTypeEnum.Invoice;
                case "Purchase Order": return DocumentTypeEnum.Purchase_Order;
                case "GRNI Invoice": return DocumentTypeEnum.GRNI_Invoice;
                case "Purchase Order Scan": return DocumentTypeEnum.Purchase_Order_Scan;
                default :
                    return DocumentTypeEnum.Invoice;
            }
        }

        public string GetVoucherDocumentTypeName(DocumentTypeEnum type)
        {
            return type.ToString().Replace("_", " ");
        }

        public string GetOrderAuthoriserId(int orderId)
        {
            return
                _routeRepository.Get(x => x.Order.Id == orderId)
                    .OrderByDescending(x => x.Number)
                    .Select(x => x.Approver.User.Id)
                    .First();
        }

        /// <summary>
        /// Check for invoice status, Return True if it's Authorised
        /// </summary>
        /// <param name="order"></param>
        /// <returns>Return True if it's Authorised</returns>
        private bool CheckForInvoice(Order order)
        {
            var voucherDoc =
                _voucherDocumentRepository.FirstOrDefault(
                    x =>
                    x.Reference == order.Id.ToString() && x.Voucher.Status.Name == StatusEnum.Authorised.ToString());
            return voucherDoc != null;
        }

        public VoucherGRNIInfoViewModel GetVoucherGRNIInfoViewModel(int voucherNumber)
        {
            var model = new VoucherGRNIInfoViewModel { VoucherPOs = GetQADPObyVoucher(voucherNumber) };
            model.VoucherPOItemDetails = GetQADPOItembyRA(model.VoucherPOs.Select(x => x.RA).ToList());
            model.TotalPO = model.VoucherPOs.Sum(x => x.Total);
            return model;
        }

        

        private List<VoucherPOItemViewModel> GetQADPObyVoucher(int voucherNumber)
        {
            var list = new List<VoucherPOItemViewModel>();
            var qadPOTA = new QADPoTableAdapter();
            var qadPODT = qadPOTA.GetData(voucherNumber.ToString());
            foreach (var poRow in qadPODT)
            {
                var model = new VoucherPOItemViewModel()
                {
                    Voucher = poRow.vpo_ref,
                    PO = poRow.vpo_po,
                    RA = poRow.prh_receiver,
                    Total = Convert.ToDouble(poRow.TOTAL)
                };
                list.Add(model);
            }
            return list;
        }

        private List<VoucherPOItemDetailViewModel> GetQADPOItembyRA(List<string> raList)
        {
            var list = new List<VoucherPOItemDetailViewModel>();
            foreach (var ra in raList)
            {
                var qadPOItemTA = new QADPOItemDetailTableAdapter();
                var qadPOItemDT = qadPOItemTA.GetData(ra);
                foreach (var item in qadPOItemDT)
                {
                    var model = new VoucherPOItemDetailViewModel()
                    {
                        Site = item.prh_site.ToUpper(),
                        InvoicedQty = Convert.ToDouble(item.prh_inv),
                        Line = item.prh_line,
                        Part = item.prh_part,
                        RecievedQty = Convert.ToDouble(item.prh_rcvd),
                        UnitCost = Convert.ToDouble(item.prh_pur_cost),
                        UnitGST = Convert.ToDouble(item.GST),
                        Total = Convert.ToDouble(item.Total)
                    };
                    list.Add(model);
                }
                qadPOItemDT.Dispose();
                qadPOItemTA.Dispose();
            }
            
            var total = list.Sum(x => x.Total);
            var m= new VoucherPOItemDetailViewModel()
                {
                    Site = "Total",
                    InvoicedQty = null,
                    Line = null,
                    Part = null,
                    RecievedQty = null,
                    UnitCost = null,
                    UnitGST = null,
                    Total = total
                };
            list.Add(m);
            return list;
        }

        public List<SearchVoucherResult> SearchVoucher(SearchViewModel model)
        {
            var searchVoucherList = new List<SearchVoucherResult>();

            try
            {
                IQueryable<Voucher> voucherList = null;
                if (!string.IsNullOrEmpty(model.VoucherNumber)) voucherList = FilterVouchersByVoucherNumber(model.VoucherNumber);
                if (!string.IsNullOrEmpty(model.InvoiceNumber)) voucherList = FilterVouchersByInvoiceNumber(model.InvoiceNumber, voucherList);
                if (!string.IsNullOrEmpty(model.OrderNumber)) voucherList = FilterVouchersByOrderNumber(model.OrderNumber, voucherList);
                if (!string.IsNullOrEmpty(model.DateFrom) || !string.IsNullOrEmpty(model.DateTo)) voucherList = FilterVouchersByDates(model.DateFrom, model.DateTo, voucherList);
                if (model.SelectedSupplier != 0) voucherList = FilterVouchersBySupplier(model.SelectedSupplier, voucherList);
                if (!string.IsNullOrEmpty(model.SelectedAuthoriser)) voucherList = FilterVouchersByAuthoriser(model.SelectedAuthoriser, voucherList);
                if (model.SelectedStatus != 0) voucherList = FilterVouchersByStatus(model.SelectedStatus, voucherList);
                if (model.SelectedCapexId != 0) voucherList = FilterVouchersByCapex(model.SelectedCapexId, voucherList);
                searchVoucherList = ConvertToSearchVoucherResult(voucherList.ToList());
            }
            catch (Exception e)
            {
                LogError("Main.Ettacher.SearchVoucher. Voucher # " + model.VoucherNumber + ")",e);
            }
            return searchVoucherList;
        }

        private IQueryable<Voucher> FilterVouchersByStatus(int statusId, IQueryable<Voucher> voucherList)
        {
            voucherList = voucherList==null || !voucherList.Any()
                              ? _voucherRepository.Get(x => !x.IsDeleted && x.Status!=null && x.Status.Id == statusId)
                              : voucherList.Where(x => !x.IsDeleted && x.Status != null && x.Status.Id == statusId);
            return voucherList;
        }

        private IQueryable<Voucher> FilterVouchersByAuthoriser(string authoriserId, IQueryable<Voucher> voucherList)
        {
            if (voucherList == null || !voucherList.Any())
            {
                voucherList =
                    _voucherDocumentRepository.Get(
                            x => x.Authoriser != null && x.Authoriser.Id == authoriserId && !x.IsDeleted)
                        .GroupBy(x => x.Voucher)
                        .Select(x => x.Key);
            }
            else
            {
                var voucherIds = voucherList.Select(_ => _.Id).ToList();
                voucherList =
                    _voucherDocumentRepository.Get(
                        x =>
                            x.Authoriser != null && x.Authoriser.Id == authoriserId && !x.IsDeleted
                            && voucherIds.Contains(x.Voucher.Id)).GroupBy(x => x.Voucher).Select(x => x.Key);
            }
            return voucherList;
        }

        private IQueryable<Voucher> FilterVouchersBySupplier(int supplierId, IQueryable<Voucher> voucherList)
        {
            var supplierCode = GetSupplierCodeById(supplierId);
            voucherList = voucherList == null || !voucherList.Any()
                              ? _voucherRepository.Get(x => !x.IsDeleted && x.SupplierCode == supplierCode)
                              : voucherList.Where(x => !x.IsDeleted && x.SupplierCode == supplierCode);
            return voucherList;
        }

        private IQueryable<Voucher> FilterVouchersByOrderNumber(string orderNumber, IQueryable<Voucher> voucherList)
        {
            var oNumber = Convert.ToInt32(orderNumber);
            var orderId =
                    _orderRepository.Get(x => !x.IsDeleted && x.OrderNumber == oNumber).Select(_ => _.Id).FirstOrDefault();
            if (voucherList == null || !voucherList.Any())
            {
                if (orderId != 0)
                {
                    voucherList =
                        _voucherDocumentRepository.Get(
                            x =>
                            !x.IsDeleted
                            && x.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " ")
                            && x.Reference == orderId.ToString()).Select(x => x.Voucher);
                }
            }
            else
            {
                var voucherIds = voucherList.Select(_ => _.Id).ToList();
                voucherList =
                    _voucherDocumentRepository.Get(
                            x =>
                                !x.IsDeleted
                                && x.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " ")
                                && x.Reference == orderId.ToString() && voucherIds.Contains(x.Voucher.Id))
                        .Select(x => x.Voucher);
            }

            return voucherList;
        }

        private IQueryable<Voucher> FilterVouchersByDates(string dateFrom, string dateTo, IQueryable<Voucher> voucherList)
        {
            string[] formats = { "dd/MM/yyyy" };
            if (voucherList == null || !voucherList.Any())
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    var dateF = DateTime.ParseExact(dateFrom, formats, new CultureInfo("en-AU"), DateTimeStyles.None);
                    var dateT = DateTime.ParseExact(dateTo, formats, new CultureInfo("en-AU"), DateTimeStyles.None);
                    voucherList =
                        _voucherRepository.Get(x => x.DateCreated >= dateF && x.DateCreated <= dateT && !x.IsDeleted);
                }
                else if (!string.IsNullOrEmpty(dateFrom))
                {
                    var dateF = DateTime.ParseExact(dateFrom, formats, new CultureInfo("en-AU"), DateTimeStyles.None);
                    voucherList =
                       _voucherRepository.Get(x => x.DateCreated >= dateF && !x.IsDeleted);
                }
                else if (!string.IsNullOrEmpty(dateTo))
                {
                    var dateT = DateTime.ParseExact(dateTo, formats, new CultureInfo("en-AU"), DateTimeStyles.None);
                    voucherList =
                        _voucherRepository.Get(x => x.DateCreated <= dateT && !x.IsDeleted);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    var dateF = DateTime.ParseExact(dateFrom, formats, new CultureInfo("en-AU"), DateTimeStyles.None);
                    var dateT = DateTime.ParseExact(dateTo, formats, new CultureInfo("en-AU"), DateTimeStyles.None);
                    voucherList =
                        voucherList.Where(x => x.DateCreated >= dateF && x.DateCreated <= dateT && !x.IsDeleted);
                }
                else if (!string.IsNullOrEmpty(dateFrom))
                {
                    var dateF = DateTime.ParseExact(dateFrom, formats, new CultureInfo("en-AU"), DateTimeStyles.None);
                    voucherList =
                       voucherList.Where(x => x.DateCreated >= dateF && !x.IsDeleted);
                }
                else if (!string.IsNullOrEmpty(dateTo))
                {
                    var dateT = DateTime.ParseExact(dateTo, formats, new CultureInfo("en-AU"), DateTimeStyles.None);
                    voucherList =
                        voucherList.Where(x => x.DateCreated <= dateT && !x.IsDeleted);
                }
            }

            return voucherList;
        }

        private IQueryable<Voucher> FilterVouchersByInvoiceNumber(string invoiceNumber, IQueryable<Voucher> voucherList)
        {
            voucherList = voucherList == null || !voucherList.Any() ? _voucherRepository.Get(x => !x.IsDeleted && x.InvoiceNumber == invoiceNumber) : voucherList.Where(x=>x.InvoiceNumber == invoiceNumber);

            return voucherList;
        }

        private IQueryable<Voucher> FilterVouchersByCapex(int capexId, IQueryable<Voucher> voucherList)
        {
            var orderIdList = _orderRepository.Get(x => x.Capex_Id == capexId && !x.IsDeleted).Select(x => x.Id.ToString()).ToList();
            if (voucherList == null || !voucherList.Any())
            {
                voucherList = _voucherDocumentRepository
                                .Get(x => !x.IsDeleted && x.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " ")
                                && orderIdList.Any(y=>y==x.Reference)).Select(x => x.Voucher);
            }
            else
            {
                var voucherIdList = voucherList.Select(x => x.Id).ToList();
                voucherList = _voucherDocumentRepository
                               .Get(x => !x.IsDeleted && x.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " ")
                               && orderIdList.Any(y => y == x.Reference) && voucherIdList.Contains(x.Voucher.Id)).Select(x => x.Voucher);
            }
                            
            return voucherList;
        }

        public List<SearchVoucherResult> ConvertToSearchVoucherResult(List<Voucher> voucherList)
        {
                var list = new List<SearchVoucherResult>();

                foreach (var voucher in voucherList.OrderBy(x=>x.VoucherNumber))
                {
                try
                {
                    var item = new SearchVoucherResult()
                                   {
                                       Id = voucher.Id.ToString(),
                                       Status = voucher.Status.Name,
                                       Total = voucher.Amount,
                                       InvoiceNumber = voucher.InvoiceNumber,
                                       Supplier = GetSupplierFullNameByCode(voucher.SupplierCode),
                                       VoucherNumber = voucher.VoucherNumber.ToString(),
                                       Date = voucher.DateCreated.ToShortDateString(),
                                       Comment = voucher.Comment
                                  
                                   };
                    var vDoc = GetInvoiceDocumnetByVoucherNumber(voucher.VoucherNumber);
                    if (vDoc != null)
                    {
                        item.Authoriser = vDoc.Authoriser != null
                                              ? vDoc.Authoriser.UserInfo.FirstName + " " + vDoc.Authoriser.UserInfo.LastName
                                              : string.Empty;
                        item.AuthorisationDate = vDoc.IsAuthorised
                                                     ? vDoc.AuthorisedDate.Value.ToShortDateString()
                                                     : string.Empty;
                    }
                    list.Add(item);
                }
                catch (Exception e)
                {
                    LogError("Main.Ettacher.ConvertToSearchVoucherResult. VoucherID=" + voucher.Id + ")",e);
                }
            }
            return list;
        }

        public VoucherDocument GetInvoiceDocumnetByVoucherNumber(int voucherNumber)
        {
            return _voucherDocumentRepository.FirstOrDefault(x => x.Voucher.VoucherNumber == voucherNumber && !x.IsDeleted && (x.DocumentType.Name == DocumentTypeEnum.Invoice.ToString() || x.DocumentType.Name == DocumentTypeEnum.GRNI_Invoice.ToString().Replace("_", " ")));
        }

        private IQueryable<Voucher> FilterVouchersByVoucherNumber(string voucherNumber)
        {
            var vNumber = Convert.ToInt32(voucherNumber);
            var list = _voucherRepository.Get(x => !x.IsDeleted && x.VoucherNumber == vNumber);
            return list;
        }

        public List<VoucherStatus> GetVoucherStatuses()
        {
            return _voucherStatusRepository.Get(x => !x.IsDeleted).ToList();
        }

        public double GetOrderTotalAmount(int orderId)
        {
            var m = _orderRepository.SingleOrDefault(x => x.Id == orderId && (x.Status.Name == StatusEnum.Receipt_Partial.ToString().Replace("_", " ")
                                     || x.Status.Name == StatusEnum.Receipt_in_Full.ToString().Replace("_", " ")));
            if (m != null) return m.Total;
            return -1;

        }

        public VoucherRelatedDocumentsPanel GetVoucherRelatedDocuments(int voucherNumber)
        {
            var voucher = _voucherRepository.FirstOrDefault(x => !x.IsDeleted && x.VoucherNumber == voucherNumber);
            var model = new VoucherRelatedDocumentsPanel();
            model.RelatedDocuments = voucher != null
                                         ? GetRelateVoucherDocument(voucher)
                                         : new List<VoucherRelatedDocumentViewModel>();
            return model;
        }

        public ResubmitVoucherForm GetVoucherResubmitFormModel(int voucherNumber)
        {
            var voucher = _voucherRepository.FirstOrDefault(x => x.VoucherNumber == voucherNumber && !x.IsDeleted);
            if (voucher != null)
            {
                var model = new ResubmitVoucherForm()
                                {
                                    VoucherNumber = voucher.VoucherNumber,
                                    VoucherId = voucher.Id,
                                    SelectedAuthoriser =
                                        _voucherDocumentRepository.Get(
                                            x => x.Voucher.Id == voucher.Id && !x.IsDeleted)
                                        .Select(x => x.Authoriser.Id)
                                        .FirstOrDefault(),
                                    AuthoriserList = GetUserViewModels(),
                                    DisplayComment = voucher.Comment

                                };
                return model;
            }
            return new ResubmitVoucherForm(){AuthoriserList = new List<UserViewModel>()};
        }

        public void ResubmitVoucher(ResubmitVoucherForm model)
        {
            var newAuthoriser = _userRepository.Find(model.SelectedAuthoriser);
            var currRoutesApprover = _voucherRouteRepository.Get(x => !x.IsDeleted && x.Voucher.Id == model.VoucherId).Include(x => x.Approver).Include(x => x.Approver.User).Select(x=>x.Approver).FirstOrDefault();
            var voucher = _voucherRepository.Find(model.VoucherId);
            voucher.Status = _voucherStatusRepository.FirstOrDefault(x => x.Name == StatusEnum.Pending.ToString());
            voucher.IsDeleted = false;
            voucher.LastModifiedDate = DateTime.Now;
            voucher.LastModifiedBy = _curUser;
            voucher.Comment += CreateCommentRecordForVoucher(model.Comment, false, true);
            var documents = _voucherDocumentRepository.Get(x => x.Voucher.Id == voucher.Id && !x.IsDeleted).ToList();
            foreach (var doc in documents)
            {
                doc.Authoriser = newAuthoriser ?? doc.Authoriser;
                doc.LastModifiedDate=DateTime.Now;
                doc.LastModifiedBy = _curUser;
            }
            if (currRoutesApprover != null)
            {
                if(newAuthoriser!=null) currRoutesApprover.User = newAuthoriser;
                _approverRepository.Update(currRoutesApprover);
            }

            db.SaveChanges();
        }

        public void DeleteDocument(int documentId)
        {
            var document = _voucherDocumentRepository.Find(documentId);
            document.IsDeleted = true;
            document.LastModifiedBy = _curUser;
            document.LastModifiedDate = DateTime.Now;
            if (document.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " "))
            {
                var orderId = Convert.ToInt32(document.Reference);
                var orderStatus = _orderRepository.Get(x => x.Id == orderId).Select(x => x.Status.Name).FirstOrDefault();
                if (orderStatus == StatusEnum.Closed.ToString())//Check for previous status just if order was closed, because by attaching PO to voucher just Status = Closed available for PO
                {
                    var status = Data.GetPreviousOrderStatus(Convert.ToInt32(document.Reference));
                    if (status != null)
                        Data.ChangeOrderStatus(
                            new OrderViewModel()
                                {
                                    OrderId = Convert.ToInt32(document.Reference),
                                    SelectedStatus = status.Id
                                });
                }
            }
            db.SaveChanges();
        }

        public DeleteVoucherForm GetVoucherDeleteFormModel(int voucherNumber)
        {
            var voucher = _voucherRepository.FirstOrDefault(x => x.VoucherNumber == voucherNumber && !x.IsDeleted);
            if (voucher != null)
            {
                var model = new DeleteVoucherForm()
                {
                    VoucherNumber = voucher.VoucherNumber,
                    VoucherId = voucher.Id,
                    Amount = voucher.Amount,
                    InvoiceNumber = voucher.InvoiceNumber,
                    Supplier = voucher.SupplierCode,
                    Comment = voucher.Comment
                };
                return model;
            }
            return new DeleteVoucherForm();
        }

        public void DeleteVoucher(DeleteVoucherForm model)
        {
            var voucher = _voucherRepository.Find(model.VoucherId);
            if (voucher != null)
            {
                voucher.IsDeleted = true;
                voucher.LastModifiedDate = DateTime.Now;
                voucher.LastModifiedBy = _curUser;
                foreach (var vDoc in _voucherDocumentRepository.Get(x=>x.Voucher.Id==voucher.Id).ToList())
                {
                    vDoc.IsDeleted = true;
                    vDoc.LastModifiedDate=DateTime.Now;
                    vDoc.LastModifiedBy = _curUser;
                }
                db.SaveChanges();
            }
        }

        public List<OutstandingInvoicesReport> GetOutstandingInvoices()
        {
            //var filter = new SearchViewModel() { SelectedStatus = 1 //Pending};
            
            var list = GetNotAuthorisedVouchersDocuments(); //SearchVoucher(filter).OrderBy(x=>x.Authoriser).ToList();
            var temp = list.Where(x => x.Authoriser != null);
            var authoriserList = list.GroupBy(x => x.Authoriser).Select(x => x.Key).Where(x=>x!=null).ToList();
            var model = new List<OutstandingInvoicesReport>();

            foreach (var authoriser in authoriserList)
            {
                if(authoriser==null) continue;
                var item = new OutstandingInvoicesReport
                               {
                                   Receiver = authoriser.Email,
                                   Subject = "Invoice Authorisation Report",
                                   Invoices = new List<InvoiceReportViewModel>(),
                               };
                var invoicesList =
                    list.Where(
                        x =>
                        x.Authoriser != null && x.Authoriser.EmployeeId == authoriser.EmployeeId
                        && x.DocumentType.Name != DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " ")).ToList();
                foreach (var doc in invoicesList)
                {
                    var invoice = new InvoiceReportViewModel()
                                      {
                                          VoucherId = doc.Voucher.Id,
                                          VoucherNumber = doc.Voucher.VoucherNumber,
                                          Date = doc.Voucher.DateCreated.ToShortDateString(),
                                          AuthrisationURL = "http://viis1.oneharvest.com.au/EPOv2/Ettacher/Invoice?SelectedItem=" + doc.Voucher.VoucherNumber.ToString(),
                    };
                    var orderId =list.Where(x =>x.Voucher.Id == invoice.VoucherId
                            && x.DocumentType.Id == 2)
                            .Select(x => x.Reference)
                            .FirstOrDefault();
                    if (!string.IsNullOrEmpty(orderId))
                    {
                        var order = _orderRepository.Find(Convert.ToInt32(orderId));
                        invoice.SupplierName = Data.GetSupplierFullName(order.SupplierId);
                        invoice.OrderNumber = order.OrderNumber.ToString();
                        invoice.OrderAuthor = order.GetAuthorFullName();
                        invoice.OrderAuthoriser = string.Empty;
                        invoice.OrderStatus = order.Status.Name;
                    }
                    else
                    {
                        invoice.SupplierName = Data.GetSupplierFullName(doc.Voucher.SupplierCode);
                        invoice.OrderNumber = "---";
                        invoice.OrderAuthor = "---";
                        invoice.OrderAuthoriser = string.Empty;
                        invoice.OrderStatus = "---";
                    }
                    item.Invoices.Add(invoice);
                }
                model.Add(item);
            }

            model = DeleteEmptys(model);

            return model;
        }

        private List<OutstandingInvoicesReport> DeleteEmptys(IEnumerable<OutstandingInvoicesReport> model)
        {
            return model.Where(x => x.Invoices.Count > 0).ToList();
        }

        public void GenerateAndSendOutstandingInvoiceReport(Controller controller, string viewName, List<OutstandingInvoicesReport> model)
        {
            foreach (var report in model)
            {
                var body = RenderPartialViewToString(controller, viewName, report);
                var bodyText = PreMailer.MoveCssInline(body).Html;
                var receiver = report.Receiver;
                var subject = report.Subject;
               _output.SendReport(receiver,bodyText,subject);
            }
        }

        public bool CheckIsPOFullyMatched(int orderId)
        {
            var order = _orderRepository.Find(orderId);
            return order.Status.Name == Data.GetStatusName(StatusEnum.Receipt_in_Full);
        }

        private List<VoucherDocument> GetNotAuthorisedVouchersDocuments()
        {
            var model =
                _voucherDocumentRepository.Get(x => !x.IsDeleted && !x.IsAuthorised && !x.Voucher.IsDeleted
                && (x.DocumentType.Name!=DocumentTypeEnum.Voucher.ToString() && x.DocumentType.Id!=3 && x.Voucher.Status.Name!=StatusEnum.Declined.ToString() && x.Voucher.Status.Name!=StatusEnum.Approved.ToString() && x.Voucher.Status.Name !=StatusEnum.Closed.ToString()) )
                .Include(x=>x.Voucher).Include(x=>x.Authoriser).ToList();
            return model;
        }

        public ChangeVoucherStatusForm GetVoucherViewModelForStatusChange(int voucherNumber)
        {
            var model = new ChangeVoucherStatusForm();
            try
            {
                var voucherList = _voucherRepository.Get(x => x.VoucherNumber == voucherNumber).ToList();
                if (voucherList.Count>0)
                {
                    var voucher = voucherList.First();
                    model.VoucherNumber = voucher.VoucherNumber;
                    model.VoucherId = voucher.Id;
                    model.VoucherList = ConvertToSearchVoucherResult(voucherList);
                }
                model.StatusList = _voucherStatusRepository.Get(x => x.Name == StatusEnum.Authorised.ToString()).ToList();
            }
            catch (Exception e)
            {
                LogError("Main.Ettacher.GetVoucherViewModelForStatusChange. Voucher # " + voucherNumber,e);
            }
            return model;
        }

        public void ChangeVoucherStatus(ChangeVoucherStatusForm model)
        {
            var voucher = _voucherRepository.Find(model.VoucherId);
            if (voucher != null)
            {
                var voucherDoc =
                    _voucherDocumentRepository.Get(
                        x =>
                        x.Voucher.Id == voucher.Id && !x.IsDeleted
                        && (x.DocumentType.Name == DocumentTypeEnum.Invoice.ToString()
                            || x.DocumentType.Name == DocumentTypeEnum.GRNI_Invoice.ToString().Replace("_", " ")))
                        .FirstOrDefault();
                if(voucherDoc!=null) AuthoriseVoucherManually(voucher, voucherDoc);
            }
        }


        public void ConvertOldvouchersToNew()
        {
            var voucherlist = new List<Voucher>();
            var statuses = _voucherStatusRepository.Get().ToList();
            var currVoucherList = _voucherRepository.Get().ToList();
            var docTypes = _voucherDocumentTypeRepository.Get().ToList();
            var oldVouchersList = oldDb.vwAp_mstr_oldpo.ToList();
            var oldVoucherDocsList = oldDb.vwAp_det_oldpo.ToList();
            //var oldVoucherGRNIrefList = oldDb.vwAp_grni_rf_oldpo.Take(1000).ToList();

            foreach (var oV in oldVouchersList)
            {
                var vNumber = Convert.ToInt32(oV.voucherNumber);
                if (currVoucherList.FirstOrDefault(x => x.VoucherNumber == vNumber) != null) continue;
                {
                    var v = new Voucher()
                                {
                                    Comment = oV.userComments,
                                    InvoiceNumber = oV.invoiceNumber,
                                    VoucherNumber = Convert.ToInt32(oV.voucherNumber),
                                    DateCreated = oV.created,
                                    LastModifiedDate = oV.updated,
                                    SupplierCode = oV.supplierID,
                                    CreatedBy =
                                        oV.userID.Contains("ONEHARVEST\\")
                                            ? oV.userID.Replace("ONEHARVEST\\", "")
                                            : Ad.GetUserLoginByFullName(oV.userID),
                                    DueDate = oV.created, //don't ask why
                                    Terms = string.Empty,
                                    Account = null,
                                    CostCentre = null,
                                    Amount = 0,
                                };
                    v.LastModifiedBy = v.CreatedBy;
                    StatusEnum status;
                    switch (oV.status)
                    {
                        case 1:
                            status = StatusEnum.Authorised;
                            break;
                        case 0:
                            status = StatusEnum.Pending;
                            break;
                        case 2:
                            status = StatusEnum.Authorised;
                            break;
                        case -1:
                            status = StatusEnum.Pending;
                            break;
                        case 5:
                            status = StatusEnum.Closed;
                            break;
                        default:
                            status = StatusEnum.Closed;
                            break;
                    }
                    v.Status = statuses.FirstOrDefault(x => x.Name == status.ToString());
                    var oldRelatedDocs = oldVoucherDocsList.Where(x => x.voucherID == oV.voucherID).ToList();
                    var vDocs = new List<VoucherDocument>();
                    foreach (var oVD in oldRelatedDocs)
                    {
                        var vd = new VoucherDocument
                                     {
                                         AuthorisedDate = oVD.authorisedDate,
                                         DateCreated = oVD.created,
                                         LastModifiedDate = oVD.updated,
                                         IsAuthorised = oVD.authorisedDate != null,
                                         Voucher = v,
                                         CreatedBy =
                                             oVD.userID.Contains("ONEHARVEST\\")
                                                 ? oVD.userID.Replace("ONEHARVEST\\", "")
                                                 : Ad.GetUserLoginByFullName(oVD.userID),
                                     };
                        var docTypeId = 1;
                        switch (oVD.documentType)
                        {
                            case 1:
                                docTypeId = 1;
                                break;
                            case 3:
                                docTypeId = 2;
                                break;
                            case 11:
                                docTypeId = 3;
                                break;
                        }
                        vd.DocumentType = docTypes.FirstOrDefault(x => x.Id == docTypeId);
                        if (docTypeId != 2) vd.Reference = oVD.fileName.Replace("tif", "pdf");
                        vd.Authoriser = Ad.GetUserByFullName(oVD.authoriser);

                        _voucherDocumentRepository.Add(vd);
                    }
                }
                db.SaveChanges();
            }
           

        }

        public List<SearchVoucherResult> GetVouchersForRestamping(RestampVoucherFilter filter)
        {
            var df = Convert.ToDateTime(filter.DateFrom);
            var dt = Convert.ToDateTime(filter.DateTo).AddDays(1);
            var list =
                _voucherDocumentRepository.Get(
                    x =>
                    x.IsAuthorised && !x.IsDeleted && x.AuthorisedDate >= df && x.AuthorisedDate <= dt
                    && (x.DocumentType.Id == 1 || x.DocumentType.Id == 3)).Include(x => x.Voucher).ToList();
            var vIdList = list.Select(x => x.Voucher.Id).ToList();
            var vouchers = _voucherRepository.Get(x => vIdList.Contains(x.Id)).ToList();
            var result = ConvertToSearchVoucherResult(vouchers);
            return result;

        }

        public void RestampVouchers(RestampVoucherFilter filter)
        {
            var df = Convert.ToDateTime(filter.DateFrom);
            var dt = Convert.ToDateTime(filter.DateTo).AddDays(1);
            var list =
                _voucherDocumentRepository.Get(
                    x =>
                    x.IsAuthorised && !x.IsDeleted && x.AuthorisedDate >= df && x.AuthorisedDate <= dt
                    && (x.DocumentType.Id == 1 || x.DocumentType.Id == 3)).Include(x => x.Voucher).ToList();
            foreach (var doc in list)
            {
                _output.AuthoriseInvoiceFile(voucherDoc:doc);
            }

        }
    }
}