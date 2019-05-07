using System.Web.Mvc;

namespace EPOv2.Controllers
{
    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;

    public class DataManagementController : Controller
    {
        //TODO: Сделать фидбэк на форму о результате!
        private readonly IData _data;

        public DataManagementController(IData data)
        {
            this._data = data;
        }

        // GET: DataManagement
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FetchEntity()
        {
            this._data.FetchEntity();
            return RedirectToAction("Index");
        }

        public ActionResult FetchCC()
        {
            this._data.FetchCC();
            return RedirectToAction("Index");
        }

        public ActionResult FetchDeliveryAddress()
        {
            this._data.FetchDeliveryAddress();
            return RedirectToAction("Index");
        }

        public ActionResult FetchGroup()
        {
            this._data.FetchReceiptGroup();
            return RedirectToAction("Index");
        }

        public ActionResult FetchAccounts()
        {
            this._data.FetchAccount();
            return RedirectToAction("Index");
        }

        public ActionResult FetchCcToEntity()
        {
            this._data.FetchCcToEntity();
            return RedirectToAction("Index");
        }

        public ActionResult FetchAccAndSubToCc()
        {
            this._data.FetchAccountToCc();
            this._data.FetchSubAccountToCc();
            return RedirectToAction("Index");
        }
        public ActionResult States()
        {
            var states = this._data.GetStates();
            return this.View(states);
        }
        
        [HttpGet]
        public ActionResult CreateState()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult CreateState(State model)
        {
            if (ModelState.IsValid)
            {
                this._data.SaveState(model);
                return RedirectToAction("States");
            }
            return this.View();
        }

        public ActionResult Currencies()
        {
            var currencies = this._data.GetCurrencies();
            return this.View(currencies);
        }

        [HttpGet]
        public ActionResult CreateCurrency()
        {
            return this.View();
        }
        [HttpPost]
        public ActionResult CreateCurrency(Currency model)
        {
            this._data.SaveCurrency(model);
            return this.View();
        }

        public ActionResult Statuses()
        {
            var model = this._data.GetStatuses();
            return this.View(model);
        }

        [HttpGet]
        public ActionResult CreateStatus()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateStatus(Status model)
        {
            this._data.SaveStatus(model);
            return RedirectToAction("Statuses");
        }

        public ActionResult Levels()
        {
            var modelList = this._data.GetLevels();
            return this.View(modelList);
        }

        [HttpGet]
        public ActionResult CreateLevel()
        {
            return this.View();
        }
        
        [HttpPost]
        public ActionResult CreateLevel(Level model)
        {
            this._data.SaveLevel(model);
            return this.RedirectToAction("Levels");
        }


        public ActionResult CostCentres()
        {
            var modelList = this._data.GetAllCostCentres();
            return this.View(modelList);
        }

        [HttpGet]
        public ActionResult EditCostCentre(int ccId)
        {
            return this.View();
        }

        //public ActionResult ChangeCostCentreOwner()
        //{
        //    var model = data.GetCostCentreOwnerVM();
        //    return this.View(model);
        //   // return null;
        //}

        //[HttpPost]
        //public ActionResult ChangeCostCentreOwner(CostCentreOwnerViewModel model)
        //{
        //    var result = data.SaveCostCentreOwner(model);
        //    return Json(result, JsonRequestBehavior.AllowGet); ;
        //}

        //public ActionResult FetchCostCentreOwner(int ccId)
        //{
        //    var result = data.GetCostCentreOwner(ccId);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult FetchCCOwner()
        {
            this._data.FetchCCOwners();
            return RedirectToAction("Index");
        }

        public ActionResult FillAuthorId()
        {
            this._data.FillAuthorId();
            return RedirectToAction("Index");
        }



        

    }
}