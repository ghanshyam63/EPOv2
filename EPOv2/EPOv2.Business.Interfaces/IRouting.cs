namespace EPOv2.Business.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using DomainModel.DataContext;
    using DomainModel.Entities;

    using EPOv2.ViewModels;

    public interface IRouting
    {
        IDataContext Db { get; set; }
        ControllerContext ControllerContext { get; set; }
        bool IsRevision { get; set; }

        int CurEmpId { get; set; }

        string RoutingResult { get; set; }
        List<ApproverViewModel> ApproverListVM { get; set; }
        void DeclineCapex(CapexCRUDViewModel model);

        void ApproveCapex(CapexCRUDViewModel model);

        void MakeCapexStatusApproved(CapexCRUDViewModel model);

        Capex MakeCapexStatusDeclined(CapexCRUDViewModel model);

        bool CheckIsLastCapexApprover(CapexCRUDViewModel model);

        RoutingStructureForCCViewModel GetRoutingStructureForCC(int ccId);
        bool CheckAttachedDocumentStatus(int DocumentTypeId);

        ReturnResutViewModel TryToStartVoucherRouting(VoucherAttachingFormViewModel model);

        RoutingStructureForCCViewModel PrepareRoutingStructureForView(List<string> rs, OwnerViewModel ccOwner);
        /// <summary>
        /// prepare for Add/Update routes
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="employee">Need just for TYPE=0</param>
        /// <param name="level"></param>
        /// <param name="type">2 - create routing for employees who staing ABOVE this Manager; 1 - create routing for employees who staing UNDER this manager; 0 - ??? </param>
        /// <param name="costCentre"></param>
        //void PushRouting(string manager, string  employee, int level, int type = 0, tblStagingCostCentresDEV costCentre = null);

        /// <summary>
        /// Add/Update routing - main procedure
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="costCentresLst"></param>
        /// <param name="limit"></param>
        /// <param name="approvalType"></param>
        /// <param name="approvalNumber"></param>
        //void MakeRouting(string employee, IEnumerable<tblStagingCostCentresDEV> costCentresLst, int limit,int approvalType = 1, int approvalNumber = 1);

        //int RemoveOldRouting(tblStagingCostCentresDEV stagingCostCentre);

        //int RemoveOldRouting(List<string> accountList, tblStagingCostCentresDEV stagingCostCentre);

        //int RemoveRouting(IEnumerable<int> entityCCAccLst);

        /// <summary>
        /// Delele user from StagingEmployees and Delete all routes for this Employee
        /// </summary>
        /// <param name="stagingEmployee"></param>
        /// <returns></returns>
        //Task<bool> RemoveUser(tblStagingEmployeesDEV stagingEmployee);

        /// <summary>
        /// Refresh All routes by Staging Account CostCentre.  PushRouting -> PushRouting -> MakeRouting
        /// </summary>
        //void PushRouting(tblStagingCostCentresDEV stagingCostCentre, List<string> accountList = null);

        /// <summary>
        /// Insert new record to tblStagingCostCentres.
        /// If StagingCostCentre exist than return existing odject
        /// </summary>
        /// <param name="costCenteCode"></param>
        /// <returns>ID of record, even it's already exist</returns>
        //Task<tblStagingCostCentresDEV> AddStagingCostCentreAsync(int costCenteCode);

        //void PushFakeRouting(FakeRouting fakeRouting);

        //void RemoveNotRelevantRouting(tblStagingEmployeesDEV stagingEmployee);

        //List<string> GetEmployeeChainList(string manager, Direction direction);

        //string GetADName();

        //List<string> GetAllADUsersName();

        //string GetADUserLoginbyName(string userName);

        //SelectList GetLevelSelectList();

        //SelectList GetLevelSelectList(int selectedVal);

        //Task<List<string>> GetManagerLst();

        //MultiSelectList GetAccountList();

        /// <summary>
        /// Get Cost Centre from original table
        /// </summary>
        /// <param name="active"></param>
        /// <returns></returns>
        //SelectList GetCostCentreList(bool active);

        //List<tblStagingCostCentresDEV> GetCostCentreList(string manager);

        //IEnumerable<tblStagingAccCC> GetAccCCsList(int stagingCCID);

        //SelectList GetCostCentreList();

        //FakeRouting FakeRoutingInit();

        //Order GetFirstScenarioOrder();

        //Order GetSecondScenarioOrder();

        //Order GetThirdScenarioOrder();

        //Order GetFourthScenarioOrder();

        //Order GetFifthScenarioOrder();

        bool Start(Order order);

        bool isOrderKit(Order order);

        bool CheckOrderKitPermission(AuthorViewModel author);

        AuthorViewModel GetAuthorViewModel(Order order, int level);

        OwnerViewModel GetOwnerViewModel(Order order, int level);

        vRockyEmployees GetRockieLevelData(string empId);

        /// <summary>
        /// Compare owner level with order $$
        /// </summary>
        /// <param name="order"></param>
        /// <param name="owner"></param>
        /// <returns>1 - Greater; 0 - Less</returns>
        int CompareOwnerLevelWithPO(Order order, OwnerViewModel owner);

        void GreaterWay(OwnerViewModel owner, Order order, ReportingStructure rs );

        bool LessWay(AuthorViewModel author, OwnerViewModel owner, Order order );

        /// <summary>
        /// Getting list of Employer ID of Repost Structure for the CC
        /// </summary>
        /// <param name="ownerEmpId"></param>
        /// <returns></returns>
        List<string> GetRSForCC(int ownerEmpId);

        List<string> GetRSForCCDetailed(int ownerEmpId);

        List<Route> GetRouteForOrder(int orderId);

        List<Approver> GetApproverList(AuthorViewModel author, OwnerViewModel owner, Order order);

        void StartApproveOrder(NewPOViewModel model);

        void SetOrderNumber(Order order);

        void SendEmail(Order order);

        bool ApproveOrder(Order order);

        void Dispose();

        void SetAuthoriserToOrder(int orderId, string approverId);

       // void SetAuthoriserForVoucherAndChangeStatus(Voucher voucher, List<VoucherDocument> vDocList);
    }
    public enum Direction { Up, Down };
    public enum ReportingStructure { Within, Outside };
}