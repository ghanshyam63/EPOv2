using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EPOv2.Controllers
{
    using System.IO;
    using DomainModel.DataContext;
    using EPOv2.Business;
    using EPOv2.Business.Interfaces;
    using EPOv2.ViewModels;
    using DomainModel.Entities;

    public class PurchaseOrderController : Controller
    {

        #region Property

        private readonly IMain _main;

        private readonly IDataContext _db;

        private readonly IOutput _output;

        private readonly IData _data;

        private readonly IRouting _routing;


        #endregion

        public PurchaseOrderController(IData data, IRouting routing, IMain main, IDataContext db, IOutput output)
        {
            this._data = data;
            this._routing = routing;
            _main = main;
            this._db = db;
            this._output = output;
        }

        public ActionResult Index()
        {
            var modelList = this._main.GetOrderList();
            return this.View(modelList);
        }


        #region New PO

        /// <summary>
        /// Brand new PO
        /// </summary>
        /// <returns></returns>
        public ActionResult RaiseNew()
        {
            var newPOVM = this._main.GetNewPoViewModel();
            return this.View(newPOVM);
        }

        public ActionResult CompanyBox(CompanyBoxViewModel model)
        {
            return this.PartialView("_CompanyBox", model);
        }

        public ActionResult DeliveryBox(DeliveryBoxViewModel model)
        {
            return this.PartialView("_DeliveryBox", model);
        }

        public ActionResult SupplierBox(SupplierBoxViewModel model)
        {
            return this.PartialView("_SupplierBox", model);
        }

        public ActionResult OrderItems(List<OrderItemTableViewModel> model)
        {
            return this.PartialView("_OrderItems", model);
        }

        public ActionResult OrderBottom()
        {
            return this.PartialView("_OrderBottom");
        }

        /// <summary>
        /// Create New Item for Order, with presaving order
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public ActionResult NewOrderItem(NewPOViewModel m)
        {
            if (!ModelState.IsValid)
            {}
            this._main.PreSaveOrder(m);
            var model = this._main.GetOrderItemViewModel(m);

            return this.View("OrderItem", model);
        }

        public ActionResult EditOrderItem(NewPOViewModel m)
        {
            if (!ModelState.IsValid)
            {
            }
            this._main.PreSaveOrder(m);

            ModelState.Clear();

            var model = this._main.GetOrderItemViewModelForEdit(m);

            return this.View("OrderItem", model);
        }

        public ActionResult DeleteOrderItem(NewPOViewModel m)
        {
            if (!ModelState.IsValid){}
            this._main.DeleteOrderItem(m);
            ModelState.Clear(); // cleaning model for proper refresh
            var model = this._main.GetExistingPoViewModel(m.Id,OrderAction.AfterDeleted);
            return this.View("RaiseNew", model);
        }

        /// <summary>
        /// Save order Item and then load PO with new Item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveOrderItem(OrderItemViewModel model, string button)
        {
            var orderId = model.OrderId;
            var isChangedByRevision = false;
          
            if (ModelState.IsValid){}
            if (button == "Save")
            {
                //Save log for OrderItem if Revision
                if (model.IsRevision || model.IsRestrictedEdit)
                {
                    if (model.itemId != 0) this._main.SaveOrderItemLog(model.itemId);
                   // else main.SaveOrderItemLog(model);
                }
                orderId = this._main.SaveOrderItem(model);
                isChangedByRevision = true;
            }

            var m = this._main.GetExistingPoViewModel(orderId, OrderAction.AfterSaved, false, isChangedByRevision);
            return this.View("RaiseNew", m);
        }

        public ActionResult SaveOrder(NewPOViewModel model, string button) //Submit button
        {
            var result = false;
            var approverChoiceVM = new ApproverChoiceViewModel();
            var partialView = string.Empty;
            
            if (button == "Save") //Just Save Order
            {
                _main.SaveOrder(model);
                return RedirectToAction("Index","Home");
            }
            if (button == "Cancel")
            {
                return this.RedirectToAction("Index", "Home");
            }
            _main.db = this._db;
            _routing.Db = this._db;
            _routing.ControllerContext = this.ControllerContext;
            _routing.IsRevision = model.isRevision;
            var order = this._main.SaveOrder(model);
           
            result = _routing.Start(order);
            if (this._routing.ApproverListVM != null)
            {
                approverChoiceVM.ApproversFullList = this._routing.ApproverListVM;
                var minLevel = approverChoiceVM.ApproversFullList.Min(x => x.Level);
                approverChoiceVM.ApproversShortList =
                    approverChoiceVM.ApproversFullList.Where(x => x.Level == minLevel).ToList();
            }
            if (approverChoiceVM.ApproversFullList != null && approverChoiceVM.ApproversFullList.Count > 0)
            {
                partialView = Data.RenderPartialViewToString(this, "_ApproverChoice", approverChoiceVM);
            }
            if(result)
            {
                return Json(new { isMulti = result, data = partialView }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { isMulti = result, url = this.Url.Action("Dashboard", "Home") }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ChooseAuthoriser(ApproverChoiceViewModel model)
        {
            //var routing = new Routing();
            _routing.SetAuthoriserToOrder(model.ApproverChoiceOrderId, model.SelectedApprover);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SetDefaultOrderSettings(NewPOViewModel model)
        {
            this._main.SavaDefaultOrderSettings(model.DefaultOrderSettingsViewModel);
            return null;
        }

        #endregion


        #region Dashboard Buttons

        public ActionResult EditOrder(DashboardViewModel model)
        {
            var m = this._main.GetExistingPoViewModel(model.SelectedItem,OrderAction.Edit);
            m.IsLocked = false;
            return this.View("RaiseNew", m);
        }

        //TODO: Everything should be locked. Need to create a new View
        public ActionResult ViewOrder(DashboardViewModel model)
        {
            var m = this._main.GetExistingPoViewModel(model.SelectedItem, OrderAction.View);
            if (m == null)
                return
                    RedirectPermanent(
                        "http://viis1.oneharvest.com.au/epo/main.aspx?LoadType=View&POID=" + model.SelectedItem);
            m.IsLocked = true;
            return this.View("RaiseNew", m);
        }
        [HttpGet]
        public ActionResult ModifyDueDate()
        {
          
            return this.View();
        }
     
        public ActionResult _ModifyDueDate(int OrderNo)
        {
            var m = _main.GetOrderToModifyDueDate(OrderNo);
            return this.View("_ModifyDueDate", m);
        }
        public ActionResult ModifyDueDateFromIntranet(int OrderNo)
        {
            var m = _main.GetOrderToModifyDueDate(OrderNo);
            return this.View("ModifyDueDateFromIntranet", m);
        }
        [HttpPost]
        public JsonResult SaveDueDate(Order m)
        {
            //DateTime DueDate = DateTime.Parse(sDueDate);
            var s = _main.SaveOrderToModifyDueDate(m);
            return Json("Sucess", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AuthoriseOrder(DashboardViewModel model)
        {
            var m = _main.GetExistingPoViewModel(model.SelectedItem,OrderAction.Authorise);
            m.IsLocked = true;
            return this.View("AuthoriseOrder",m);
        }

        [HttpPost]
        public ActionResult ApproveOrder(NewPOViewModel model, string button)
        {
            if (button == "Cancel")
            {
                return RedirectToAction("Dashboard", "Home");
            }
            _routing.ControllerContext = this.ControllerContext;
            _routing.StartApproveOrder(model);
            return RedirectToAction("Dashboard", "Home");
        }

        public ActionResult DeleteOrder(DashboardViewModel model)
        {
            //TODO: НАХУЯ ЭТО ЗДЕСЬ????
            var m = this._main.GetExistingPoViewModel(model.SelectedItem,OrderAction.Delete); 
            this._main.DeleteOrder(m);
            return RedirectToAction("Dashboard", "Home");
        }
       
        public ActionResult OpenPDFPO(DashboardViewModel model)
        {
            var order = this._data.GetOrder(model.SelectedItem);
            _output.Order = order;
            _output.CreatePDFContainer();
            //var pdfModel = output.GeneratePDFPO();
            var fileName = "PO" + order.OrderNumber + ".pdf";
            var binary = _output.GeneratePOPDFFile(fileName, _output.PdfPOContainer, ControllerContext);
            return File(binary, "application/pdf");
        }

        public ActionResult SendPObyEmail(DashboardViewModel model)
        {
            var order = this._data.GetOrder(model.SelectedItem);
            _routing.ControllerContext=this.ControllerContext;
            if (order.Status.Name == StatusEnum.Cancelled.ToString())
            {
                _output.Order = order;
                _output.CreatePDFContainer();
                var fileName = "PO" + order.OrderNumber + ".pdf";
                var filePath = Path.Combine(Output.fileDirectory, fileName);
                _output.SendEmailWithCancelledOrder(filePath);
            }
            else
            {
                _routing.SendEmail(order);
            }
            return RedirectToAction("Dashboard", "Home");
        }

      public ActionResult MatchOrder(DashboardViewModel model)
        {
            var m = this._main.GetExistingPoViewModel(model.SelectedItem,OrderAction.Matching,true);
            m.IsLocked = true;
            m.IsMatching = true;
            return this.View("MatchOrder", m);;
        }

        [HttpPost]
        public ActionResult MatchingOrder(NewPOViewModel model, string button)
        {
            if (button == "Cancel")
            {
                return RedirectToAction("Dashboard", "Home");
            }
            if (model.MatchingItems.Any(x => x.QtyReceived >= 0))
            {
                _main.SaveMatchOrder(model);
            }
            else return RedirectToAction("MatchOrder", new DashboardViewModel() { SelectedItem = model.Id });

            return RedirectToAction("Dashboard", "Home");
        }

        public ActionResult CancelOrder(DashboardViewModel model)
        {
            this._main.controllerContext = this.ControllerContext;
            this._main.CancelOrder(model.SelectedItem);
            return RedirectToAction("Dashboard", "Home");
        }

        public ActionResult CloseOrder(DashboardViewModel model)
        {
            _main.CloseOrder(model.SelectedItem);
            return RedirectToAction("Dashboard", "Home");
        }

        public ActionResult SearchEPO(SearchViewModel model)
        {
            var result = _main.SearchEPO(model);
            return this.PartialView("_SearchEPOResult", result);
        }

        #region External Buttons

        public ActionResult ViewOrderExternal(int id)
        {
            return ViewOrder(new DashboardViewModel() { SelectedItem = id });
        }

        public ActionResult EditOrderExternal(int id)
        {
            return EditOrder(new DashboardViewModel() { SelectedItem = id });
        }

        public ActionResult AuthoriseOrderExternal(int id)
        {
            return AuthoriseOrder(new DashboardViewModel() { SelectedItem = id });
        }

        public ActionResult DeleteOrderExternal(int id)
        {
            return DeleteOrder(new DashboardViewModel() { SelectedItem = id });
        }

        public ActionResult OpenPDFPOExternal(int id)
        {
            return OpenPDFPO(new DashboardViewModel() { SelectedItem = id });
        }

        public ActionResult SendPObyEmailExternal(int id)
        {
            return SendPObyEmail(new DashboardViewModel() { SelectedItem = id });
        }

        public ActionResult CancelOrderExternal(int id)
        {
            return this.CancelOrder(new DashboardViewModel() { SelectedItem = id });
        }

        public ActionResult MatchOrderExternal(int id)
        {
            return MatchOrder(new DashboardViewModel() { SelectedItem = id });
        }

        public ActionResult CloseOrderExternal(int id)
        {
            return CloseOrder(new DashboardViewModel() { SelectedItem = id });
        }

        #endregion

        public ActionResult PdfPOView()
        {
            //var order = this._data.GetOrder(3735);
            var order = this._data.GetOrder(3114);
            _output.Order = order;
            var model = _output.GeneratePDFPO();
            _output.CreatePDFContainer();
            return this.PartialView("PdfContainer", this._output.PdfPOContainer);
        }

        #endregion






        #region FetchingPartialViews

        public ActionResult FetchCompanyBox(string entityId)
        {
            var model = this._main.GetCompanyBoxViewModel(Convert.ToInt32(entityId));

            return this.PartialView("_CompanyBox", model);
        }

        public ActionResult FetchCompanyBoxByCC(string entityId, string ccId)
        {
            var model = this._main.GetCompanyBoxViewModel(Convert.ToInt32(entityId), Convert.ToInt32(ccId));

            return this.PartialView("_CompanyBox", model);
        }

        public ActionResult FetchDeliveryBox(string daId, string userId = "")
        {
            var model = this._main.GetDeliveryBoxViewModel(Convert.ToInt32(daId), userId);
            return this.PartialView("_DeliveryBox", model);
        }

        public ActionResult FilterSuppliers(string entityId)
        {

            var model = this._main.FilterSuppliersViewModel(Convert.ToInt32(entityId));
            return this.PartialView("_SupplierBox", model);
        }

        public ActionResult FetchSupplierBox(string supplierId, string entityId)
        {
            var model = this._main.GetSupplierBoxViewModel(Convert.ToInt32(supplierId), Convert.ToInt32(entityId), "");
            return this.PartialView("_SupplierBox", model);
        }

        public ActionResult FetchItemKit(int accountId, int orderId)
        {
            var model = this._main.GetItemKitDdl(accountId,orderId);
            return this.PartialView("_ItemKit", model);
        }

        public ActionResult FetchItemKitDetails(int itemKitId)
        {
            var model = this._data.GetOrderItemKit(itemKitId);
            return Json(new { result = model }, JsonRequestBehavior.AllowGet);
        }

        #endregion



    }
}