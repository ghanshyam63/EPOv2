using System.Web.Mvc;

namespace EPOv2.Controllers
{
    public class ManageRoutingController : Controller
    {
       // #region Property

//        //private readonly PurchaseOrderContext _db = new PurchaseOrderContext();
//        private readonly PurchaseOrderOldContext db = new PurchaseOrderOldContext();
///*
//        private readonly cUtilities _cUtil = new cUtilities();
//*/
///*
//        private readonly Directory _adsi = new Directory();
//*/
//        private readonly Routing _routing = new Routing();

//        public PurchaseOrderOldContext Db
//        {
//            get { return this.db; }
//        }

//        public PurchaseOrderOldContext Db1
//        {
//            get { return this.db; }
//        }

//        //code list: 
//        // 7XX -Error
//        //701 - has Cost Centre as owner
//        // 2XX -SUCCESS
//        // 200 - OK

//        #endregion
        
//        // GET: /ManageRouting/AddUser
//        public async Task<ActionResult> AddUser(string result =null, string runtime = null)
//        {
//            var dInfo = new DInfo();
//            dInfo.StartWatch();
//            //var adLst = _routing.GetAllAdUsersName();
//            var usrLst = _routing.GetAllADUsersName();//GetUserLst();
//            var mngLst = await _routing.GetManagerLst();
//            var lvlLst = _routing.GetLevelSelectList();
//            ViewBag.userLst = new SelectList(usrLst);
//            ViewBag.mngLst = new SelectList(mngLst);
//            ViewBag.lvlLst = lvlLst;
//            ViewBag.runtime1 = "DDL runtime: " + dInfo.Stopwatch();
//            ViewBag.runtime2 = "Saveing runtime" + runtime;
//            ViewBag.Result = result;
//            ViewBag.name = _routing.GetADName();

//            return View();
//        }

//        // POST: /ManageRouting/AddUser
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> AddUser(tblStagingEmployeesDEV stagingEmployee)
//        {
//            if (!ModelState.IsValid) return View();
//            var debugInfo = new DInfo();
//            debugInfo.StartWatch();
//            var sEmployee =
//                this.db.tblStagingEmployeesDEVs.FirstOrDefault(
//                    x => x.Employee == stagingEmployee.Employee && x.Manager == stagingEmployee.Manager);
//            if (sEmployee == null)
//            {
//                this.db.tblStagingEmployeesDEVs.Add(stagingEmployee);
                
//            }
//            else
//            {
//                sEmployee.isDeleted = 0;
//                sEmployee.Level = stagingEmployee.Level;
//            }
            
//            await this.db.SaveChangesAsync();
//            // Add/Update routing
//            _routing.PushRouting(stagingEmployee.Manager,stagingEmployee.Employee, stagingEmployee.Level);
            
//            var elapsedTime = debugInfo.Stopwatch();
//            Debug.WriteLine("KOGOT = AddUser-RunTime: "+elapsedTime);
//            return RedirectToAction("AddUser", "ManageRouting", new { runtime = elapsedTime, result = "Done" });
//        }

//        //public async Task<ActionResult> FlushRouting()
//        //{
//        //    return View();
//        //}
        
//        // GET: /ManageRouting/ManageUsers
//        public async Task<ActionResult> ManageUsers(string code =null)
//        {
//            var empList = await this.db.tblStagingEmployeesDEVs.Where(x => x.isDeleted == 0).OrderBy(x=>x.Employee).ToListAsync();
//            //TODO: make it as Dictionary!!!
//            if (code != null)
//            {
//                if (code.StartsWith("2"))
//                {
//                    ViewBag.Result = "Done";
//                    ViewBag.AlertClass = "alert-success";
//                    ViewBag.Msg = "Done";
//                }
//                else
//                {
//                    ViewBag.Result = "Error";
//                    ViewBag.Msg = "This employee own of Cost Centre, please change owner before deleting";
//                    ViewBag.AlertClass = "alert-danger";
//                }
//            }

//            ViewBag.code = code;
//            return View(empList);
//            //return View();
//        }

//        public async Task<ActionResult> ManageUsersEdit(int? id)
//        {
//            if (id == null) return RedirectToAction("ManageUsers");
//            var emp =this.db.tblStagingEmployeesDEVs.FirstOrDefault(x => x.StagingEmpID == id);
//            var mngLst = await _routing.GetManagerLst();
//            if (emp != null)
//            {
//                var selectedMng = mngLst.Where(x => x.Contains(emp.Manager));
//                ViewBag.mngLst = new SelectList(mngLst, selectedMng);
//                var lvlLst = _routing.GetLevelSelectList(emp.Level);
//                ViewBag.lvlLst = lvlLst;
//            }
//            else
//            {
//                ViewBag.mngLst = new SelectList(mngLst);
//                var lvlLst = _routing.GetLevelSelectList();
//                ViewBag.lvlLst = lvlLst;
//            }
            
//            return PartialView("ManageUsersEdit",emp);
//        }

//        //POST
//        [HttpPost]
//        public async Task<ActionResult> ManageUsersEdit(tblStagingEmployeesDEV stagingEmployee)
//        {
//            if (!ModelState.IsValid) return View();
//            this.db.Entry(stagingEmployee).State = EntityState.Modified;
//            await this.db.SaveChangesAsync();
//            _routing.RemoveNotRelevantRouting(stagingEmployee);
//           _routing.PushRouting(stagingEmployee.Manager, stagingEmployee.Employee, stagingEmployee.Level);
//            return RedirectToAction("ManageUsers");
//        }

//        //GET
//        public async Task<ActionResult> ReplaceManager(string runtime =null,string result=null, string par=null)
//        {
//            var dInfo = new DInfo();
//            dInfo.StartWatch();
            
//            var oldManagerLst = await _routing.GetManagerLst();
//            //var newManagerLst = await _routing.GetManagerLst();
//            ViewBag.oldMngLst = new SelectList(oldManagerLst);
//            ViewBag.newMngLst = new SelectList(oldManagerLst);
//            ViewBag.runtime1 = "DDL runtime: " + dInfo.Stopwatch();
//            ViewBag.runtime2 = "Saveing runtime" + runtime;

//            ViewBag.Result = result;
//            return View();
//        }

//        //POST
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ReplaceManager(string oldMng, string newMng)
//        {
//            var dInfo = new DInfo();
//            dInfo.StartWatch();
//            var result = "Done";

//            var oldMngList = this.db.tblStagingEmployeesDEVs.Where(x => x.Manager == oldMng).ToList();
//            foreach (var employee in oldMngList)
//            {
//                this.db.Entry(employee).State = EntityState.Modified;
//                employee.Manager = newMng;
//            }
//            await this.db.SaveChangesAsync();
//            var stagingCostCentre = this.db.tblStagingCostCentresDEVs.FirstOrDefault(x => x.Owner == newMng);
//            if (stagingCostCentre != null) await UpdateCostCentreOwner(stagingCostCentre);
//            else result = "That manager does not own Cost Centre";
//            var elapsedTime = dInfo.Stopwatch(); 
//            return RedirectToAction("ReplaceManager", "ManageRouting", new { runtime = elapsedTime, result });
//        }

//        //GET  /ManageRouting/ChangeOwner
//        public async Task<ActionResult> ChangeOwner(string runtime =null,string result=null, int cc=-1)
//        {
//            var dInfo = new DInfo();
//            dInfo.StartWatch();
//            //if (cc == -1)
//            //{
//                //var usrLst = _routing.GetAllADUsersName();//GetUserLst();
//                var ownerLst = await _routing.GetManagerLst();
//                //var lvlLst = _routing.GetLevelSelectLst(this);
//                //var selectedOwner = 
//                ViewBag.ownerLst = new SelectList(ownerLst);
//                ViewBag.costCentreLst = _routing.GetCostCentreList();

//           // }
//            //else
//            //{
//            //    var selectedCC = _db.tblStagingCostCentresDEVs.FirstOrDefault(x => x.CostCentreCode==cc);
//            //    ViewBag.costCentreLst = _routing.GetCostCentreList(selectedCC);
//            //    var ownerLst = await _routing.GetManagerLst();
//            //    var selectedOwner =
//            //        _db.tblStagingCostCentresDEVs.Where(x => x.StagingCostCentreID == selectedCC.StagingCostCentreID)
//            //            .Select(x => x.Owner).FirstOrDefault();
//            //    ViewBag.ownerLst = new SelectList(ownerLst,selectedOwner);
//            //}
//            ViewBag.runtime1 = "DDL runtime: " + dInfo.Stopwatch();
//            ViewBag.runtime2 = "Saveing runtime" + runtime;
//            ViewBag.Result = result;
            
//            return View();
//        }

//        //Post: /ManageRouting/ChangeOwner
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> ChangeOwner(tblStagingCostCentresDEV stagingCostCentre)
//        {
//            //if (stagingCostCentre.Owner == null)
//            //  return  RedirectToAction("ChangeOwner", "ManageRouting", new {cc = stagingCostCentre.CostCentreCode});
//            if (!ModelState.IsValid) return View();
//            var debugInfo = new DInfo();
//            var result = "Done";
//            debugInfo.StartWatch();
//            //var e = _routing.GetEmployeeChainList(stagingCostCentre.Owner,Routing.Direction.Up);
//            try
//            {
//                await UpdateCostCentreOwner(stagingCostCentre);
//            }
//            catch (Exception e)
//            {
//                Debug.WriteLine(e.Message);
//                result = e.Message;
//            }

//            var elapsedTime = debugInfo.Stopwatch();
//            return RedirectToAction("ChangeOwner", "ManageRouting", new {runtime=elapsedTime, result});
//        }

//        private async Task UpdateCostCentreOwner(tblStagingCostCentresDEV stagingCostCentre)
//        {
//            stagingCostCentre.EntityID = Routing.GetEntityIDbyBL(stagingCostCentre.CostCentreCode.ToString(CultureInfo.InvariantCulture));
//            stagingCostCentre.tblEntity =
//                this.db.tblEntities.FirstOrDefault(x => x.EntityID == stagingCostCentre.EntityID);

//            var sStagingCC =
//                this.db.tblStagingCostCentresDEVs.FirstOrDefault(x => x.CostCentreCode == stagingCostCentre.CostCentreCode);

//            if (sStagingCC == null)
//            {
//                this.db.tblStagingCostCentresDEVs.Add(stagingCostCentre);
//            }
//            else
//            {
//                this.db.Entry(sStagingCC).State = EntityState.Modified;
//                //stagingCostCentre.EntityID = sStagingCC.EntityID;
//                sStagingCC.Owner = stagingCostCentre.Owner;
//            }
//            await this.db.SaveChangesAsync();
//            _routing.RemoveOldRouting(sStagingCC);
//            {
//                var level =
//                    this.db.tblStagingEmployeesDEVs.Where(x => x.Employee == sStagingCC.Owner)
//                        .Select(x => x.Level)
//                        .FirstOrDefault();
//                if (sStagingCC != null)
//                {
//                    _routing.PushRouting(sStagingCC.Owner, null, level, 2, sStagingCC);
//                    _routing.PushRouting(sStagingCC.Owner, null, level, 1, sStagingCC);
//                }
//            }
//        }
        
//        //GET
//        public ActionResult MapAccToCC(string result = null, string runtime = null)
//        {
//            var accLst = _routing.GetAccountList();
//            var ccLst = _routing.GetCostCentreList(true);
//            ViewBag.accLst = accLst;
//            ViewBag.CCLst = ccLst;
//            if (result != null)
//            {
//                ViewBag.result = result;
//                ViewBag.runtime = runtime;
//            }
//            return View();
//        }
//        //POST
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//        public async Task<ActionResult> MapAccToCC(List<string> AccountCodes, string CostCenteCode)
//         {
//             var debugInfo = new DInfo();
//             debugInfo.StartWatch();
//             var stagingCostCentre = _routing.AddStagingCostCentreAsync(Convert.ToInt32(CostCenteCode));
//             foreach (var accountCode in AccountCodes)
//             {
//                 var stagingAccCC =
//                     this.db.tblStagingAccCCs.FirstOrDefault(
//                         x => x.StagingCostCentreID == stagingCostCentre.Result.StagingCostCentreID &&
//                              x.AccountCode == accountCode && x.isDeleted == false);
//                 if (stagingAccCC == null)
//                 {
//                     stagingAccCC = new tblStagingAccCC
//                     {
//                         //tblStagingCostCentresDEV = stagingCostCentre.Result,
//                         Active = true,
//                         StagingCostCentreID = stagingCostCentre.Result.StagingCostCentreID,
//                         AccountCode = accountCode,
//                         Created = DateTime.Now,
//                         Updated = DateTime.Now,
//                         isDeleted = false,
//                         StagingAccCCID = Guid.NewGuid()
//                     };
//                     this.db.tblStagingAccCCs.Add(stagingAccCC);
//                 }
//                 else
//                 {
//                     this.db.Entry(stagingAccCC).State = EntityState.Modified;
//                     stagingAccCC.Updated = DateTime.Now;
//                     stagingAccCC.Active = true;
//                 }
                 
//             }
//             this.db.SaveChanges();
//             _routing.PushRouting(stagingCostCentre.Result, AccountCodes);
//             var elapsedTime = debugInfo.Stopwatch();
//             Debug.WriteLine("KOGOT = AddUser-RunTime: " + elapsedTime);
//             return RedirectToAction("MapAccToCC", "ManageRouting",new { result= "Done", runtime = elapsedTime});
//        }

//         // GET: /ManageRouting/ManageUsers/DeleteManageUser/5
//         public async Task<ActionResult> DeleteManageUser(int? id)
//         {
//             string resCode=null;
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            var stagingEmployee = await this.db.tblStagingEmployeesDEVs.FindAsync(id);
//            if (stagingEmployee == null)
//            {
//                return HttpNotFound();
//            }
//             var stagingCC =
//                 await
//                     this.db.tblStagingCostCentresDEVs.Where(x => x.Owner == stagingEmployee.Employee).FirstOrDefaultAsync();
//             if (stagingCC == null)
//             {
//                 if (await _routing.RemoveUser(stagingEmployee)) resCode = "200";
//                 else resCode = "700";
//             }
//             else return RedirectToAction("ManageUsers", new {code ="701"}); 
                
//            return RedirectToAction("ManageUsers", new {code =resCode});
//        }

//        #region Generatedcode
//        //----------------------GENERATED-----------------------------
//        // GET: /ManageRouting/
//        public async Task<ActionResult> Index()
//        {
//            var tblstagingaccccs = this.db.tblStagingAccCCs.Include(t => t.tblStagingCostCentresDEV);
//            return View(await tblstagingaccccs.ToListAsync());
//        }

//        // GET: /ManageRouting/Details/5
//        public async Task<ActionResult> Details(Guid? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            tblStagingAccCC tblstagingacccc = await this.db.tblStagingAccCCs.FindAsync(id);
//            if (tblstagingacccc == null)
//            {
//                return HttpNotFound();
//            }
//            return View(tblstagingacccc);
//        }

//        // GET: /ManageRouting/Create
//        public ActionResult Create()
//        {
//            ViewBag.StagingCostCentreID = new SelectList(this.db.tblStagingCostCentresDEVs, "StagingCostCentreID", "Description");
//            return View();
//        }

//        // POST: /ManageRouting/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Create([Bind(Include="StagingAccCCID,AccountCode,StagingCostCentreID,Active,isDeleted,Created,Updated")] tblStagingAccCC tblstagingacccc)
//        {
//            if (ModelState.IsValid)
//            {
//                tblstagingacccc.StagingAccCCID = Guid.NewGuid();
//                this.db.tblStagingAccCCs.Add(tblstagingacccc);
//                await this.db.SaveChangesAsync();
//                return RedirectToAction("Index");
//            }

//            ViewBag.StagingCostCentreID = new SelectList(this.db.tblStagingCostCentresDEVs, "StagingCostCentreID", "Description", tblstagingacccc.StagingCostCentreID);
//            return View(tblstagingacccc);
//        }

//        // GET: /ManageRouting/Edit/5
//        public async Task<ActionResult> Edit(Guid? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            tblStagingAccCC tblstagingacccc = await this.db.tblStagingAccCCs.FindAsync(id);
//            if (tblstagingacccc == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.StagingCostCentreID = new SelectList(this.db.tblStagingCostCentresDEVs, "StagingCostCentreID", "Description", tblstagingacccc.StagingCostCentreID);
//            return View(tblstagingacccc);
//        }

//        // POST: /ManageRouting/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Edit([Bind(Include="StagingAccCCID,AccountCode,StagingCostCentreID,Active,isDeleted,Created,Updated")] tblStagingAccCC tblstagingacccc)
//        {
//            if (ModelState.IsValid)
//            {
//                this.db.Entry(tblstagingacccc).State = EntityState.Modified;
//                await this.db.SaveChangesAsync();
//                return RedirectToAction("Index");
//            }
//            ViewBag.StagingCostCentreID = new SelectList(this.db.tblStagingCostCentresDEVs, "StagingCostCentreID", "Description", tblstagingacccc.StagingCostCentreID);
//            return View(tblstagingacccc);
//        }

//        // GET: /ManageRouting/Delete/5
//        public async Task<ActionResult> Delete(Guid? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            tblStagingAccCC tblstagingacccc = await this.db.tblStagingAccCCs.FindAsync(id);
//            if (tblstagingacccc == null)
//            {
//                return HttpNotFound();
//            }
//            return View(tblstagingacccc);
//        }

        

//        // POST: /ManageRouting/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> DeleteConfirmed(Guid id)
//        {
//            tblStagingAccCC tblstagingacccc = await this.db.tblStagingAccCCs.FindAsync(id);
//            this.db.tblStagingAccCCs.Remove(tblstagingacccc);
//            await this.db.SaveChangesAsync();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                this.db.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        #endregion

//        #region PopulateData
///*
//        private List<string> GetUserLst()
//        {
//            _adsi.Credentials = _cUtil.SetCredential();
//            var adsiUsers = _adsi.GetADInfo().Tables[0].AsEnumerable()
//                .Where(x => !String.IsNullOrWhiteSpace(x.Field<string>("Mail")) && !String.IsNullOrWhiteSpace(x.Field<string>("TelephoneNumber")))
//                .Select(x => x.Field<string>("cn"))
//                .ToList();

            
//            return adsiUsers;
//        }
//*/

//        #endregion
    }
}
