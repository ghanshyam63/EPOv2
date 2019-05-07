using System.Web.Mvc;

namespace EPOv2.Controllers
{
    using System;
    using System.IO;

    using EPOv2.Business;
    using EPOv2.Business.Interfaces;
    using EPOv2.Migrations;
    using EPOv2.ViewModels;

    public class CapexController : Controller
    {
        private readonly IData _data;

        private readonly IUserInterface _userInterface;

        private readonly IMain _main;

        private readonly IRouting _routing;

        private readonly IOutput _output;

        public CapexController(IData data, IUserInterface userInterface, IMain main, IRouting routing, IOutput output)
        {
            this._data = data;
            this._userInterface = userInterface;
            this._main = main;
            this._routing = routing;
            this._output = output;
        }

        public ActionResult Dashboard()
        {
            return this.View();
        }

        // GET: Capex
        public ActionResult Create()
        {
            ViewBag.Title = "new Capex";
            var model = _data.GetCapexCRUDViewModel();  // new CapexCRUDViewModel() {CapexCompanyBox = new CapexCompanyBox(), CapexDetailBox = new CapexDetailBox()};
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveCapex(CapexCRUDViewModel model, string button)
        {
            if (button == "Save")
            {
                try
                {
                    var capex= _main.SaveCapex(model);
                    _main.DeleteExistingCapexRouting(capex);
                    _main.StartRouting(capex);
                    _main.SendCapexApproveNotification(capex.Id,this);
                    _main.SendCapexNotificationToAuthor(model.Id, this);
                }
                catch (Exception e)
                {
                    _main.LogError("CapexController.SaveCapex",e);
                }
            }
            return RedirectToAction("Dashboard");
        }

        public ActionResult FetchCostCentre(int entityId)
        {
            var model = _data.GetCapexCompanyBox(entityId);
            return this.PartialView("_CompanyBox", model);

        }

        public ActionResult OrdersForMyCapex()
        {

            return null;
        }

        public ActionResult DashboardCapexForApprove()
        {
            var list = _userInterface.GetCapexesForApprove();
            ViewBag.Action = "Approve";
            return this.PartialView("_DashboardCapexesShort", list);
        }

        public ActionResult DashboardMyCapexes()
        {
            var list = _userInterface.GetMyCapexes();
            ViewBag.Action = "Edit";
            ViewBag.Action2 = "View";
            return this.PartialView("_DashboardCapexes",list);
        }

        public ActionResult ShowTransactions(int capexId)
        {
            return this.PartialView("_Transactions", _main.GetCapexTransactions(capexId));
        }

        
        public ActionResult ApproveCapex(DashboardViewModel model)
        {
            var capexVM = _data.GetCapexCRUDViewModel(model.SelectedItem, CapexAction.Authorise);
            ViewBag.Title = "Authorise Capex";
            ViewBag.Action = "Authorise";
            if (capexVM.IsDeleted) return View("DeletedCapex");
            return View("Create", capexVM);
        }

        public ActionResult ApproveCapexExternal(int id)
        {
            return RedirectToAction("ApproveCapex", new DashboardViewModel() { SelectedItem = id });
            //return ApproveCapex(new DashboardViewModel() { SelectedItem = id });
        }

       public ActionResult AuthoriseCapex(CapexCRUDViewModel model, string button)
        {
            _routing.ApproveCapex(model);
            _main.SendCapexApproveNotification(model.Id, this);
            _main.SendCapexNotificationToAuthor(model.Id, this);
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public ActionResult DeclineCapex(CapexCRUDViewModel model, string button)
        {
            _routing.DeclineCapex(model);
            _main.SendCapexNotificationToAuthor(model.Id, this);
            return RedirectToAction("Dashboard");
        }

        public ActionResult EditCapex(DashboardViewModel model)
        {
          
            var capexVM = _data.GetCapexCRUDViewModel(model.SelectedItem, CapexAction.Edit);
            ViewBag.Title = "Edit Capex";
            ViewBag.Action = "Edit";
            return View("Create", capexVM);
        }

        public ActionResult ViewCapex(DashboardViewModel model)
        {
           
            ViewBag.Title = "view Capex";
            var capex = _data.GetCapexCRUDViewModel(model.SelectedItem, CapexAction.View);
            return View("Create",capex);
        }
        public ActionResult DeleteCapex(DashboardViewModel model)
        {
            _main.DeleteCapex(model.SelectedItem);
            return RedirectToAction("Dashboard");
        }

        public ActionResult SendNotification(DashboardViewModel model)
        {
            _main.SendCapexApproveNotification(model.SelectedItem,this);
            return RedirectToAction("Dashboard");
        }

        //public ActionResult SendNotification(int id)
        //{
        //    _main.SendCapexApproveNotification(id, this);
        //    return RedirectToAction("Dashboard");
        //}

        [HttpPost]
        public ActionResult ViewDocument(string Id)
        {
           if (!string.IsNullOrEmpty(Id))
            {
                var capexReference = _data.GetCapexReference(Id);
                var fileStream = new FileStream(capexReference,
                                         FileMode.Open,
                                         FileAccess.Read
                                       );
                var fsResult = new FileStreamResult(fileStream, "application/pdf");
                return fsResult;
            }
            return null;
        }

        public ActionResult SearchCapex(SearchViewModel model)
        {
            var result = this._main.SearchCapex(model);
            return this.PartialView("_SearchCapexResult", result);
        }

    }
}