using System.Web.Mvc;

namespace EPOv2.Controllers
{
    public class StagingController : Controller
    {

        ////private readonly PurchaseOrderContext _db = new PurchaseOrderContext();
        //private readonly PurchaseOrderOldContext db = new PurchaseOrderOldContext();

        //private readonly Routing _routing = new Routing();
        ////
        //// GET: /Staging/
        //public  ActionResult Index()
        //{
        //    return View();
        //}

        //public async Task<ActionResult> LevelReport()
        //{
        //    var levelLst = await this.db.tblStagingLevels.ToListAsync();
        //    return View(levelLst);
        //}

        //public async Task<ActionResult> EmployeeLevelReport()
        //{
        //    var empLst =await this.db.tblStagingEmployeesDEVs.Where(x => x.isDeleted == 0).ToListAsync();
        //    return View(empLst);
        //}

        //public async Task<ActionResult> CCOwnerReport()
        //{
        //    var ccOwnerLst =await this.db.tblStagingCostCentresDEVs.OrderBy(x=>x.CostCentreCode).ToListAsync();
        //    return View(ccOwnerLst);
        //}

        //public async Task<ActionResult> CCAccountReport(string CostCenteCode)
        //{
        //    ViewBag.ccLst = _routing.GetCostCentreList();
        //    if (CostCenteCode != null)
        //    {
        //        var ccCode = Convert.ToInt32(CostCenteCode);
        //        var ccAccLst =
        //            await this.db.tblStagingAccCCs.Where(x => x.tblStagingCostCentresDEV.CostCentreCode == ccCode).OrderBy(x=>x.AccountCode).ToListAsync();
        //        return View(ccAccLst);
        //    }
        //    return View();
        //}
    }
}