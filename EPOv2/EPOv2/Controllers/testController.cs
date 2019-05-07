using System.Web.Mvc;


namespace EPOv2.Controllers
{
    public class testController : Controller
    {
    //    private readonly PurchaseOrderContext db = new PurchaseOrderContext();
    //    private readonly PurchaseOrderOldContext dbOld = new PurchaseOrderOldContext();
    //    private readonly Routing routing = new Routing();
    //    private readonly Data data = new Data();
    //    private readonly UserInterface ui = new UserInterface();
    //    private readonly AD ad= new AD();
        

    //    // GET: /test/
    //    public async Task<ActionResult> Index()
    //    {
    //        var tblstagingemployeesdevs = dbOld.tblStagingEmployeesDEVs.Include(t => t.tblStagingLevel);
    //        return View(await tblstagingemployeesdevs.ToListAsync());
    //    }
    //    public ActionResult Popup()
    //    {
    //        //var tblstagingemployeesdevs = db.tblStagingEmployeesDEVs.Include(t => t.tblStagingLevel);
    //        return View();
    //    }

    //    public ActionResult RockieEmp()
    //    {
    //        var rockieEmp = db.vRockyEmployees.ToList();
    //        return this.View(rockieEmp);
    //    }


    //    // GET: /test/Details/5
    //    public async Task<ActionResult> Details(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        tblStagingEmployeesDEV tblstagingemployeesdev = await dbOld.tblStagingEmployeesDEVs.FindAsync(id);
    //        if (tblstagingemployeesdev == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(tblstagingemployeesdev);
    //    }

    //    // GET: /test/Create
    //    public ActionResult Create()
    //    {
    //        ViewBag.Level = new SelectList(dbOld.tblStagingLevels, "level", "level");
    //        return View();
    //    }

    //    // POST: /test/Create
    //    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    //    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> Create([Bind(Include="StagingEmpID,Employee,Manager,Level,isDeleted")] tblStagingEmployeesDEV tblstagingemployeesdev)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            dbOld.tblStagingEmployeesDEVs.Add(tblstagingemployeesdev);
    //            await dbOld.SaveChangesAsync();
    //            return RedirectToAction("Index");
    //        }

    //        ViewBag.Level = new SelectList(dbOld.tblStagingLevels, "level", "level", tblstagingemployeesdev.Level);
    //        return View(tblstagingemployeesdev);
    //    }

    //    // GET: /test/Edit/5
    //    public async Task<ActionResult> Edit(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        tblStagingEmployeesDEV tblstagingemployeesdev = await dbOld.tblStagingEmployeesDEVs.FindAsync(id);
    //        if (tblstagingemployeesdev == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        ViewBag.Level = new SelectList(dbOld.tblStagingLevels, "level", "level", tblstagingemployeesdev.Level);
    //        return View(tblstagingemployeesdev);
    //    }

    //    // POST: /test/Edit/5
    //    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    //    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> Edit([Bind(Include="StagingEmpID,Employee,Manager,Level,isDeleted")] tblStagingEmployeesDEV tblstagingemployeesdev)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            dbOld.Entry(tblstagingemployeesdev).State = EntityState.Modified;
    //            await dbOld.SaveChangesAsync();
    //            return RedirectToAction("Index");
    //        }
    //        ViewBag.Level = new SelectList(dbOld.tblStagingLevels, "level", "level", tblstagingemployeesdev.Level);
    //        return View(tblstagingemployeesdev);
    //    }

    //    // GET: /test/Delete/5
    //    public async Task<ActionResult> Delete(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //        }
    //        tblStagingEmployeesDEV tblstagingemployeesdev = await dbOld.tblStagingEmployeesDEVs.FindAsync(id);
    //        if (tblstagingemployeesdev == null)
    //        {
    //            return HttpNotFound();
    //        }
    //        return View(tblstagingemployeesdev);
    //    }

    //    // POST: /test/Delete/5
    //    [HttpPost, ActionName("Delete")]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> DeleteConfirmed(int id)
    //    {
    //        tblStagingEmployeesDEV tblstagingemployeesdev = await dbOld.tblStagingEmployeesDEVs.FindAsync(id);
    //        dbOld.tblStagingEmployeesDEVs.Remove(tblstagingemployeesdev);
    //        await dbOld.SaveChangesAsync();
    //        return RedirectToAction("Index");
    //    }

    //    public ActionResult RaisingPO()
    //    {
    //        return this.View("TestRaisingPO");
    //    }

    //    public ActionResult FirstScenario()
    //    {
    //        var order = routing.GetFirstScenarioOrder();

    //        routing.Start(order);
    //        var routeList = routing.GetRouteForOrder(order.Id);

    //        return RedirectToAction("RaisingPO");
    //    }

    //    public ActionResult SecondScenario()
    //    {
    //        var order = routing.GetSecondScenarioOrder();

    //        routing.Start(order);
    //        var routeList = routing.GetRouteForOrder(order.Id);

    //        return RedirectToAction("RaisingPO");
    //    }

    //    public ActionResult ThirdScenario()
    //    {
    //        var order = routing.GetThirdScenarioOrder();

    //        routing.Start(order);
    //        var routeList = routing.GetRouteForOrder(order.Id);

    //        return RedirectToAction("RaisingPO");
    //    }

    //    public ActionResult FourthScenario()
    //    {
    //        var order = routing.GetFourthScenarioOrder();

    //        routing.Start(order);
    //        var routeList = routing.GetRouteForOrder(order.Id);

    //        return RedirectToAction("RaisingPO");
    //    }

    //    public ActionResult FifthScenario()
    //    {
    //        var order = routing.GetFifthScenarioOrder();

    //        routing.Start(order);
    //        var routeList = routing.GetRouteForOrder(order.Id);

