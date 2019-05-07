using System.Web.Mvc;
using Hangfire;

namespace EPOv2.Controllers
{
    using Business.Interfaces;

    //using MvcRazorToPdf;

    

    public class HomeController : Controller
    {
        private readonly IUserInterface _ui;

        private readonly IAd _ad;
        private readonly IData _data;

        public HomeController(IUserInterface ui, IAd ad,IData data)
        {
            _ui = ui;
            _ad = ad;
            _data = data;
        }

        public ActionResult Index()
        {
          
            return this.View("Dashboard");
        }
       
        public ActionResult LoginPartial()
        {
           // ViewBag.currentUserName = _ad.GetCurrentFullName();
            //var model = _ad.GetCurrentUserRoles();
            return this.PartialView("_LoginPartial", _ad);
        }

        public ActionResult Intranet()
        {
           return RedirectPermanent("http://intranet.oneharvest.com.au/i2006/default.aspx");
        }

        public ActionResult Dashboard()
        {
            return this.View();
        }

        public ActionResult DashboardMyOrders()
        {
     
            var model = _ui.GetMyOrders();
            ViewBag.Action = "Edit";
            ViewBag.Action2 = "View";
            return PartialView("_Dashboard",model);
        }
        public ActionResult DashboardOrdersForApprove()
        {
            var model = this._ui.GetOrdersForApprove();
            ViewBag.Action = "Authorise";
            ViewBag.Action2 = "View";
            return this.PartialView("_Dashboard",model);
        }

        public ActionResult OrdersForApproveFromIntranet()
        {
            var model = _ui.GetOrdersForApprove();
            ViewBag.Action = "Authorise";
            ViewBag.Action2 = "View";
            return this.PartialView("_DashboardIntranet", model);
        }

        public ActionResult DashboardOrdersForMatching()
        {
            var model = _ui.GetOrdersForMatching();
            ViewBag.Action = "Matching";
            ViewBag.Action2 = "View";
            return PartialView("_Dashboard", model);
        }

        public ActionResult ShowTransactions(int orderId)
        {
            _ui.orderId = orderId;
             return PartialView("_Transactions", _ui.GetOrderTransactions());
        }

        public ActionResult DashboardInvoicesForApprove()
        {
            var model = this._ui.GetInvoicesForApprove();
            
            return this.PartialView("_InvoicesForApprove",model);
        }



        [Authorize]
        public ActionResult Pdf()
        {
            //var model = ui.GetMyOrders();

            //var fileName = "testPDf1.pdf";
            //var filePath = Path.Combine("E:\\", fileName);

            //var res = new ViewAsPdf(model)
            //{
            //    FileName = fileName,
            //    PageSize = Size.A4,
            //    PageOrientation = Orientation.Portrait,
            //    PageMargins = { Left = 0, Right = 0 },
            //    SaveOnServerPath = filePath
            //};
            
            //var binary= res.BuildPdf(this.ControllerContext);
            //return File(binary, "application/pdf");
            return RedirectToAction("Dashboard");
            //var pdfResult = new ActionAsPdf("Pdf") { FileName = "Test.pdf" };

            //var binary = pdfResult.BuildPdf(this.ControllerContext);

            //return File(binary, "application/pdf");
        }

        #region Search 

        public ActionResult Search()
        {
            var model = this._ui.GetSearchViewModel();
            return View(model);
        }

        #endregion
    }
}