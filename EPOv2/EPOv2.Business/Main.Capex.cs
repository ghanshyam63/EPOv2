namespace EPOv2.Business
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;

    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.Business.Properties;
    using EPOv2.ViewModels;

    using PreMailer.Net;

    public partial class Main
    {
        //public static readonly string capexFilesPath = System.Web.HttpContext.Current.Server.MapPath("~/CapexFiles");
        //public const string capexFilesPath = @"\\dnas01\\EPO\Capex";
        public static int CapexStartNumber { get; } = 1100;
        //TODO should be calculated
        private static readonly int CapexFinancialYearPrefix = 1819;

        public Capex SaveCapex(CapexCRUDViewModel model)
        {
            var capex = _capexRepository.Get(x => x.CapexNumber == model.CapexNumber && x.Id == model.Id).FirstOrDefault();
            try
            {
               if (capex != null)
                {
                    if (capex.Entity.Id != model.EntityId && model.EntityId != 0) capex.Entity = this._entityRepository.Find(model.EntityId);
                    if (capex.CostCentre.Id != model.CostCentreId && model.CostCentreId != 0) capex.CostCentre = this._costCentreRepository.Find(model.CostCentreId);
                    if (capex.Owner.Id != model.OwnerId && !string.IsNullOrEmpty(model.OwnerId)) capex.Owner = this._userRepository.Find(model.OwnerId);
                    capex.Status = this._statusRepository.Get(x => x.Name == StatusEnum.Pending.ToString()).FirstOrDefault();
                    capex.LastModifiedBy = this._curUser;
                    capex.LastModifiedDate = DateTime.Now;
                    capex.TotalExGST = model.TotalExGST;
                    capex.Title = model.Title;
                    capex.Description = model.Description;
                    capex.RevisionQty++;
                    if (model.SelectedFilePath != null)
                    {
                        var fileToDelete = Path.Combine(Data.capexFilesPath, Path.GetFileName(capex.Reference));
                        File.Delete(fileToDelete);
                        capex.Reference = capex.CapexNumber + "_" + GetCapexFileName(model);
                        //var newFile = Path.Combine(capexFilesPath, Path.GetFileName(capex.Reference));
                        //model.SelectedFilePath.SaveAs(newFile);
                    }
                }
                else
                {
                    capex = new Capex
                    {
                        Author = _userRepository.Get(x => x.UserName == _curUser).FirstOrDefault(),
                        CostCentre = _costCentreRepository.Find(model.CostCentreId),
                        CreatedBy = _curUser,
                        LastModifiedBy = _curUser,
                        DateCreated = DateTime.Now,
                        LastModifiedDate = DateTime.Now,
                        Owner = _userRepository.Find(model.OwnerId),
                        Status =
                                        _statusRepository.Get(x => x.Name == StatusEnum.Pending.ToString()).FirstOrDefault(),
                        Title = model.Title,
                        Description = model.Description,
                        Entity = _entityRepository.Find(model.EntityId),
                        TotalExGST = model.TotalExGST,
                        CapexType = model.CapexType,
                        CapexNumber = GetNewCapexNumber()
                    };
                    capex.Reference = capex.CapexNumber + "_" + GetCapexFileName(model);
                    _capexRepository.Add(capex);

                }
                if (capex.Reference != null)
                {
                    var file = Path.Combine(Data.capexFilesPath, Path.GetFileName(capex.Reference));
                    model.SelectedFilePath?.SaveAs(file);
                }
                this.db.SaveChanges();

            }
            catch (Exception e)
            {
                LogError("Main.Capex.SaveCapex",e);
            }
            return capex;
        }

        private static string GetCapexFileName(CapexCRUDViewModel model)
        {
            var fName = "";
            if (model.SelectedFilePath.FileName.LastIndexOf("\\", StringComparison.Ordinal) > 0)
                fName =
                    model.SelectedFilePath.FileName.Substring(
                        model.SelectedFilePath.FileName.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            else fName = model.SelectedFilePath.FileName;
            return fName;
        }

        private string GetNewCapexNumber()
        {
            var number = this._capexRepository.Get().Select(x => x.CapexNumber).Max();
            //number = CapexFinancialYearPrefix.ToString() + "-" + CapexStartNumber; //Initialisation
            number = (Convert.ToInt32(number.Replace("-", "")) + 1).ToString().Replace(number.Substring(0,4),CapexFinancialYearPrefix.ToString()).Insert(4, "-");
            return number;
        }

        public void DeleteCapex(int capexId)
        {
            try
            {
                var capex = this._capexRepository.Find(capexId);
                if (capex == null){return;}
                capex.IsDeleted = true;
                capex.LastModifiedBy = this._curUser;
                capex.LastModifiedDate=DateTime.Now;
                this.DeleteExistingCapexRouting(capex);
                this.db.SaveChanges();
            }
            catch (Exception e)
            {
                LogError("Main.Capex.DeleteCapex(capexId="+capexId+")",e);
            }
        }

        public void StartRouting(Capex capex)
        {
            var approverList = this.GetApproversListForCapexRouting(capex);
            var i = 1;

            foreach (var approver in approverList)
            {
                var exRoute =
                    this._capexRouteRepository.Get(
                        x => x.Approver.Id == approver.Id && x.Capex.Id == capex.Id && x.Number == i).FirstOrDefault();
                if (exRoute != null)
                {
                    exRoute.IsDeleted = false;
                    exRoute.LastModifiedBy = _curUser;
                    exRoute.LastModifiedDate = DateTime.Now;
                }
                else
                {
                    var route = new CapexRoute()
                                    {
                                        Approver = approver,
                                        Capex = capex,
                                        LastModifiedBy = _curUser,
                                        CreatedBy = _curUser,
                                        DateCreated = DateTime.Now,
                                        LastModifiedDate = DateTime.Now,
                                        Number = i,
                                    };

                    _capexRouteRepository.Add(route);
                }
                i++;
            }
            db.SaveChanges();
        }

        private List<CapexApprover> GetApproversListForCapexRouting(Capex capex)
        {
            var approversList =this._capexApproverRepository.Get(x => !x.IsDeleted).OrderBy(x => x.Level).ToList();

            var list =approversList.GroupBy(x => new { x.Limit, x.Level })
                    .Select(x => new { x.Key.Level, x.Key.Limit })
                    .OrderBy(x => x.Level).ToList();

            var maxLimit = (from ll in list where ll.Limit > capex.TotalExGST select ll.Limit).FirstOrDefault();
            var level = list.Where(x => x.Limit == maxLimit).Max(x => x.Level);
            return approversList.Where(x => x.Level <= level && (((x.Division?.CostCentreRangeFrom<=capex.CostCentre.Code && x.Division?.CostCentreRangeTo >= capex.CostCentre.Code))|| (x.Division==null))).ToList();
        }

        public void DeleteExistingCapexRouting(Capex capex)
        {
            var list = this._capexRouteRepository.Get(x => x.Capex.Id == capex.Id).ToList();
            foreach (var route in list)
            {
                this._capexRouteRepository.Delete(route);
            }
            db.SaveChanges();
        }

        public List<CapexTransaction> GetCapexTransactions(int CapexId)
        {
            var i = 1;
            var order = this._capexRepository.Find(CapexId);
            var user = Ad.GetUser(order.CreatedBy);
            var transactionList = new List<CapexTransaction>();
            var transaction = new CapexTransaction()
            {
                Capex = i,
                Type = Transaction.TransactionType.Created.ToString(),
                Date = order.DateCreated.ToShortDateString() + " " + order.DateCreated.ToShortTimeString(),
                User = user.UserInfo.FirstName + " " + user.UserInfo.LastName,
                Description = string.Empty
            };
            transactionList.Add(transaction);

            var routelist = this._capexRouteRepository.Get(x => x.Capex.Id == CapexId ).OrderBy(x => x.Number).ToList();
            if (routelist.Count >= 0)
            {
                i++;
                var tr = new CapexTransaction();
                tr.Type = Transaction.TransactionType.Route.ToString();
                tr.Capex = i;
                foreach (var route in routelist)
                {
                    user = this._userRepository.Find(route.Approver.User.Id);

                    tr.Description += user.UserInfo.FirstName + " " + user.UserInfo.LastName;
                    if (route.Number < routelist.Count) tr.Description += " -> ";
                }
                transactionList.Add(tr);
            }

            if (order.Status.Name != StatusEnum.Draft.ToString())
            {
                var approverList = routelist.Where(x => x.IsDeleted).OrderBy(x => x.Number).ToList();
                foreach (var approver in approverList)
                {
                    i++;
                    user = Ad.GetUser(approver.Approver.User.UserName);
                    var tr = new CapexTransaction()
                    {
                        Type = Transaction.TransactionType.Approved.ToString(),
                        User = user.UserInfo.FirstName + " " + user.UserInfo.LastName,
                        Capex = i,
                        Date =
                                         approver.LastModifiedDate.ToShortDateString() + " "
                                         + approver.LastModifiedDate.ToShortTimeString(),
                        Description = string.Empty
                    };
                    transactionList.Add(tr);
                }


            }
            return transactionList;
        }

        public void SendCapexApproveNotification(int capexId, Controller controller)
        {
            var capex = _capexRepository.Find(capexId);
            var route = _capexRouteRepository.Get(x =>!x.IsDeleted && x.Capex.Id == capex.Id).OrderBy(x => x.Number).FirstOrDefault();

            if (route != null)
            {
                var model = new List<CapexDashboardViewModel>();
                var item = new CapexDashboardViewModel()
                {
                    DateCreated = capex.DateCreated.ToShortDateString(),
                    Status = capex.Status.Name,
                    CapexNumber = capex.CapexNumber,
                    Description = capex.Description,
                    Title = capex.Title,
                    TotalExGST = capex.TotalExGST,
                    CapexType = capex.CapexType,
                    CostCentre =
                                           capex.CostCentre.Code + " - " + capex.CostCentre.Name,
                    Entity = capex.Entity.Code + " - " + capex.Entity.Name,
                    Id = capex.Id,
                    Owner =capex.Owner.UserInfo.FirstName + " "+ capex.Owner.UserInfo.LastName,
                    URL = Settings.Default.CapexApproveURL + capexId
                };
                model.Add(item);
                var user = route.Approver.User;
                var body = RenderPartialViewToString(controller, "_CapexNotificationReport", model);
                var bodyText = PreMailer.MoveCssInline(body).Html;
                var subject = "Notification: Capex for Approve";
                _output.SendCapexNotification(user, bodyText, subject);
            }
        }

        public void SendCapexNotificationToAuthor(int capexId, Controller capexController)
        {
            var capex = _capexRepository.Find(capexId);
            var subject = string.Empty;
            if (capex != null)
            {
                var route =
                    _capexRouteRepository.Get(x => !x.IsDeleted && x.Capex.Id == capex.Id)
                        .OrderBy(x => x.Number)
                        .FirstOrDefault();
                var bodyText = "Your Capex #" + capex.CapexNumber;
                if (capex.Status.Name == StatusEnum.Declined.ToString())
                {
                    bodyText += " declined by " + this.Data._ad.GetADNamebyLogin(capex.LastModifiedBy) + ". Comment: "
                                + capex.Comment + ".";
                    subject = "Capex #" + capex.CapexNumber + " declined!";
                }
                else if (route != null)
                {
                    bodyText += " waiting for approve by " + route.Approver.User.GetFullName() + ".";
                    subject = "Capex #" + capex.CapexNumber + " notification.";
                }
                else
                {
                    bodyText += " approved!";
                    subject = "Capex #" + capex.CapexNumber + " notification.";
                }
                var user = capex.Author;
                _output.SendCapexNotification(user, bodyText, subject);
            }
        }



        public List<SearchCapexResult> SearchCapex(SearchViewModel model)
        {
            var capexList = new List<Capex>();
            //var searchResult = new SearchEPOResult();
            var searchCapexResultList = new List<SearchCapexResult>();
            if (!string.IsNullOrEmpty(model.CapexNumber)) capexList = this.FilterCapexByCapexNumber(model.CapexNumber);
            if (!string.IsNullOrEmpty(model.DateFrom) || !string.IsNullOrEmpty(model.DateTo)) capexList = this.FilterCapexByDates(model.DateFrom, model.DateTo, capexList);
            if (model.SelectedEntity != 0) capexList = this.FilterCapexByEntity(model.SelectedEntity, capexList);
            if (model.SelectedCostCenter != 0) capexList = this.FilterCapexByCostCentre(model.SelectedCostCenter, capexList);
            if (!string.IsNullOrEmpty(model.SelectedAuthor)) capexList = this.FilterCapexByAuthor(model.SelectedAuthor, capexList);
            if (!string.IsNullOrEmpty(model.SelectedOwner)) capexList = this.FilterCapexByOwner(model.SelectedOwner, capexList);
            if (model.SelectedStatus != 0) capexList = this.FilterCapexByStatus(model.SelectedStatus, capexList);
            if (!string.IsNullOrEmpty(model.Description)) capexList = this.FilterCapexByDescription(model.Description, capexList);
            
            searchCapexResultList = ConvertToSearchCapexResult(capexList);
            
            return searchCapexResultList;
        }

       

        public static List<SearchCapexResult> ConvertToSearchCapexResult(List<Capex> capexList)
        {
            var model= new List<SearchCapexResult>();
            foreach (var capex in capexList)
            {
                var item = new SearchCapexResult()
                               {
                                   Title = capex.Title,
                                   Total = capex.TotalExGST,
                                   Date = capex.DateCreated.ToShortDateString(),
                                   isEditLocked = true,
                                   CapexNumber = capex.CapexNumber,
                                   Status = capex.Status.Name,
                                   Author = capex.Author.GetFullName(),
                                   Owner = capex.Owner.GetFullName(),
                                   Entity = capex.Entity.GetFullName(),
                                   CostCentre = capex.CostCentre.GetFullName(),
                                   Id = capex.Id.ToString()
                               };
                model.Add(item);
            }
            return model;
        }


        private List<Capex> FilterCapexByDescription(string description, List<Capex> capexList)
        {
            capexList = capexList.Count == 0
                          ? this._capexRepository.Get(x => x.Description.Contains(description) && !x.IsDeleted).ToList()
                          : capexList.Where(x =>x.Description.Contains(description)).ToList();
            return capexList;
        }

        private List<Capex> FilterCapexByStatus(int selectedStatus, List<Capex> capexList)
        {
            capexList = capexList.Count == 0
                           ? this._capexRepository.Get(x => x.Status.Id == selectedStatus && !x.IsDeleted).ToList()
                           : capexList.Where(x => x.Status != null && x.Status.Id == selectedStatus).ToList();
            return capexList;
        }

        private List<Capex> FilterCapexByOwner(string selectedOwner, List<Capex> capexList)
        {
            capexList = capexList.Count == 0
                            ? this._capexRepository.Get(x => x.Owner.Id == selectedOwner && !x.IsDeleted).ToList()
                            : capexList.Where(x => x.Owner != null && x.Owner.Id == selectedOwner).ToList();
            return capexList;
        }

        private List<Capex> FilterCapexByAuthor(string selectedAuthor, List<Capex> capexList)
        {
            capexList = capexList.Count == 0
                            ? this._capexRepository.Get(x => x.Author.Id == selectedAuthor && !x.IsDeleted).ToList()
                            : capexList.Where(x => x.Author != null && x.Author.Id == selectedAuthor).ToList();
            return capexList;
        }

        private List<Capex> FilterCapexByCostCentre(int selectedCostCenter, List<Capex> capexList)
        {
            capexList = capexList.Count == 0
                            ? this._capexRepository.Get(x => x.CostCentre.Id == selectedCostCenter && !x.IsDeleted).ToList()
                            : capexList.Where(x => x.CostCentre != null && x.CostCentre.Id == selectedCostCenter).ToList();
            return capexList;
        }

        private List<Capex> FilterCapexByEntity(int selectedEntity, List<Capex> capexList)
        {
            capexList = capexList.Count == 0
                            ? this._capexRepository.Get(x => x.Entity.Id == selectedEntity && !x.IsDeleted).ToList()
                            : capexList.Where(x => x.Entity != null && x.Entity.Id == selectedEntity).ToList();
            return capexList;
        }

        private List<Capex> FilterCapexByDates(string dateFrom, string dateTo, List<Capex> capexList)
        {
            if (capexList.Count == 0)
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    var dateF = Convert.ToDateTime(dateFrom);
                    var dateT = Convert.ToDateTime(dateTo);
                    capexList =
                        this._capexRepository.Get(x => x.DateCreated >= dateF && x.DateCreated <= dateT && !x.IsDeleted).ToList();
                }
                else if (!string.IsNullOrEmpty(dateFrom))
                {
                    var dateF = Convert.ToDateTime(dateFrom);
                    capexList =
                       this._capexRepository.Get(x => x.DateCreated >= dateF && !x.IsDeleted).ToList();
                }
                else if (!string.IsNullOrEmpty(dateTo))
                {
                    var dateT = Convert.ToDateTime(dateTo);
                    capexList =
                        this._capexRepository.Get(x => x.DateCreated <= dateT && !x.IsDeleted).ToList();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    var dateF = Convert.ToDateTime(dateFrom);
                    var dateT = Convert.ToDateTime(dateTo);
                    capexList =
                        capexList.Where(x => x.DateCreated >= dateF && x.DateCreated <= dateT && !x.IsDeleted).ToList();
                }
                else if (!string.IsNullOrEmpty(dateFrom))
                {
                    var dateF = Convert.ToDateTime(dateFrom);
                    capexList =
                       capexList.Where(x => x.DateCreated >= dateF && !x.IsDeleted).ToList();
                }
                else if (!string.IsNullOrEmpty(dateTo))
                {
                    var dateT = Convert.ToDateTime(dateTo);
                    capexList =
                        capexList.Where(x => x.DateCreated <= dateT && !x.IsDeleted).ToList();
                }
            }
            return capexList;
        }

        private List<Capex> FilterCapexByCapexNumber(string capexNumber)
        {
            var list = this._capexRepository.Get(x => x.CapexNumber == capexNumber).ToList();
            return list;
        }
    }
}