    //        return RedirectToAction("RaisingPO");
    //    }


    //    public ActionResult PdfPO()
    //    {
    //        var output = new Output(586);
    //        var model = output.GeneratePDFPO();
    //        //var model = ui.GetMyOrders();
    //        var fileName = "testPDf1.pdf";
    //        var filePath = Path.Combine(Output.fileDirectory, fileName);

    //        var res = new ViewAsPdf("pdfReport",model)
    //        {
    //            FileName = fileName,
    //            PageSize = Size.A4,
    //            PageOrientation = Orientation.Portrait,
    //            PageMargins = { Left = 3, Right = 3,Bottom = 2,Top=0},
    //            SaveOnServerPath = filePath,
    //           // CustomSwitches = string.Format("--username \"{0}\" --password \"{1}\"" , "administrator", ".Wheel Lock.")
    //        };

    //        var binary = res.BuildPdf(this.ControllerContext);
    //        //return File(binary, "application/pdf");
    //        return RedirectToAction("Dashboard","Home");
    //        //return this.View("pdfReport",model);
    //    }
    //    public ActionResult PdfPOView()
    //    {
    //        var output = new Output(586);
    //        var model = output.GeneratePDFPO();
    //        return this.View("testPDFLogo",model);
    //    }

    //    public ActionResult PdfMultiPage()
    //    {
    //        var output = new Output(342);
    //        output.CreatePDFContainer();
    //        var fileName = "testPDf1.pdf";
    //        var filePath = Path.Combine(Output.fileDirectory, fileName);

    //      // return this.View("PdfContainer", output.PdfPOContainer);

    //        var res = new ViewAsPdf("PdfContainer", output.PdfPOContainer)
    //        {
    //            FileName = fileName,
    //            PageSize = Size.A4,
    //            PageOrientation = Orientation.Portrait,
    //            PageMargins = { Left = 3, Right = 3, Bottom = 2, Top = 0 },
    //            SaveOnServerPath = filePath,
    //            // CustomSwitches = string.Format("--username \"{0}\" --password \"{1}\"" , "administrator", ".Wheel Lock.")
    //        };

    //        var binary = res.BuildPdf(this.ControllerContext);
    //        return File(binary, "application/pdf");
    //        //return RedirectToAction("Dashboard", "Home");
    //    }

    //    public ActionResult SendEmail()
    //    {
    //       // var output = new Output(312);
    //       // var model = output.GeneratePDFPO();
    //       //// var fileName = "PO"+model.PONumber+".pdf";
    //       ////// var filePath = Path.Combine("E:\\WWWApps\\EPOv2\\ElectronicPO\\", fileName);
    //       //// var filePath = Path.Combine("E:\\", fileName);
    //       //// var res = new ViewAsPdf("pdfReport", model)
    //       //// {
    //       ////     FileName = fileName,
    //       ////     PageSize = Size.A4,
    //       ////     PageOrientation = Orientation.Portrait,
    //       ////     PageMargins = { Left = 3, Right = 3, Bottom = 2, Top = 0 },
    //       ////     SaveOnServerPath = filePath
    //       //// };
    //       //// var binary = res.BuildPdf(this.ControllerContext);
    //       // var filePath = output.CreatePDFFile(model, this.ControllerContext);
    //       // output.SendEmail(filePath);

    //        var order = data.GetOrder(312);
    //        var route = new Routing(this.ControllerContext);
    //        route.SendEmail(order);
    //        return RedirectToAction("Dashboard", "Home");
    //    }

    //    public ActionResult OpenPDF()
    //    {
    //        var output = new Output(586);
    //        output.CreatePDFContainer();
    //        //var model = output.GeneratePDFPO();
    //        var fileName = "testPDf1.pdf";
    //        //var filePath = Path.Combine("E:\\", fileName);
    //        var binary = output.GeneratePOPDFFile(fileName, output.PdfPOContainer, ControllerContext);
    //        return File(binary, "application/pdf");
    //    }

    //    [HttpGet]
    //    public ActionResult FindUserGroups()
    //    {
    //       // var list = ad.TryToFindUserGroups("Ehall");
    //        return this.View(new List<string>());
    //    }

    //    [HttpPost]
    //    public ActionResult FindUserGroups(string UserName)
    //    {
    //        var list = ad.TryToFindUserGroups(UserName);
    //        return this.View(list.Select(x=>x.Name).ToList());
    //    }

    //    public ActionResult Watermark()
    //    {
    //        var output = new Output();
    //        output.MakeWatermarkOnPdf();
    //        return RedirectToAction("Index", "Home");
    //    }


    //    protected override void Dispose(bool disposing)
    //    {
    //        if (disposing)
    //        {
    //            dbOld.Dispose();
    //            db.Dispose();
    //        }
    //        base.Dispose(disposing);
    //    }

    //    public ActionResult TestValidation()
    //    {
    //       // var model = new VoucherAttachingFormViewModel();
    //        return this.View();
    //    }

    //    public ActionResult partialTestValidation()
    //    {
    //        var model = new VoucherAttachingFormViewModel();
    //        return this.PartialView("partialTestValidation",model );
    //    }

    //    public ActionResult TestValidation2()
    //    {
    //        // var model = new VoucherAttachingFormViewModel();
    //        return this.View();
    //    }

    //    public ActionResult partialTestValidation2()
    //    {
    //        var model = new VoucherAttachingFormViewModel();
    //        return this.PartialView("partialTestValidation", model);
    //    }

    //    public ActionResult CopyPDFPage()
    //    {
    //        var output = new Output();
    //        output.SaveInvoiceFilettTest();
    //        return RedirectToAction("Index", "Home");
    //    }
    }
}
