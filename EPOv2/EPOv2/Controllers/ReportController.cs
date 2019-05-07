using System.Net.Mail;
using System.Web.Mvc;
using Hangfire;

namespace EPOv2.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;

    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.Properties;
    using EPOv2.ViewModels;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Serilog;

    public class ReportController : Controller
    {
        private readonly ILogger _logger = Log.Logger;
        private readonly IData _data;

        private readonly IUserManagement _userManagement;

        private readonly IOutput _output;

        private readonly IMain _main;

        private readonly IRouting _routing;

        public ReportController(IData data, IUserManagement userManagement, IOutput output, IMain main,IRouting routing)
        {
            this._data = data;
            this._userManagement = userManagement;
            this._output = output;
            this._main = main;
            this._routing = routing;
        }

        // GET: Report
        public ActionResult Index()
        {
          
              
            return View();
        }

        public ActionResult LevelReport()
        {
            var model = this._data.GetLevels();
            return View(model);
        }

        public ActionResult RoutingReport()
        {
            //var model = Data.GetRoutingTree();
            var model = this._data.GetCostCentresDDLVM(); 
            return this.View(model);
        }

        public ActionResult FetchRoutingStructureForCC(int ccId)
        {
            _logger.Information("Fetch Routing Structure For CC:{ccId}",ccId);
           var model= _routing.GetRoutingStructureForCC(ccId);
            return this.PartialView("_reportingStructureForCC",model);
        }

        public ActionResult AdUsersWithoutPhone()
        {
            var model = _userManagement.GetAllUsersWithoutPhone();
            return this.View(model);
        }

        public ActionResult CostCentreOwnerReport()
        {
            var model = this._data.GetCostCentresWithOwnerVM();
            return this.View(model);
        }

        public ActionResult AdminStats()
        {
            var model = this._data.GetAdminStats();
            return this.View(model);
        }

        public ActionResult RunEttacherEmailReports()
        {
            this.AuthorisedVouchersEmailReport();
            this.DeclinedVouchersEmailReport();
            return View("Index");
        }

        public ActionResult RunOutstandingInvoiceEmailReport()
        {
            this.OutstandingInvoiceEmailReport();
            return this.View("Index");
        }

        public ActionResult RunCostCentreOwnerCheckReport()
        {
            var model = this._data.CheckCostCentreOwners();
            var recipient = Settings.Default.EmptyCcOwnerRecipient;
            if (model.Count > 0)
            {
                this._main.GenerateAndSendCCOwnerReport(this, "_EmptyCostCentreOwnerReport", model, recipient);
            }
            return this.View("Index");
        }

        public void OutstandingInvoiceEmailReport()
        {
            var model = _main.GetOutstandingInvoices();
            _main.GenerateAndSendOutstandingInvoiceReport(this, "_OutstandingInvoiceReport", model);
            //var bodytext = RenderPartialViewToString(this, "OutstandingInvoiceReport", model);
            //var text = PreMailer.MoveCssInline(bodytext).Html;
            //_output.SendEmailReport(text, model);
            //return;
        }

        public void AuthorisedVouchersEmailReport()
        {
            var searchModel = new SearchViewModel { SelectedStatus = 2, DateFrom = DateTime.Today.AddDays(-90).ToString("dd/MM/yyyy"), DateTo = DateTime.Today.ToString("dd/MM/yyyy") }; //2 - Authorised
            var model = _main.SearchVoucher(searchModel);
            model = this._main.GetAuthorisedVouchersReport(model);
            var bodytext = RenderPartialViewToString(this, "EttacherReportAuthorised", model.OrderByDescending(x => x.AuthorisationDate));
            _output.SendEmailForEttacher(bodytext, "Authorised vouchers");
            // BackgroundJob.Enqueue(() => BackgroundAuthoriseVoucherEmail());
        }

        public void DeclinedVouchersEmailReport()
        {
            var searchModel = new SearchViewModel { SelectedStatus = 4, DateTo = DateTime.Today.ToString("dd/MM/yyyy") }; //4 - Declined
            var model = _main.SearchVoucher(searchModel);
            var bodytext = RenderPartialViewToString(this, "EttacherReportAuthorised", model.OrderByDescending(x => x.Date));
            _output.SendEmailForEttacher(bodytext, "Declined vouchers");
            // BackgroundJob.Enqueue(() => BackgroundDeclineVoucherEmail());
        }

        public void BackgroundDeclineVoucherEmail()
        {
            var searchModel = new SearchViewModel { SelectedStatus = 4, DateTo = DateTime.Today.ToString("dd/MM/yyyy") }; //4 - Declined
            var model = _main.SearchVoucher(searchModel);
            var bodytext = RenderPartialViewToString(this, "EttacherReportAuthorised", model.OrderByDescending(x => x.Date));
            _output.SendEmailForEttacher(bodytext, "Declined vouchers");
        }
        //This task will run in background using Hangfire.Because it's timeout problem with old code.
        public void BackgroundAuthoriseVoucherEmail()
        {
            var searchModel = new SearchViewModel { SelectedStatus = 2, DateFrom = DateTime.Today.AddDays(-90).ToString("dd/MM/yyyy"), DateTo = DateTime.Today.ToString("dd/MM/yyyy") }; //2 - Authorised
            var model = _main.SearchVoucher(searchModel);
            model = this._main.GetAuthorisedVouchersReport(model);
            var bodytext = RenderPartialViewToString(this, "EttacherReportAuthorised", model.OrderByDescending(x => x.AuthorisationDate));
            _output.SendEmailForEttacher(bodytext, "Authorised vouchers");
        }

        public ActionResult AuthorisedVouchersEmailReportTest()
        {
            var searchModel = new SearchViewModel { SelectedStatus = 2 };//4 - Declined
            var model = _main.SearchVoucher(searchModel);
            return this.View("EttacherReportAuthorised",model);
        }

        public static string RenderPartialViewToString <T>(Controller controller, string viewName, T model)
        {
            controller.ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(controller.ControllerContext, viewName,null);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.ToString();
            }
        }

        [HttpGet]
        public ActionResult PurchaseOrderAccurals()
        {
            var filterModel = this._data.GetReportEPOAccuralsFilterViewModel();
            return this.View("EPOAccural",filterModel);
        }

        [HttpPost]
        public ActionResult PurchaseOrderAccurals(ReportEPOAccuralsFilterViewModel filter)
        {
            var model = this._data.RunEPOAccuralsReport(filter);
            return this.PartialView("_epoAccuralResult", model);
        }


        public ActionResult UnreceiptedPO()
        {
            var model = _data.GetUnrecieptedPO(DateTime.Today);
            return View(model);
        }

        #region Budget

        public ActionResult Budget()
        {
            var model = this._data.GetBudgetReportEntryDataViewModel();
            return this.View(model);
        }

        public ActionResult FetchBudgetReportEntryData(int entityId)
        {
            _logger.Information($"Fetch Budget Entry data for entity:{entityId}");
            var model = _data.GetBudgetReportEntryDataViewModel(entityId);
            return PartialView("_BudgetReportFilter", model);
        }

        public ActionResult RunBudgetReport(BudgetReportEntryDataViewModel filter)
        {
            _logger.Information("Run Budget Report for Entity:{entityId}, CC:{ccId}, Category:{categoryId}", filter.SelectedEntity,filter.SelectedCostCenter, filter.SelectedCategory);
            var model = _data.GetBudgetReport(filter);
            return PartialView("_BudgetReportResult",model);
        }

        
        public ActionResult GetReForecastAndVarianceForBudgetReport()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            BudgetReportResult input;
            try
            {
                input = JsonConvert.DeserializeObject<BudgetReportResult>(json);
            }

            catch (Exception e)
            {
                _logger.Error(e, "JsonConvert to BudgetReportResult");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _logger.Information($"GetReForecastAndVarianceForBudgetReport");
            var result = _data.GetReForecastAndVarianceForBudgetReport(input);
            return PartialView("_BudgetReportReForecastVariance", result);
        }

        public ActionResult GetDetailsForBudgetReport(string glCode, int period, int entity, int cc,bool isEstimate)
        {
            var model = _data.GetPeriodEPODetailsForBudgetReport(glCode, period, entity, cc, isEstimate);
            return PartialView("_BudgetReportDetails",model);
        }

        public ActionResult GetVoucherDetailsForBudgetReportByCategory(int categoryId, int period, int entity, int cc)
        {
            var model = _data.GetPeriodVoucherDetailsForBudgetReportByCategory(categoryId, period, entity, cc);
            return PartialView("_BudgetReportDetails", model);
        }

        public ActionResult GetDetailsForBudgetReportByCategory(int categoryId, int period, int entity, int cc)
        {
            var model = _data.GetPeriodEPODetailsForBudgetReport(categoryId, period, entity, cc);
            return PartialView("_BudgetReportDetails", model);
        }

        public ActionResult RunBudgetWithVouchersReport(BudgetReportEntryDataViewModel filter)
        {
            var model = _data.GetBudgetWithVouchersReport(filter);
            return PartialView("_BudgetWithVouchersReportResult",model);
        }


        public ActionResult TestAction()
        {
           // var dd = JsonConvert.DeserializeObject<List<BudgetReportItem>>(model);

            _logger.Information($"Test Action:");
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            BudgetReportResult input = null;
            try
            {
                // assuming JSON.net/Newtonsoft library from http://json.codeplex.com/
                input = JsonConvert.DeserializeObject<BudgetReportResult>(json);
            }

            catch (Exception)
            {
                // Try and handle malformed POST body
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View("Index");
        }

        public ActionResult InvoiceExceedReport()
        {
            return View("InvoiceExceedReport", new InvoiceExceedReportFilterVM());
        }

        public ActionResult RunInvoiceExceedReport(InvoiceExceedReportFilterVM filter)
        {
            _logger.Information("Run Invoice Exceed Report from:{dateFrom} to:{dateTo}", filter.dateFrom, filter.dateTo);
            var model = _main.GetInvoiceExceedReport(filter);
            return PartialView("_InvoiceExceedReportResult", model);
        }

        #endregion

    }
}