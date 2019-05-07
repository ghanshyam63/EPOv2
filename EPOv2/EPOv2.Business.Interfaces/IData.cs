namespace EPOv2.Business.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Reflection;
    using System.Threading.Tasks;

    using DomainModel.DataContext;
    using DomainModel.Entities;
    using DomainModel.Enums;

    using EPOv2.ViewModels;
    using EPOv2.ViewModels.Interfaces;

    using Intranet.ViewModels;

    public interface IData
    {
        IAd _ad { get; set; }

        string GetSupplierFullName(string supplierCode);

        string capexFilesPath{get;}

        //void DeleteExpiredSubstitutionsEveryday();
        string GetSupplierFullName(int supplierId);

        OwnerViewModel GetOwnerViewModel(CostCentre cc, vRockyEmployees employeesData);

        string GetAuthoriser(int orderId);

        AdminStatsViewModel GetAdminStats();

        ReportEPOAccuralsFilterViewModel GetReportEPOAccuralsFilterViewModel();

        RoutingStructureElement GetRSElement(string empId, int pseudoLevel, vRockyEmployees employeesData);

        List<EPOAccuralReportViewModel> RunEPOAccuralsReport(ReportEPOAccuralsFilterViewModel filter);

        CapexCRUDViewModel GetCapexCRUDViewModel(int selectedItem, CapexAction action );

        CapexCRUDViewModel GetCapexCRUDViewModel();

        CapexCompanyBox GetCapexCompanyBox(int entityId);

        string GetCapexReference(string capexId);

        void SaveCapexApprover(CapexApproverCRUDViewModel model);

        CapexApproverCRUDViewModel GetCapexApproverCRUDViewModel();

        List<DivsionViewModel> GetDivisionViewModelList();

        CapexApproverCRUDViewModel GetCapexApproverCRUDViewModel(int id);

        List<CapexApprover> GetCapexApproversList();
        CalendarEventCRUDViewModel GetCalendarEventCRUDViewModel(int id);
        List<Division> GetDivisions();

        DivisionCRUDViewModel GetDivisionCRUDViewModel();

        void SaveDivision(DivisionCRUDViewModel model);

        DivisionCRUDViewModel GetDivisionCRUDViewModel(int id);

        List<EntityViewModel> GetEntityViewModelList();

        EntityViewModel GetEntityViewModelById(int id);

        void SaveEntity(EntityViewModel passModel);

        void SaveEntity(Entity model);

        void DeleteOrActivateEntity(int id, bool toDelete = true);

        EntityDDLViewModel GetEntityDDLVM();

        List<CostCentreToEntityViewModel> GetCostCentreToEntityViewModelList(int entityId);

        void DeleteOrActivateCostCentreToEntityMap(int id, bool toDelete);

        CostCentreToEntityAdding GetCostCentreVMFilteredList(int entityId);

        void SaveCostCentreToEntityMapping(CostCentreToEntityAdding model);

        CostCentre GetCostCentre(int id);

        void SaveCostCentre(CostCentre passModel);

        void DeleteOrActivateCostCentre(int id, bool toDelete = true);

        Account GetAccount(int id);

        void SaveAccount(Account passModel);

        void DeleteOrActivateAccount(int id, bool toDelete=true);

        List<AccountToCostCentreViewModel> GetAccountToCCViewModelList();

        List<AccountToCostCentreViewModel> GetAccountToCCViewModelList(int costCentreId);

        List<AccountViewModel> GetAccountViewModelList(int type=0);

        AccountToCostCentreAdding GetAccountVMFilteredList(int costCentreId, int type = 0);

        CostCentreDDLViewModel GetCostCentresDDLVM();

        void DeleteOrActivateAccountToCCMap(int id,bool toDelete);

        AccountToCostCentreAdding GetAccountToCCAddingViewModel();

        void SaveAccountToCCMapping(AccountToCostCentreAdding model);

        SubstituteApproverViewModel GetSubstituteApproverViewModel();

        void SaveSubstituteApprover(SubstituteApproverViewModel model);

        void ApplySubstitution(SubstituteApprover substitute);

        List<SubstituteApproverTableViewModel> GetSubstituteApproverTableVMList(string approverId);

        void DeleteSubstitution(int sustitutionId);

        void CancelSubstitution(SubstituteApprover substitute);

        void CheckForSubstitution(Order order);

        SubstituteApprover CheckForCascadeSubstitute(SubstituteApprover subsIn);

        List<Approver> GetActiveApproverList(int id);

        List<UserViewModel> GetUserViewModelList();

        List<IUserViewModel> GetUserViewModelDdList();

        UserEditViewModel GetUserEditViewModel(string id);

        void DeleteOrActivateReceiptGroup(int id, bool toDelete = true);

        ReceiptGroupViewModel GetReceiptGroupViewModel(int groupId);

        OrderViewModel GetOrderViewForReassignByTempOrder(string tempNumber);

        void ReassignOrder(OrderViewModel model);

        void SaveReceiptGroup(ReceiptGroupViewModel model);

        void SaveUser(UserEditViewModel model);

        List<OrderItemKitViewModel> GetItemKitList();

        void DeleteOrActivateItemKit(int id, bool toDelete = true);

        OrderItemKitCRUDViewModel GetOrderItemKitCRUD(int id);

        OrderItemKitViewModel GetOrderItemKit(int id);

        void SaveOrderItemKit(OrderItemKitCRUDViewModel model);

        UserOrderSettingsViewModel GetUserOrderSettingsViewModel(string id);

        void SaveUserOrderSettings(UserOrderSettingsViewModel model);

        int curEmpId { get; set; }

        UserTilesForDashboard _userTilesForDashboard { get; set; }

        void FetchEntity();

        void FetchCC();

        /// <summary>
        /// Fetch Accounts & SubAccounts
        /// </summary>
        void FetchAccount();

        void FetchDeliveryAddress();

        void FetchReceiptGroup();

        void FetchCcToEntity();

        void FetchAccountToCc();

        void FetchSubAccountToCc();

        void SaveState(State model);

        void SaveCurrency(Currency model);

        List<State> GetStates();

        List<Currency> GetCurrencies();

        /// <summary>
        /// Get Active Entities. Sorted by CodeNumber
        /// </summary>
        /// <returns></returns>
        List<Entity> GetEntities();

        List<CostCentre> GetCostCentres();

        List<CostCentre> GetAllCostCentres();

        List<CostCentreViewModel> GetCostCentresVM(bool isFullList =false);

        List<CostCentreViewModel> GetCostCentresWithOwnerVM(bool isFullList = false);

        List<CostCentreViewModel> GetCostCentresVM(int entityId);

        List<Account> GetAccounts();

        List<Account> GetAccountsAll();

        List<Account> GetCCAccountsByType(int ccId,int type = 0);

        Order GetOrder(int orderId);

        List<tblCostCentre> GetOldCostCentres();

        List<tblEntity> GetOldEntities();

        List<tblAccount> GetOldAccounts();

        List<tblSubAccount> GetOldSubAccounts();

        List<DeliveryAddress> GetDeliveryAddresses();

        List<tblSupplier> GetSuppliers();

        List<tblSupplier> GetSuppliers(int entityCode);

        tblSupplier GetSupplier(int supplierId);

        List<Group> GetGroups();

        List<Group> GetGroups_all();

        List<Status> GetStatuses();

        void SaveStatus(Status model);

        List<Level> GetLevels();

        void SaveLevel(Level model);

        void FetchCCOwners();

        List<tblCapex> GetCapexes();

        List<Capex> GetApprovedCapexes();

        CostCentre GetCostCentreForCapex(int capexId);

        List<CapexViewModel> GetCapexViewModels(bool isEmpty, bool fullList = false);

        List<Capex> GetAllCapexes();

        List<tblCapex> GetFullListCapexesFromOldSystem();

        void FillAuthorId();

        string GetCostCentreOwner(int ccId);

        CostCentreOwnerViewModel GetCostCentreOwnerVM();

        bool SaveCostCentreOwner(CostCentreOwnerViewModel model);

        string GetUserFullName(string id);

        Status GetStatus(StatusEnum status);

        string GetStatusName(StatusEnum status);

        List<VoucherDocumentType> GetVoucherDocumentTypeList();

        void SaveVoucherDocumentType(VoucherDocumentType model);

        VoucherDocumentType GetVoucherDocumentType(int id);

        void DeleteOrActivateVoucherDocumentType(int id, bool toDelete = true);

        List<VoucherStatus> GetVoucherStatusList();
        UserDashboardSettingsViewModel GetUserDashboardSettingsViewModelForCurrentUser(bool loadRequiredTiles, bool loadAvailableTiles=false);

        List<DTileShowroomViewModel> LoadMyTiles(UserDashboardSettings userSettings);

        List<DTileShowroomViewModel> LoadMyTiles();

        List<DTileShowroomViewModel> LoadAvailableTiles(List<int> myTileIds, List<int> reqTileIds);

        VoucherStatus GetVoucherStatus(int id);

        void SaveVoucherStatus(VoucherStatus model);

        void DeleteOrActivateVoucherStatus(int id, bool toDelete = true);
       
        VoucherDocument GetVoucherDocument(int voucherDocumentId);

        EpoDashboardViewModel GetEPODashboardViewModel(int currentEmployeeId);

        CasefillDashboardViewModel GetCasefillDashboardViewModel(DateTime addDays);

        List<MatchOrder> GetMatchingList(Order order);

        List<CalendarEventViewModel> GetCalendarEventsViewModelFullList();

        CalendarEventCRUDViewModel GetCalendarEventCRUDViewModel();

        void SaveDashboardCalendarEvent(CalendarEventCRUDViewModel model);

        List<DCalendarEventType> GetCalendarEventTypeList();

        List<ReceiptGroupMemberViewModel> GetGroupMembers(int groupId);

        void SaveDashboardCalendarEventType(DCalendarEventType model);

        DCalendarEventType GetCalendarEventType(int id);

        void DeleteOrActivateReceiptGroupMember(int id, bool toDelete = true);

        void DeleteOrActivateCalendarEvent(int id, bool toDelete = true);

        DataTable MDxQueryRun(string connectionString, string query);

        string GenerateMDXQueryCasefillByDay(DateTime date);

        string GenerateMDXQueryCasefillTrend(DateTime dateFrom, DateTime dateTo);

        string GenerateMDXQueryCasefillDetail(DateTime date);

        string GenerateMDXQueryDateRangeYield(DateTime startDate, string[] codeList);

        List<CostCentreViewModel> CheckCostCentreOwners();

        OrderViewModel GetOrderViewForStatusChange (int orderNumber);

        void ChangeOrderStatus (OrderViewModel model);

        List<AccountCategory> GetAccountCategoryList();

        void SaveAccountCategory(AccountCategory model);

        void DeleteOrActivateAccountCategory(int id, bool toDelete=true);

        AccountCategory GetAccountCategory(int id);

        List<AccountToCategoryMatrixViewModel> GetAccountToCategoryMappingList();

        AccountToCategoryAdding GetAccountToCategoryAddingViewModel(int categoryId);

        AccountCategoryDLLViewModel GetAccountCategoryDDL();

        void SaveAccountToCategoryMap(AccountToCategoryAdding model);

        void DeleteOrActivateAccountToCategory(int id, bool toDelete=true);

        BudgetReportEntryDataViewModel GetBudgetReportEntryDataViewModel(int entityId=0, int costCentreId=0, int categoryId=0);

        BudgetReportResult GetBudgetReport(BudgetReportEntryDataViewModel filter);

        BudgetReportResult GetReForecastAndVarianceForBudgetReport(BudgetReportResult inputData);

        Entity GetEntity(int id);

        Group GetGroup(int id);

        DeliveryAddress GetDeliveryAddress(int id);

        CapexViewModel GetCapexViewModelForStatusChange(string capexNumber);

        void ChangeCapexStatus(CapexViewModel model);

        void GenerateOrderLog(Order order, OrderLogSubject subject);

        void GenerateOrderLog(int orderId, OrderLogSubject subject);

        Status GetPreviousOrderStatus(int orderId);

        Status GetPreviousOrderStatus(Order order);

        void CheckForSubstitutionForVoucher(Voucher voucher);

        List<Approver> GetActiveVoucherApproverList(int voucherId);

        void ApplyInvoiceSubstitution(SubstituteApprover substitute);

        User CheckAuthoriserForSubstitution(string userId);

        void CancelSubstitutionForVoucher(SubstituteApprover substitute);

        void ApplyCapexSubstitution(SubstituteApprover substitute);

        void ReApplySubstitutions();

        string GenerateMDXQueryAttainmentToPlan(DateTime date);

        string GenerateMDXQueryRightFirstTime(DateTime date);

        DashboardNotificationDataViewModel GetEPODashboardNotificationData(int currentEmployeeId);

        TnADashboardTiles GetTNADashboardViewModel(int currentEmployeeId);

        string GenerateMDXQueryYield(DateTime date, string[] codeList);

        List<DNews> GetDashboardNews();

        void SaveDashboardNews(NewsCRUDViewModel model);

        void DeleteOrActivateNews(int id, bool b=true);

        NewsCRUDViewModel GetNews(int id);

        void PublishOrUnpublishNews(int id, bool toPublish=true);

        OrderViewModel GetOrderViewForChangeAuthor(int orderNumber);

        void ChangeOrderAuthor(OrderViewModel model);

        List<DTile> GetDashboardTiles(bool fullList);

        void SaveDTile(DTileCRUDViewModel model);

        List<DUserGroup> GetDashboardUserGroups();

        void SaveDUserGroup(DUserGroupCRUDViewModel model);

        DUserGroupCRUDViewModel GetDashboardUserGroupCRUDViewModel(int id);

        void DeleteOrActivateDUserGroup(int id, bool toDelete = true);

        DTileCRUDViewModel GetDashboardTileCRUDViewModel(int id);

        void DeleteOrActivateDTile(int id, bool toDelete = true);

        string SaveMyTiles(List<int> tileIds);

        bool HasUserAnyPresetTilesForDashboard();

        EpoTile GetEPOTile(int type, int employeeId);

        EpoTile InitEpoTileData(DTile tile);

        EpoTile GetEPOTilebySubType(TileSubType subType, int employeeId);
        TnATile InitTnaTileData(DTile tile);

        CasefillDashboardViewModel InitCasefillTileData(DTile tile);

        SmsTileViewModel InitSmsTileData(DTile tile);

        //int WeekOfYearISO8601(DateTime date);
        UnrecieptedPOReportViewModel GetUnrecieptedPO(DateTime today);

        Dictionary<string, string> GetPeriodEPODetailsForBudgetReport(string glCode, int period, int entity, int cc, bool isEstimate = true);
        Dictionary<string, string> GetPeriodEPODetailsForBudgetReport(int categoryId, int period, int entity, int cc);

        Dictionary<string, string> GetPeriodVoucherDetailsForBudgetReportByCategory(int categoryId, int period, int entity, int cc);

        BudgetWithVoucherReportResult GetBudgetWithVouchersReport(BudgetReportEntryDataViewModel filter);

        int GetCorrectPeriod(int calendarPeriod);

    }

    public enum CapexAction { New, View, Edit, Delete, Authorise };
    public enum StatusEnum { Draft, Pending, Approved, Authorised, Declined, Revised, Transmitted,
        [Description("Receipt in Full")]
        Receipt_in_Full,
        [Description("Receipt Partial")]
        Receipt_Partial, Invoiced, Cancelled, Closed };
    public enum FunctionParam { OAP, OMA, OMY, IAP,CAP };

    public enum OrderLogSubject {Ettacher, EPO, Manual, Capex, Unknown, EPOMatching }




}