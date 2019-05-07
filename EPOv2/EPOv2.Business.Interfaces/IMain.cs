namespace EPOv2.Business.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;

    using DomainModel.DataContext;
    using DomainModel.Entities;

    using EPOv2.ViewModels;

    using Intranet.ViewModels;

    public interface IMain
    {
        /// <summary>
        /// Brand New Order
        /// </summary>
        /// <returns></returns>
        NewPOViewModel GetNewPoViewModel();

        IDataContext db { get; set; }
        int CurEmpId { get; set; }
        ControllerContext controllerContext { get; set; }
        /// <summary>
        /// Load existing PO
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderAction"></param>
        /// <param name="isMatching">is that view model for Matching order</param>
        /// <param name="isChangedByRevision"></param>
        /// <returns></returns>
        NewPOViewModel GetExistingPoViewModel(int orderId, OrderAction orderAction, bool isMatching=false, bool isChangedByRevision= false);

        List<UserViewModel> GetUserViewModels();

        Order RaiseOrder();

        List<int> GetSubUsersEmployeeId(int curEmpId);

        List<OrderItemTableViewModel> GetItemTableViewModelList(Order order);

        List<OrderItemTableViewModel> GetItemMatchTableViewModelList(Order order);

        OrderItemTableViewModel GetOrderItemTableViewModel(OrderItem m);

        OrderItemTableViewModel GetOrderItemMatchTableViewModel(OrderItem m);

        CompanyBoxViewModel GetCompanyBoxViewModel();

        CompanyBoxViewModel GetCompanyBoxViewModel(int entityId, int ccId =0, int capexId=0, int groupId=0, bool isBlocked=false);

        DeliveryBoxViewModel GetDeliveryBoxViewModel();

        DeliveryBoxViewModel GetDeliveryBoxViewModel(int deliveryId, string userID="",bool isBlocked=false);

        SupplierBoxViewModel GetSupplierBoxViewModel(bool doLoad=true);

        SupplierBoxViewModel FilterSuppliersViewModel(int entityId);

        SupplierBoxViewModel GetSupplierBoxViewModel(int supplierId,int entityId, string email,string method="",bool isBlocked=false);

        List<SupplierViewModel> GetSupplierViewModels();

        List<SupplierViewModel> GetSupplierViewModels(int entityCode);

        /// <summary>
        /// Generate new Order Item
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        OrderItemViewModel GetOrderItemViewModel(NewPOViewModel m);

        /// <summary>
        /// Prepare View Model for existing Item
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        OrderItemViewModel GetOrderItemViewModelForEdit(NewPOViewModel m);

        List<AccountViewModel> GetAccountViewModels(int ccId, int type = 0);

        void DeleteOrderItem(NewPOViewModel m);

        /// <summary>
        /// Pre-Save Order
        /// </summary>
        /// <param name="inputModel"></param>
        void PreSaveOrder(NewPOViewModel inputModel);

        Order SaveOrder(NewPOViewModel model);

        int SaveOrderItem(OrderItemViewModel m);

        OrderItem CreateOrderItemFromViewModel(OrderItemViewModel m);

        List<Order> GetOrderList();

        void DeleteOrder(NewPOViewModel model);

        void DeleteItem(OrderItemTableViewModel item);

       

        List<Order> GetOrdersByAuthoriser(string authoriserId);

        void SaveMatchOrder(NewPOViewModel model);

        bool CheckOrderItemsQty(int orderId);

        List<AccountViewModel> GetAccountViewModels_All();

        SearchEPOResult SearchEPO(SearchViewModel model);

        //List<SearchEPOResultItem> ConvertToSearchEPOResult(List<Order> orderList);

        //SearchEPOResultItem ConvertToSearchEPOResult(Order order);

        string GetSupplierFullNameByOldId(int oldId);

        string GetSupplierFullNameByCode(string code);

       // void Dispose();

        void CancelOrder(int orderId);

        void GenerateAndSendCancelledOrder(Order order);

        IEnumerable<ItemKitDdlViewModel> GetItemKitDdl(int accountId, int orderId);

        void SaveOrderItemLog(int itemId);

        void SaveOrderItemLog(OrderItemViewModel model);

        void SaveOrderItemLog(OrderItem unChangedModel);

        VoucherPanelViewModel GetVoucherPanelViewModel();

        /// <summary>
        /// Supplier list from QADLive(vsql1) based on active/closed vouchers
        /// </summary>
        /// <param name="isShowConfirmed"></param>
        /// <param name="isFullList"></param>
        /// <returns></returns>
        List<SupplierViewModel<string>> GetSuppliersFromQADBasedOnVouchers(bool isShowConfirmed = false, bool isFullList = false);

        VoucherInfoViewModel GetVoucherInfoViewModel(int voucherNumber);

        VoucherAttachmentPanelViewModel GetAttachmentPanelViewModel(bool loadEPO = true, string supplierCode = null);

        List<VoucherFileInvoiceViewModel> GetVoucherFileInvoiceViewModelList();

        List<VoucherFileInvoiceViewModel> GetEPOAsFileInvoiceViewModelList(string supplierCode=null);

        List<FileInfo> GetFileInvoiceList();

        VoucherAttachingFormViewModel GetVoucherAttachingForm(int voucherNumber);

        List<VoucherDocumentType> GetVoucherDocumentTypes();

        int GetMaxPageOFFile(string fileName);

        ReturnResutViewModel SaveVoucherAttachForm(VoucherAttachingFormViewModel model);

        InvoicePageViewModel GetInvoiceForAuthorisation(int voucherNumber);

        InvoicePageViewModel GetInvoiceDetails(int voucherId);

        List<VoucherRelatedDocumentViewModel> GetRelateVoucherDocument(Voucher voucher);

        List<OrderItemTableViewModel> GetItemTableViewModelListForInvoicePage(Order order);
        Order GetOrderToModifyDueDate(int orderId);
        Order SaveOrderToModifyDueDate(Order order);
        void AuthoriseInvoice(InvoicePageViewModel model);

        DocumentTypeEnum GetVoucherDocumentType(int voucherDocumentId);

        string GetOrderAuthoriserId(int orderId);

        List<Voucher> FindRelatedVoucher(string orderId, int voucherId);

        VoucherGRNIInfoViewModel GetVoucherGRNIInfoViewModel(int voucherNumber);

        List<SearchVoucherResult> SearchVoucher(SearchViewModel model);

        VoucherDocument GetInvoiceDocumnetByVoucherNumber(int voucherNumber);

        List<VoucherStatus> GetVoucherStatuses();

        double GetOrderTotalAmount(int orderId);

        VoucherRelatedDocumentsPanel GetVoucherRelatedDocuments(int voucherNumber);

        ResubmitVoucherForm GetVoucherResubmitFormModel(int voucherNumber);

        void ResubmitVoucher(ResubmitVoucherForm model);

        void DeleteDocument(int documentId);

        DeleteVoucherForm GetVoucherDeleteFormModel(int voucherNumber);

        void DeleteVoucher(DeleteVoucherForm model);

        Capex SaveCapex(CapexCRUDViewModel model);

        void DeleteCapex(int capexId);

        void StartRouting(Capex capex);

        void DeleteExistingCapexRouting(Capex capex);

        List<CapexTransaction> GetCapexTransactions(int CapexId);

        void TestMethod();

        void CloseOrder(int orderId);

        List<OutstandingInvoicesReport> GetOutstandingInvoices();

        void GenerateAndSendOutstandingInvoiceReport(Controller controller, string viewName, List<OutstandingInvoicesReport> model);

        void GenerateAndSendCCOwnerReport(
            Controller controller,
            string viewName,
            List<CostCentreViewModel> model,
            string receiver);

        string RenderPartialViewToString<T>(Controller controller, string viewName, T model);

        void SendCapexApproveNotification(int capexId, Controller controller);

        List<CalendarEventViewModel> GetDashboardCalendar();

        List<SearchCapexResult> SearchCapex(SearchViewModel model);

        CasefillDashboardSecondLevelViewModel GetCasefillDashboardSecondLevelViewModel(DateTime date, string connectionString);

        CasefillDashboardSecondLevelViewModel GetCasefillDashboardTrendViewModel(
            DateTime dateFrom,
            DateTime dateTo,
            string connectionString);

        CasefillDashboardSecondLevelViewModel GetCasefillDashboardDetailViewModel(DateTime date, string connectionString);

        bool CheckIsPOFullyMatched(int orderId);

        List<SearchVoucherResult> GetAuthorisedVouchersReport(List<SearchVoucherResult> model);

        void SendCapexNotificationToAuthor(int capexId, Controller capexController);

        void SavaDefaultOrderSettings(DefaultOrderSettingsViewModel defaultOrderSettingsViewModel);

        ChangeVoucherStatusForm GetVoucherViewModelForStatusChange(int voucherNumber);

        void ChangeVoucherStatus(ChangeVoucherStatusForm model);

        void LogError(string msg, Exception exception);

        InvoiceExceedReportVM GetInvoiceExceedReport(InvoiceExceedReportFilterVM filter);

        VoucherDocument CreateUpdateVoucherDocument(
            VoucherAttachingFormViewModel model,
            string fileName,
            Voucher voucher);

        string GetVoucherDocumentTypeName(DocumentTypeEnum type);

        void SetAuthoriserForVoucherAndChangeStatus(Voucher voucher, List<VoucherDocument> vDocList, string comment="");

        AttainmentToPlanDashboardViewModel GetAttainmentToPlanDashboardViewModel(DateTime date);

        RightFirstTimeDashboardViewModel GetRightFirstTimeDashboardViewModel(DateTime date);
        List<YieldGraphTile> GetYieldDashboardData();

        List<NewsViewModel> GetDashboardNews_Published();

        UserTilesForDashboard GetUserDashboard();

        void ConvertOldvouchersToNew();

        List<SearchVoucherResult> GetVouchersForRestamping(RestampVoucherFilter filter);

        void RestampVouchers(RestampVoucherFilter filter);
    }

    public enum OrderAction { New, View, Edit, Delete, Authorise, AfterSaved, AfterDeleted, Matching }

    public enum DocumentTypeEnum { Invoice, Purchase_Order, GRNI_Invoice, Voucher, Purchase_Order_Scan }
}