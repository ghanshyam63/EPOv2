namespace EPOv2.Business
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.Repositories.Interfaces;
    using EPOv2.ViewModels;

    public partial class Routing
    {
        private ControllerContext controllerContext;

        public Routing(ControllerContext controllerContext, ICostCentreRepository costCentreRepository)
        {
            this.controllerContext = controllerContext;
            this._costCentreRepository = costCentreRepository;
        }

        public void DeclineCapex(CapexCRUDViewModel model)
        {
           var capex=this.MakeCapexStatusDeclined(model);
           _main.DeleteExistingCapexRouting(capex);
        }



        public void ApproveCapex(CapexCRUDViewModel model)
        {
            try
            {
                var isLastApprover = false;
                var boardLevel = _capexApproverRepository.Get(x => !x.IsDeleted).Max(x => x.Level);
                var routes =
                    _capexRouteRepository.Get(
                        x => x.Capex.Id == model.Id && !x.IsDeleted && x.Approver.User.EmployeeId == CurEmpId)
                        .OrderBy(x => x.Number).ToList();
                if (routes.Where(x => x.Number != boardLevel).ToList().Count > 1) //if more than 1 and no Board Approvers do for
                {
                    foreach (var route in routes)
                    {
                        if (route == null) continue;
                        route.IsDeleted = true;
                        _capexRouteRepository.Update(route);
                    }
                }
                else 
                {
                    if (routes.Any()) 
                    {
                        var route = routes.OrderBy(x=>x.Number).First();// it's could be 2 routes, general level + board level, like level 3 and level 6(board), but we take 1(first). To approve on the time
                        route.IsDeleted = true;
                        _capexRouteRepository.Update(route);
                    } 
                    
                }

                Db.SaveChanges();
                isLastApprover = CheckIsLastCapexApprover(model);
                if (isLastApprover)
                {
                    MakeCapexStatusApproved(model);
                }
            }
            catch (Exception e)
            {
                _main.LogError("ApproveCapex(capexId="+model.Id+",user:"+_curUser+")",e);
            }
        }

        public void MakeCapexStatusApproved(CapexCRUDViewModel model)
        {
            try
            {
                var capex = _capexRepository.Find(model.Id);
                capex.Status = _data.GetStatus(StatusEnum.Approved);
                _capexRepository.Update(capex);
                Db.SaveChanges();
            }
            catch (Exception e)
            {
                _main.LogError("MakeCapexStatusApproved(capexId=" + model.Id + ",user:" + _curUser + ")", e);
            }
        }

        public Capex MakeCapexStatusDeclined(CapexCRUDViewModel model)
        {
            var capex = this._capexRepository.Find(model.Id);
            capex.Status = this._data.GetStatus(StatusEnum.Declined);
            capex.Comment = model.Comment;
            capex.LastModifiedDate = DateTime.Now;
            capex.LastModifiedBy = this._curUser;
            Db.SaveChanges();
            return capex;
        }

        public bool CheckIsLastCapexApprover(CapexCRUDViewModel model)
        {
            var routeCnt = _capexRouteRepository.Get(x => x.Capex.Id == model.Id && !x.IsDeleted).Count();
            return routeCnt == 0;
        }
    }
}
