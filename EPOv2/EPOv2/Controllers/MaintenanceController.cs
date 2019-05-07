using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace EPOv2.Controllers
{
    using System;

    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.ViewModels;

    public class MaintenanceController : Controller
    {
        private readonly IData _data;

        private readonly IUserManagement _userManagement;

        private readonly IAd _ad;

        private readonly IMain _main;

        public MaintenanceController(IData data, IUserManagement userManagement, IAd ad, IMain main)
        {
            _data = data;
            _userManagement = userManagement;
            _ad = ad;
            _main = main;
        }
      
        // GET: Maintenance
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageEntity()
        {
            //var data = new Data();
            var model = _data.GetEntityViewModelList();
            return View(model);
        }

        [HttpGet]
        public ActionResult EditEntity(int id)
        {
            var model = _data.GetEntityViewModelById(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditEntity(EntityViewModel model)
        {
            _data.SaveEntity(model);
            return RedirectToAction("ManageEntity");
        }

        [HttpGet]
        public ActionResult AddEntity()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEntity(Entity model)
        {
            _data.SaveEntity(model);
            return RedirectToAction("ManageEntity");
        }

        public ActionResult ActivateEntity(int id)
        {
            _data.DeleteOrActivateEntity(id,false);
            return RedirectToAction("ManageEntity");
        }

        public ActionResult DeleteEntity(int id)
        {
            _data.DeleteOrActivateEntity(id);
            return RedirectToAction("ManageEntity");
        }

        #region CostCentre
        public ActionResult ManageCostCentre()
        {
            var model = _data.GetAllCostCentres();
            return View(model);
        }

        public ActionResult CreateCostCentre()
        {
            var model = new CostCentre();
            ViewBag.Action = "Create";
            return View("EditCostCentre", model);
        }

        public ActionResult EditCostCentre(int id)
        {
            var model = _data.GetCostCentre(id);
            ViewBag.Action = "Edit";
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateCostCentre(CostCentre model)
        {
            _data.SaveCostCentre(model);
            return RedirectToAction("ManageCostCentre");
        }

        [HttpPost]
        public ActionResult EditCostCentre(CostCentre model)
        {
            _data.SaveCostCentre(model);
            return RedirectToAction("ManageCostCentre");
        }

        public ActionResult ActivateCostCentre(int id)
        {
            _data.DeleteOrActivateCostCentre(id, false);
            return RedirectToAction("ManageCostCentre");
        }

        public ActionResult DeleteCostCentre(int id)
        {
            _data.DeleteOrActivateCostCentre(id);
            return RedirectToAction("ManageCostCentre");
        }
        #endregion

        #region Cost Centre Owner
        public ActionResult ChangeCostCentreOwner()
        {
            var model = _data.GetCostCentreOwnerVM();
            return View(model);
            // return null;
        }

        [HttpPost]
        public ActionResult ChangeCostCentreOwner(CostCentreOwnerViewModel model)
        {
            var result = _data.SaveCostCentreOwner(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FetchCostCentreOwner(int ccId)
        {
            var result = _data.GetCostCentreOwner(ccId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Account
        public ActionResult ManageAccount()
        {
            var model = _data.GetAccountsAll();
            return View(model.OrderBy(x => x.Type).ThenBy(x => x.Code).ToList());
        }

        public ActionResult CreateAccount()
        {
            var model = new Account();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateAccount(Account model)
        {
            _data.SaveAccount(model);
            return RedirectToAction("ManageAccount");
        }

        public ActionResult EditAccount(int id)
        {
            var model = _data.GetAccount(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditAccount(Account model)
        {
            _data.SaveAccount(model);
            return RedirectToAction("ManageAccount");
        }

     
        public ActionResult ActivateAccount(int id)
        {
            _data.DeleteOrActivateAccount(id,false);
            return RedirectToAction("ManageAccount");
        }

       
        public ActionResult DeleteAccount(int id)
        {
            _data.DeleteOrActivateAccount(id);
            return RedirectToAction("ManageAccount");
        }

        #endregion

        #region Mapping Account to Cost Centre

        public ActionResult MapAccountToCC()
        {
            var model = _data.GetCostCentresDDLVM(); 
            return View(model);
        }

        public ActionResult FetchAccountCCMap(int ccId)
        {
            var model = _data.GetAccountToCCViewModelList(ccId);
            return PartialView("_AccountToCCMap",model);
        }

        public ActionResult DeleteAccountCCMap(int ccId, int id)
        {
            _data.DeleteOrActivateAccountToCCMap(id, true);
            var model = _data.GetAccountToCCViewModelList(ccId);
            return PartialView("_AccountToCCMap", model);
        }

        public ActionResult ActivateAccountCCMap(int ccId, int id)
        {
            _data.DeleteOrActivateAccountToCCMap(id, false);
            var model = _data.GetAccountToCCViewModelList(ccId);
            return PartialView("_AccountToCCMap", model);
        }

        [HttpGet]
        public ActionResult AddAccToCCMapMapping()
        {
            var model = _data.GetCostCentresDDLVM();
            return View(model);
        }

        public ActionResult FetchAccountListForMap(int ccId, int type)
        {
            var model = _data.GetAccountVMFilteredList(ccId, type);
            return PartialView("_AccountListForMap", model);
        }

        [HttpPost]
        public ActionResult AddAccToCCMapMapping(AccountToCostCentreAdding model)
        {
            _data.SaveAccountToCCMapping(model);
            return RedirectToAction("MapAccountToCC");
        }

        #endregion

        #region Mapping Account to Category

        public ActionResult ManageAccountCategory()
        {
            var model = _data.GetAccountCategoryList();
            return View("ManageCategory",model);
        }

        [HttpGet]
        public ActionResult AccountCategoryCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AccountCategoryCreate(AccountCategory model)
        {
            _data.SaveAccountCategory(model);
            return RedirectToAction("ManageAccountCategory");
        }

        public ActionResult ActivateAccountCategory(int id)
        {
            _data.DeleteOrActivateAccountCategory(id, false);
            return RedirectToAction("ManageAccountCategory");
        }

        public ActionResult DeleteAccountCategory(int id)
        {
            _data.DeleteOrActivateAccountCategory(id);
            return RedirectToAction("ManageAccountCategory");
        }

        public ActionResult EditAccountCategory(int id)
        {
            var model = _data.GetAccountCategory(id);
            return View(model);
        }
        public ActionResult EditAccountCategory(AccountCategory model)
        {
            _data.SaveAccountCategory(model);
            return RedirectToAction("ManageAccountCategory");
        }
        [HttpGet]
        public ActionResult AddAccountCategoryMapping()
        {
            var model = _data.GetAccountCategoryDDL();
            return View(model);
        }

        public ActionResult FetchAccountsListForCategoryMap(int categoryId)
        {
            var model = _data.GetAccountToCategoryAddingViewModel(categoryId);
            return PartialView("_AccountListForCategoryMap",model );
        }

        [HttpGet]
        public ActionResult MapAccountToCategory()
        {
            var list = _data.GetAccountToCategoryMappingList();
            return View(list);
        }

        [HttpPost]
        public ActionResult AddAccountCategoryMapping(AccountToCategoryAdding model)
        {
            _data.SaveAccountToCategoryMap(model);
            return RedirectToAction("MapAccountToCategory");
        }

        public ActionResult ActivateAccountToCategory(int id)
        {
            _data.DeleteOrActivateAccountToCategory(id, false);
            return RedirectToAction("MapAccountToCategory");
        }

        public ActionResult DeleteAccountToCategory(int id)
        {
            _data.DeleteOrActivateAccountToCategory(id);
            return RedirectToAction("MapAccountToCategory");
        }

        #endregion

        #region Entity to Cost Centre mapping

        public ActionResult MapEntityToCC()
        {
            var model = _data.GetEntityDDLVM();
            return View(model);
        }

        public ActionResult FetchCCEntityMap(int entityId)
        {
            var model = _data.GetCostCentreToEntityViewModelList(entityId);
            return PartialView("_CCToEntityMap", model);
        }

        public ActionResult DeleteCCEntityMap(int entityId, int id)
        {
            _data.DeleteOrActivateCostCentreToEntityMap(id, true);
            var model = _data.GetCostCentreToEntityViewModelList(entityId);
            return PartialView("_CCToEntityMap", model);
        }

        public ActionResult ActivateCCEntityMap(int entityId, int id)
        {
            _data.DeleteOrActivateCostCentreToEntityMap(id, false);
            var model = _data.GetCostCentreToEntityViewModelList(entityId);
            return PartialView("_CCToEntityMap", model);
        }

        [HttpGet]
        public ActionResult AddCostCentreEntityMapping()
        {
            var model = _data.GetEntityDDLVM();
            return View(model);
        }

        public ActionResult FetchCostCentreListForMap(int entityId)
        {
            var model = _data.GetCostCentreVMFilteredList(entityId);
            return PartialView("_CostCentreListForMap", model);
        }

        [HttpPost]
        public ActionResult AddCostCentreEntityMapping(CostCentreToEntityAdding model)
        {
            _data.SaveCostCentreToEntityMapping(model);
            return RedirectToAction("MapAccountToCC");
        }

        #endregion

        #region Substitute

        public ActionResult ManageSubstitute()
        {
            ViewBag.CurrentUserRoles = _ad.GetCurrentUserRoles();
            ViewBag.CurrentUserRoles = "Super Admin";
            var model = _data.GetSubstituteApproverViewModel();
            return View(model);
        }

        public ActionResult SaveSubstituteApprover(SubstituteApproverViewModel model)
        {
            _data.SaveSubstituteApprover(model);
            return RedirectToAction("ManageSubstitute");
        }

        public ActionResult GetSubstituteTable(string approverId)
        {
            
            var model = _data.GetSubstituteApproverTableVMList(approverId);
            return PartialView("_SubstituteTable", model);
        }

        public ActionResult DeleteSubsitution(int substitutionId, string approverId)
        {
            _data.DeleteSubstitution(substitutionId);
            return RedirectToAction("GetSubstituteTable", new { approverId = approverId });
        }

        public ActionResult ReApplySubstitutions()
        {
            _data.ReApplySubstitutions();
            return View("Index");
        }


        [HttpGet]
        public ActionResult ReassignOrder()
        {
            return View();
        }

        public ActionResult FetchOrderForReassign(string tempNumber)
        {
            var model = _data.GetOrderViewForReassignByTempOrder(tempNumber);
            return PartialView("_ReassignOrderForm", model);
        }

        [HttpPost]
        public ActionResult ReassignOrder(OrderViewModel model)
        {
            _data.ReassignOrder(model);
            return RedirectToAction("ReassignOrder");
        }

        #endregion

        #region Ettacher
        
        [HttpGet]
        public ActionResult VouchersForStamping(RestampVoucherFilter filter)
        {
            var model = _main.GetVouchersForRestamping(filter);
            return PartialView("../Ettacher/_SearchVoucherResult", model);
        }

        [HttpGet]
        public ActionResult RestampVouchers()
        {
            var model = new RestampVoucherFilter()
                            {
                                DateFrom = DateTime.Today.ToShortDateString(),
                                DateTo = DateTime.Today.ToShortDateString(),
                            };
            return View(model);
        }

        [HttpPost]
        public ActionResult RestampVouchers(RestampVoucherFilter filter)
        {
            _main.RestampVouchers(filter);
            return null;
        }

        #endregion

        #region Voucher Document Type

        public ActionResult ManageVoucherDocumentTypes()
        {
            var model = _data.GetVoucherDocumentTypeList();
            return View(model);
        }

        [HttpGet]
        public ActionResult VoucherDocumentTypeCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult VoucherDocumentTypeCreate(VoucherDocumentType model)
        {
            _data.SaveVoucherDocumentType(model);
            return RedirectToAction("ManageVoucherDocumentTypes");
        }

        [HttpGet]
        public ActionResult EditVoucherDocumentType(int id)
        {
            var model = _data.GetVoucherDocumentType(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditVoucherDocumentType(VoucherDocumentType model)
        {
            _data.SaveVoucherDocumentType(model);
            return View(model);
        }

        public ActionResult ActivateVoucherDocumentType(int id)
        {
            _data.DeleteOrActivateVoucherDocumentType(id, false);
            return RedirectToAction("ManageVoucherDocumentTypes");
        }

        public ActionResult DeleteVoucherDocumentType(int id)
        {
            _data.DeleteOrActivateVoucherDocumentType(id);
            return RedirectToAction("ManageVoucherDocumentTypes");
        }

        #endregion

        #region Voucher Status

        public ActionResult ManageVoucherStatuses()
        {
            var model = _data.GetVoucherStatusList();
            return View(model);
        }

        [HttpGet]
        public ActionResult VoucherStatusCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult VoucherStatusCreate(VoucherStatus model)
        {
            _data.SaveVoucherStatus(model);
            return RedirectToAction("ManageVoucherStatuses");
        }

        [HttpGet]
        public ActionResult EditVoucherStatus(int id)
        {
            var model = _data.GetVoucherStatus(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditVoucherStatus(VoucherStatus model)
        {
            _data.SaveVoucherStatus(model);
            return View(model);
        }

        public ActionResult ActivateVoucherStatus(int id)
        {
            _data.DeleteOrActivateVoucherStatus(id, false);
            return RedirectToAction("ManageVoucherStatuses");
        }

        public ActionResult DeleteVoucherStatus(int id)
        {
            _data.DeleteOrActivateVoucherStatus(id);
            return RedirectToAction("ManageVoucherStatuses");
        }

        #endregion

        #region Manage Users

       
        public ActionResult ManageUsers()
        {
            var model = _data.GetUserViewModelList();
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult DownloadUsers()
        {
            _userManagement.GetAllUsers();
            return RedirectToAction("ManageUsers");
        }

        #endregion

        #region Receipt Group

        public ActionResult ManageReceiptGroups()
        {
            var model = _data.GetGroups_all();
            return View(model);
        }

        public ActionResult ActivateReceiptGroup(int id)
        {
            _data.DeleteOrActivateReceiptGroup(id, false);
            return RedirectToAction("ManageReceiptGroups");
        }

        public ActionResult DeleteReceiptGroup(int id)
        {
            _data.DeleteOrActivateReceiptGroup(id);
            return RedirectToAction("ManageReceiptGroups");
        }

        public ActionResult EditReceiptGroup(int id)
        {
            var model = _data.GetReceiptGroupViewModel(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditReceiptGroup(ReceiptGroupViewModel model)
        {
            _data.SaveReceiptGroup(model);
           return RedirectToAction("ManageReceiptGroups");
        }

        [HttpGet]
        public ActionResult CreateReceiptGroup()
        {
            var model = new ReceiptGroupViewModel(){Members = new List<ReceiptGroupMemberViewModel>(), UserList = _data.GetUserViewModelList()};
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateReceiptGroup(ReceiptGroupViewModel model)
        {
            _data.SaveReceiptGroup(model);
            return RedirectToAction("ManageReceiptGroups");
        }

        public ActionResult DeleteReceiptGroupMember(int groupId, int id)
        {
            _data.DeleteOrActivateReceiptGroupMember(id);
            var model = _data.GetGroupMembers(groupId);
            return PartialView("_receiptGroupMembers", model);
        }

        public ActionResult ActivateReceiptGroupMember(int groupId, int id)
        {
            _data.DeleteOrActivateReceiptGroupMember(id,false);
            var model = _data.GetGroupMembers(groupId);
            return PartialView("_receiptGroupMembers", model);
        }

        #endregion

        #region Capex Routing

        public ActionResult CapexRouting()
        {
            return View();
        }

        public ActionResult CapexApprovers()
        {
            var list = _data.GetCapexApproversList();
            return View(list);
        }

        [HttpGet]
        public ActionResult CapexApproverCreate()
        {
            var model = _data.GetCapexApproverCRUDViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CapexApproverCreate(CapexApproverCRUDViewModel model)
        {
            _data.SaveCapexApprover(model);
            return RedirectToAction("CapexApprovers");
        }
        [HttpGet]
        public ActionResult CapexApproverEdit(int id)
        {
            var model = _data.GetCapexApproverCRUDViewModel(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult CapexApproverEdit(CapexApproverCRUDViewModel model)
        {
            _data.SaveCapexApprover(model);
            return RedirectToAction("CapexApprovers");
        }

        public ActionResult Divisions()
        {
            var model = _data.GetDivisions();
            return View(model);
        }

        [HttpGet]
        public ActionResult DivisionCreate()
        {
            var model = _data.GetDivisionCRUDViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult DivisionCreate(DivisionCRUDViewModel model)
        {
            _data.SaveDivision(model);
            return RedirectToAction("Divisions");
        }

        [HttpGet]
        public ActionResult DivisionEdit(int id)
        {
            var model = _data.GetDivisionCRUDViewModel(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult DivisionEdit(DivisionCRUDViewModel model)
        {
            _data.SaveDivision(model);
            return RedirectToAction("Divisions");
        }

        #endregion

        #region Capex

        public ActionResult CapexStatus()
        {
            return View();
        }

        public ActionResult FetchCapexForStatusChange(string CapexNumber)
        {
            var model = _data.GetCapexViewModelForStatusChange(CapexNumber);
            return PartialView("_CapexStatusChangeForm", model);
        }

        public ActionResult CapexStatusChange(CapexViewModel model)
        {
            _data.ChangeCapexStatus(model);
            return RedirectToAction("CapexStatus");
        }
        #endregion


       


        #region ItemKit

        public ActionResult ManageItemKit()
        {
            var model = _data.GetItemKitList();
            return View(model);
        }

        public ActionResult ActivateItemKit(int id)
        {
            _data.DeleteOrActivateItemKit(id, false);
            return RedirectToAction("ManageItemKit");
        }

        public ActionResult DeleteItemKit(int id)
        {
            _data.DeleteOrActivateItemKit(id);
            return RedirectToAction("ManageItemKit");
        }

        [HttpGet]
        public ActionResult EditItemKit(int id)
        {
            var model = _data.GetOrderItemKitCRUD(id);
            return View("EditItemKit",model);
        }

        [HttpPost]
        public ActionResult EditItemKit(OrderItemKitCRUDViewModel model)
        {
            _data.SaveOrderItemKit(model);
            return RedirectToAction("ManageItemKit");
        }

        [HttpGet]
        public ActionResult CreateItemKit()
        {
            var model = _data.GetOrderItemKitCRUD(0);
            return View("EditItemKit", model);
        }
        [HttpPost]
        public ActionResult CreateItemKit(OrderItemKitCRUDViewModel model)
        {
            _data.SaveOrderItemKit(model);
            return RedirectToAction("ManageItemKit");
        }


        #endregion

        #region EPO

        public ActionResult POStatus()
        {
            return View();
        }

        public ActionResult FetchOrderForStatusChange(int OrderNumber)
        {
            var model = _data.GetOrderViewForStatusChange(OrderNumber);
            return PartialView("_OrderStatusChangeForm", model);
        }

        public ActionResult OrderStatusChange(OrderViewModel model)
        {
            _data.ChangeOrderStatus(model);
            return RedirectToAction("POStatus");
        }

        public ActionResult ChangeOrderAuthor()
        {
            return View();
        }

        public ActionResult FetchOrderForChangeAuthor(int OrderNumber)
        {
            var model = _data.GetOrderViewForChangeAuthor(OrderNumber);
            return PartialView("_OrderAuthorChangeForm", model);
        }

        public ActionResult OrderAuthorChange(OrderViewModel model)
        {
            _data.ChangeOrderAuthor(model);
            return RedirectToAction("ChangeOrderAuthor");
        }
        #endregion

        #region OLD-Ettacher

        public ActionResult ConvertOldVouchers()
        {
            _main.ConvertOldvouchersToNew();
            return RedirectToAction("Index");
        }


        #endregion

        

    }
}