using System;
using System.Web.Mvc;

namespace EPOv2.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using EPOv2.Business.Interfaces;
    using EPOv2.ViewModels;

    public class AdministratorController : Controller
    {
        private readonly IRouting _routing;

        private readonly IAd _ad;

        public AdministratorController(IRouting routing, IAd ad)
        {
            this._routing = routing;
            _ad = ad;
        }

        // GET: Administrator
        public ActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //public ActionResult FakeRouting()
        //{
        //    var fakeRouting = this._routing.FakeRoutingInit();
        //    return this.View(fakeRouting);
        //}

        [HttpGet]
        public Action LoadFakeRouting()
        {
            //var fakeRoungingList = routing.GetExistingFakeRouting();
            //return this.PartialView(fakeRoungingList);
            return null;
        }


        //[HttpPost]
        //public ActionResult FakeRouting(FakeRouting fakeRouting)
        //{
        //    this._routing.PushFakeRouting(fakeRouting);
        //    var fakeRout = this._routing.FakeRoutingInit();
        //    return this.View(fakeRout);
        //}

        [HttpGet]
        public ActionResult FindUserGroups()
        {
            return this.View(new List<string>());
        }

        [HttpPost]
        public ActionResult FindUserGroups(string userName)
        {
            var list = _ad.TryToFindUserGroups(userName);
            return this.View(list.Select(x => x.Name).ToList());
        }
    }
}