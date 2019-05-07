using System;
using System.Collections.Generic;
using System.Linq;

namespace EPOv2.Business
{
    using System.Data.Entity;
    using System.Globalization;

    using DomainModel.BaseClasses;
    using DomainModel.DataContext;
    using DomainModel.Entities;
    using DomainModel.Enums;

    using EPOv2.Business.Interfaces;

    using Intranet.ViewModels;

    using Microsoft.VisualBasic;

    using Serilog;

    using Site = DomainModel.Enums.Site;

    public partial class Data
    {
       
        public const int OverdueHoursQty = 24;

        public Dictionary<int,string> EpoTileDict = new Dictionary<int,string>()
                                                    {
                                                        {1,"Orders to Approve"},
                                                        {2,"Orders to be Matched"},
                                                        {3,"Your Orders Awaiting Approve"},
                                                        {4,"Invoices to Approve"},
                                                        {5,"Capexes to Approve"}
                                                    };

        public Dictionary<int, string> TnaTileDict = new Dictionary<int, string>()
                                                    {
                                                        {1,"Employee exceptions"}
                                                       
                                                    };


        public EpoDashboardViewModel GetEPODashboardViewModel(int currentEmployeeId)
        {
			var model = new EpoDashboardViewModel() {EpoTiles = new List<EpoTile>()};
            foreach (var tileType in EpoTileDict.Keys)
            {
                model.EpoTiles.Add(GetEPOTile(tileType, currentEmployeeId));
                if (tileType != 2 && tileType != 3) model.NotificationQty += model.EpoTiles.Last().Value;
            }
            return model;
        }

        public DashboardNotificationDataViewModel GetEPODashboardNotificationData(int currentEmployeeId)
        {
            var model = new DashboardNotificationDataViewModel() { Tiles = new List<TileData>() };
            foreach (var tileType in EpoTileDict.Keys)
            {
                var epoTile = GetEPOTile(tileType, currentEmployeeId);
                model.Tiles.Add(new TileData() {TypeKey = tileType, isOverdue = epoTile.OverdueValue>0, Value = epoTile.Value,OverdueValue = epoTile.OverdueValue});
            }
            model.NotificationQty= model.Tiles.Where(x => x.TypeKey != 2 && x.TypeKey!= 3).Sum(x =>x.Value);
            return model;
        }

        public TnADashboardTiles GetTNADashboardViewModel(int currentEmployeeId)
        {
            var model = new TnADashboardTiles() { TnATiles = new List<TnATile>() };
            foreach (var tnaType in TnaTileDict.Keys)
            {
                var tnaTile = GetTnaTile(tnaType, currentEmployeeId);
                model.TnATiles.Add(tnaTile);
            }
            return model;
        }

      

        private TnATile GetTnaTile(int type, int employeeId)
        {
            TnATile model;
            switch (type)
            {
                case 1:
                    model = GetEmployeeExceptions(employeeId);
                    break;
                default: model = new TnATile();
                    break;
            }
            return model;
        }

        private TnATile GetTnaTilebySubType(TileSubType subType, int employeeId)
        {
            var model =new TnATile();

            try
            {
                switch (subType)
                {
                    case TileSubType.EE:
                        model = GetEmployeeExceptions(employeeId);
                        break;
                    default:
                        model = new TnATile();
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e,"Data.Dashboard.GetTnaTilebySubType(empId:{employeeId}, subType:{subType})", employeeId, subType);
            }
            return model;
        }

        private TnATile GetEmployeeExceptions(int employeeId)
        {
            var employeeName = _ad.CurrentUser?.GetFullName().ToUpper();
            var list =
                _oldTnaContext.EmployeeException.Where(x => x.Status == 0 && x.ManagerName.ToUpper() == employeeName)
                    .ToList();
            var model = new TnATile()
                            {
                                TypeKey = 1,
                                TileColor = "black-bg",
                                Header = "Emp Exceptions",
                                SortNumber = 1,
                                Value = list.Count,
                                TileIcon = "fa fa-street-view"
                            };
            return model;
        }

        public EpoTile GetEPOTile(int type, int employeeId)
        {
            var model = new EpoTile();

            try
            {
                switch (type)
                {
                    case 1:
                        model = GetOrdersForApproveTile(employeeId);
                        break;
                    case 2:
                        model = GetOrdersToMatchTile(employeeId);
                        break;
                    case 3:
                        model = GetYourOrdersAwaitingApproveTile(employeeId);
                        break;
                    case 4:
                        model = GetInvoiceForApproveTile(employeeId);
                        break;
                    case 5:
                        model = GetCapexForApproveTile(employeeId);
                        break;
                    default: model = new EpoTile();
                        break;

                }
            }
            catch (Exception e)
            {
                _logger.Error(e,"Data.Dashboard.GetEPOTile(empId:{employeeId}, type:{type})", employeeId, type);
            }
            return model;
        }

        public EpoTile GetEPOTilebySubType(TileSubType subType, int employeeId)
        {
            EpoTile model;
            switch (subType)
            {
                case TileSubType.OTA:
                    model = GetOrdersForApproveTile(employeeId);
                    break;
                case TileSubType.OTM:
                    model = GetOrdersToMatchTile(employeeId);
                    break;
                case TileSubType.MO:
                    model = GetYourOrdersAwaitingApproveTile(employeeId);
                    break;
                case TileSubType.ITA:
                    model = GetInvoiceForApproveTile(employeeId);
                    break;
                case TileSubType.CTA:
                    model = GetCapexForApproveTile(employeeId);
                    break;
                default:
                    model = new EpoTile();
                    break;
            }
            return model;
        }

       

        private EpoTile GetCapexForApproveTile(int employeeId)
        {
            var epoTile = new EpoTile() { Header = "Capex to Approve", TileColor = "navy-bg", TileIcon = "fa fa-briefcase", SortNumber = 5, TypeKey = 5, FunctionParam = FunctionParam.CAP.ToString(), BottomDescription = "New" };
            var isMyTurn = true;
            var routes = this._capexRouteRepository.Get(x => !x.IsDeleted && x.Approver.User.EmployeeId == employeeId && !x.Capex.IsDeleted && x.Capex.Status.Name == StatusEnum.Pending.ToString()).ToList();
            foreach (var capexRoute in routes)
            {
                isMyTurn = CheckTurnForCapex(capexRoute);
                if (isMyTurn)
                {
                    epoTile.Value++;
                    if (IsRouteToApproveOverdue(capexRoute)) epoTile.OverdueValue++;
                }
            }
            return epoTile;
        }

        private bool IsRouteToApproveOverdue<T>(T route) where T:BaseEntity
        {
            var routeDate = route.DateCreated;
            return routeDate.AddHours(OverdueHoursQty) < DateTime.Now;
        }

        public bool CheckTurnForCapex(CapexRoute capexRoute)
        {
            var route =
                _capexRouteRepository.Get(x => !x.IsDeleted && x.Capex.Id == capexRoute.Capex.Id).Include(_ => _.Approver)
                    .OrderBy(x => x.Number)
                    .First();
            return route.Approver.Id == capexRoute.Approver.Id;
        }

        private int GetEpoTileProgressbarValue(int value)
        {
            var result = 100 - value;
            return result;
        }

        private EpoTile GetInvoiceForApproveTile(int employeeId)
        {
            var epoTile = new EpoTile() { Header = "Invoices to Approve", TileColor = "red-bg", TileIcon = "fa fa-credit-card-alt", SortNumber = 4, TypeKey = 4,FunctionParam = FunctionParam.IAP.ToString(), BottomDescription = "New"};
            var doclist = _voucherDocumentRepository.Get(x => !x.IsAuthorised && x.Authoriser.EmployeeId == employeeId
                && (x.DocumentType.Name == DocumentTypeEnum.Invoice.ToString() || x.DocumentType.Name == DocumentTypeEnum.GRNI_Invoice.ToString().Replace("_", " "))
                && x.Voucher.Status.Name == StatusEnum.Pending.ToString() && !x.IsDeleted && !x.Voucher.IsDeleted).GroupBy(x => x.Voucher).ToList();
            epoTile.Value = doclist.Count;
            epoTile.BottomValue = doclist.Count(x => x.Key.LastModifiedDate.ToShortDateString() == DateAndTime.DateString);
            epoTile.OverdueValue = doclist.Count(x => x.Key.LastModifiedDate.AddHours(OverdueHoursQty) < DateTime.Now);
            //epoTile.ProgressValue = GetEpoTileProgressbarValue(epoTile.Value);
            return epoTile;
        }

        private EpoTile GetYourOrdersAwaitingApproveTile(int employeeId)
        {
            var epoTile = new EpoTile() { Header = "My Orders", TileColor = "white-bg", TileIcon = "fa fa-shopping-basket", SortNumber = 3, TypeKey = 3,FunctionParam = FunctionParam.OMY.ToString(), BottomDescription = "Awaiting approve"};
            epoTile.Value = _orderRepository.Get(x =>!x.IsDeleted && x.Status.Name == StatusEnum.Pending.ToString() && x.Author.EmployeeId == employeeId).Count();
            //Not using anymore
            //epoTile.BottomValue =list.Count(x => x.LastModifiedDate.ToShortDateString() == DateAndTime.Now.ToShortDateString());
            //epoTile.ProgressValue = GetEpoTileProgressbarValue(epoTile.Value);
            return epoTile;
        }

        private EpoTile GetOrdersToMatchTile(int employeeId)
        {
            var epoTile = new EpoTile() { Header = "Orders To Be Matched", TileColor = "yellow-bg", TileIcon = "fa fa-shopping-bag", TypeKey = 2, SortNumber = 2, FunctionParam = FunctionParam.OMA.ToString(), BottomDescription = "New" };
            var myReceiptGroups =_groupMemberRepository.Get(x => x.User.UserInfo.EmployeeId == employeeId && !x.IsDeleted)
                   .Select(x => x.Group)
                   .ToList();
            var mRGIds = myReceiptGroups.Select(x => x.Id).ToList();
            var orderList = _orderRepository.Get(x =>mRGIds.Contains(x.ReceiptGroup.Id)
                        && !x.IsDeleted
                        && x.Status.Name != StatusEnum.Draft.ToString()
                        && x.Status.Name != StatusEnum.Pending.ToString()
                        && x.Status.Name != StatusEnum.Closed.ToString()
                        && x.Status.Name != StatusEnum.Cancelled.ToString()
                        && x.Status.Name != StatusEnum.Receipt_in_Full.ToString().Replace("_", " ")).ToList();
            var ordersCnt = 0;
            var newOrderCnt = 0;
            if (myReceiptGroups.Any())
            {
                foreach (var list in myReceiptGroups.Select(myReceiptGroup => orderList.Where(x =>x.ReceiptGroup.Id == myReceiptGroup.Id).ToList()))
                {
                    ordersCnt += list.Count;
                    newOrderCnt +=list.Count(x => x.LastModifiedDate.ToShortDateString() == DateAndTime.Now.ToShortDateString());
                }
            }
            epoTile.BottomValue = newOrderCnt;
            epoTile.Value = ordersCnt;
            //epoTile.ProgressValue = GetEpoTileProgressbarValue(epoTile.Value);
            return epoTile;
        }

        private EpoTile GetOrdersForApproveTile(int employeeId)
        {
            var epoTile = new EpoTile() { Header = "Orders to Approve", TileColor = "blue-bg", TileIcon = "fa fa-check-circle-o", SortNumber = 1, TypeKey = 1, FunctionParam = FunctionParam.OAP.ToString(), BottomDescription = "New" };
            var isMyTurn = true;
            var routes =_routeRepository.Get(x => x.Approver.User.EmployeeId == employeeId && !x.IsDeleted && !x.Approver.IsDeleted && !x.Order.IsDeleted && x.Order.Status.Name == StatusEnum.Pending.ToString())
                    .ToList();
            var ordersCnt = 0;
            var newOrderCnt = 0;
            var expiredCnt = 0;
            foreach (var route in routes)
            {
                if (route.Number > 1)
                {
                    isMyTurn = CheckTurn(route);
                }
                if (isMyTurn)
                {
                    ordersCnt++;
                    if (IsOrderToApproveOverdue(route)) expiredCnt++;
                    if (IsRouteToApproveOverdue(route)) newOrderCnt++;
                }
                isMyTurn = true;
            }
            epoTile.Value = ordersCnt;
            epoTile.BottomValue = newOrderCnt;
            epoTile.OverdueValue = expiredCnt;
            //epoTile.ProgressValue = GetEpoTileProgressbarValue(epoTile.Value);
            return epoTile;
        }

        private static bool IsOrderToApproveOverdue(Route route)
        {
            var routeDate = route.DateCreated;
            return routeDate.AddHours(OverdueHoursQty) <= DateTime.Now;
        }

        private bool CheckTurn(Route route)
        {
            var list = this._routeRepository.Get(x => x.Order.Id == route.Order.Id && !x.IsDeleted && x.Approver.User.EmployeeId != this.curEmpId).ToList();
            var res = !(list.Count > 0);
            return res;
        }

        public CasefillDashboardViewModel GetCasefillDashboardViewModel(DateTime date)
        {
            var model = new CasefillDashboardViewModel();

            model.CasefillSales = GetCasefillSales(date.AddDays(-1));//get Yesterday data
            model.CasefillBudget = new CasefillBudgetTile();

            return model;
        }

        public List<CalendarEventViewModel> GetCalendarEventsViewModelFullList()
        {
            var modelList = new List<CalendarEventViewModel>();
            var list = _dCalendarEventRepository.Get().ToList();
            foreach (var ce in list)
            {
                var item = new CalendarEventViewModel()
                               {
                                   Id = ce.Id,
                                   Title = ce.Title,
                                   Description = ce.Description,
                                   IconColor = ce?.EventType?.IconColor,
                                   Icon = ce?.EventType?.Icon,
                                   IsOneDayEvent = ce.IsOneDayEvent,
                                   Start = ce?.Start.ToString(),
                                   End = ce?.End.ToString(),
                                   IsDeleted = ce.IsDeleted
                               };
                modelList.Add(item);
            }
            return modelList;
        }

        public CalendarEventCRUDViewModel GetCalendarEventCRUDViewModel()
        {
            var model = new CalendarEventCRUDViewModel();
            model.EventTypes = new List<CalendarEventTypeViewModel>();
            model.EventTypes =
                _dCalendarEventTypeRepository.Get()
                    .Select(x => new CalendarEventTypeViewModel() { Id = x.Id, FullName = x.Name })
                    .ToList();
            return model;
        }

        public void SaveDashboardCalendarEvent(CalendarEventCRUDViewModel model)
        {
            var cEvent = _dCalendarEventRepository.Find(model.Id);
            if (cEvent != null)
            {
                cEvent.Title = model.Title;
                cEvent.Description = model.Description;
                cEvent.EventType = _dCalendarEventTypeRepository.Find(model.SelectedType);
                cEvent.IsOneDayEvent = model.IsOneDayEvent;
                cEvent.Start = Convert.ToDateTime(model.Start);
                cEvent.End = !string.IsNullOrEmpty(model.End)? Convert.ToDateTime(model.End):DateTime.Now;
            }
            else
            {
                cEvent = new DCalendarEvent()
                             {
                                 Title = model.Title,
                                 Description = model.Description,
                                 IsOneDayEvent = model.IsOneDayEvent,
                                 End = !string.IsNullOrEmpty(model.End) ? Convert.ToDateTime(model.End) : DateTime.Now,
                                 Start = Convert.ToDateTime(model.Start),
                                 EventType = _dCalendarEventTypeRepository.Find(model.SelectedType)
                             };
                _dCalendarEventRepository.Add(cEvent);
            }
            _dataContext.SaveChanges();
        }

        public List<DCalendarEventType> GetCalendarEventTypeList()
        {
            return _dCalendarEventTypeRepository.Get().ToList();
        }

        public void SaveDashboardCalendarEventType(DCalendarEventType model)
        {
            var cEventType = _dCalendarEventTypeRepository.Find(model.Id);
            if (cEventType != null)
            {
                cEventType.Icon = model.Icon;
                cEventType.IconColor = model.IconColor;
                cEventType.Name = model.Name;
            }
            else
            {
                cEventType = new DCalendarEventType()
                                 {
                                     Icon = model.Icon,
                                     IconColor = model.IconColor,
                                     Name = model.Name
                                 };
                _dCalendarEventTypeRepository.Add(cEventType);
            }
            _dataContext.SaveChanges();
        }

        public void DeleteOrActivateCalendarEvent(int id, bool toDelete = true)
        {
            try
            {
                var model = this._dCalendarEventRepository.Find(id);
                model.IsDeleted = toDelete;
                this._dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.Error(e, "DeleteOrActivateCalendarEvent(id:{id})",id);
            }
        }

       
        public DCalendarEventType GetCalendarEventType(int id)
        {
            return _dCalendarEventTypeRepository.Find(id);
        }

        public CalendarEventCRUDViewModel GetCalendarEventCRUDViewModel(int id)
        {
            var cEvent = _dCalendarEventRepository.Get(x => x.Id == id).FirstOrDefault();

            if (cEvent != null)
            {
                var model = new CalendarEventCRUDViewModel()
                                {
                                    Id = cEvent.Id,
                                    Title = cEvent.Title,
                                    Description = cEvent.Description,
                                    Start = cEvent.Start.ToShortDateString(),
                                    End = cEvent.End.ToShortDateString(),
                                    IsOneDayEvent = cEvent.IsOneDayEvent,
                                    EventTypes = new List<CalendarEventTypeViewModel>(),
                                    SelectedType = cEvent.EventType?.Id ?? 0
                                };
                model.EventTypes =
                    _dCalendarEventTypeRepository.Get(x => !x.IsDeleted)
                        .Select(x => new CalendarEventTypeViewModel() { Id = x.Id, FullName = x.Name })
                        .ToList();
                return model;
            }
            return new CalendarEventCRUDViewModel();
        }

        private List<TileItem> GetCasefillSales(DateTime date)
        {
            var result = new List<TileItem>();
            try
            {
                result =_qadLive.Fact_SalesRC.Where(x => x.InvoiceDate == date)
                    .GroupBy(x => x.Site)
                    .Select(
                        y =>
                        new TileItem
                            {
                                SiteName = y.FirstOrDefault().Site,
                                ProgressValue = (y.Sum(c => c.InvoiceCartonsQty) / y.Sum(c => c.OrderCartonsQty)) * 100,
                            })
                    .ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Data.GetCasefillSales(date:{date})",date);
            }
            foreach (var casefillSale in result)
            {
                if (casefillSale.ProgressValue == 100) casefillSale.Color = "success";
                else if (casefillSale.ProgressValue <= 98) casefillSale.Color = "danger";
                else casefillSale.Color = "info";
            }
            return result.OrderBy(x=>x.SiteName).ToList();
        }

        /// <summary>
        /// Get full list of Dashboard News
        /// </summary>
        /// <returns></returns>
        public List<DNews> GetDashboardNews()
        {
            return _dNewsRepository.Get().ToList();
        }

        public void SaveDashboardNews(NewsCRUDViewModel model)
        {
            var news = _dNewsRepository.Get(x => x.Id == model.Id).FirstOrDefault();
            if (news != null)
            {
                news.IsPublished = model.IsPublished;
                news.IsDeleted = model.IsDeleted;
                news.Text = model.Text;
                news.Title = model.Title;
                _dNewsRepository.Update(news);
            }
            else
            {
                news = new DNews()
                           {
                               IsDeleted = model.IsDeleted,
                               IsPublished = model.IsPublished,
                               Text = model.Text,
                               Title = model.Title
                           };
                _dNewsRepository.Add(news);
            }
            _dataContext.SaveChanges();
        }

        public void DeleteOrActivateNews(int id, bool toDelete = true)
        {
            try
            {
                var model = _dNewsRepository.Find(id);
                model.IsDeleted = toDelete;
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.Error(e, "DeleteOrActivateNews(newsId:{newsId})", id);
            }
        }

        public NewsCRUDViewModel GetNews(int id)
        {
            var news = _dNewsRepository.Find(id);
            if (news != null)
            {
                var model = new NewsCRUDViewModel()
                                {
                                    Id = news.Id,
                                    Title = news.Title,
                                    IsDeleted = news.IsDeleted,
                                    IsPublished = news.IsPublished,
                                    Text = news.Text
                                };
                return model;
            }
            return new NewsCRUDViewModel();
        }

        public void PublishOrUnpublishNews(int id, bool toPublish = true)
        {
            var model = _dNewsRepository.Find(id);
            model.IsPublished = toPublish;
            _dNewsRepository.Update(model);
            _dataContext.SaveChanges();
        }

        public List<DTile> GetDashboardTiles(bool fullList)
        {
            var list = _dTileRepository.Get().ToList();
            if (!fullList) list = list.Where(x => !x.IsDeleted).ToList();
            return list;
        }

        public void SaveDTile(DTileCRUDViewModel model)
        {
            var dtile = _dTileRepository.Find(model.Id);
            if (dtile != null)
            {
                dtile.TileStyle = model.SelectedStyle;
                dtile.TileSubType = model.SelectedSubType;
                dtile.TileType = model.SelectedType;
                dtile.Department = model.SelectedDepartment;
                dtile.Site = model.SelectedSite;
                dtile.Name = model.Name;
                dtile.Description = model.Description;
                _dTileRepository.Update(dtile);
            }
            else
            {
                dtile = new DTile()
                {
                    TileStyle = model.SelectedStyle,
                    TileType = model.SelectedType,
                    TileSubType = model.SelectedSubType,
                    Department = model.SelectedDepartment,
                    Site = model.SelectedSite,
                    Description = model.Description,
                    Name = model.Name,
                    IsDeleted = model.IsDeleted,
                };
                _dTileRepository.Add(dtile);
            }
            _dataContext.SaveChanges();
        }

        public DTileCRUDViewModel GetDashboardTileCRUDViewModel(int id)
        {
            var dtile = _dTileRepository.Find(id);
            var model = new DTileCRUDViewModel()
            {
                Id = dtile.Id,
                Description = dtile.Description,
                Name = dtile.Name,
                IsDeleted = dtile.IsDeleted,
                SelectedSite = dtile.Site,
                SelectedStyle = dtile.TileStyle,
                SelectedType = dtile.TileType,
                SelectedSubType = dtile.TileSubType,
                SelectedDepartment = dtile.Department,
                Sites = Enum.GetValues(typeof(Site)).Cast<Site>().ToList(),
                Departments =Enum.GetValues(typeof(Department)).Cast<Department>().ToList(),
                TileStyles =Enum.GetValues(typeof(TileStyle)).Cast<TileStyle>().ToList(),
                TileTypes = Enum.GetValues(typeof(TileType)).Cast<TileType>().ToList(),
                SubTypes = Enum.GetValues(typeof(TileSubType)).Cast<TileSubType>().ToList()
            };
            return model;
        }

        public void DeleteOrActivateDTile(int id, bool toDelete = true)
        {
            var model = _dTileRepository.Find(id);
            model.IsDeleted = toDelete;
            _dTileRepository.Update(model);
            _dataContext.SaveChanges();
        }

        

        public List<DUserGroup> GetDashboardUserGroups()
        {
            return _dUserGroupRepository.Get().ToList();
        }

        public void SaveDUserGroup(DUserGroupCRUDViewModel model)
        {
            var uGroup = _dUserGroupRepository.Find(model.Id);
            if (uGroup != null)
            {
                uGroup.Description = model.Description;
                uGroup.Name = model.Name;
                uGroup.RequiredTiles = new PersistableIntCollection();
                uGroup.RequiredTiles.AddRange(model.SelectedRequiredTiles);
                if (model.SelectedDefaultTiles!=null && model.SelectedDefaultTiles.Any())
                {
                    uGroup.DefaultTiles = new PersistableIntCollection();
                    uGroup.DefaultTiles.AddRange(model.SelectedDefaultTiles);
                }
                _dUserGroupRepository.Update(uGroup);
            }
            else
            {
                uGroup = new DUserGroup()
                {
                    Description = model.Description,
                    Name = model.Name,
                    RequiredTiles = new PersistableIntCollection(),
                    DefaultTiles = new PersistableIntCollection()
                };
                if(model.SelectedRequiredTiles!=null) uGroup.RequiredTiles.AddRange(model.SelectedRequiredTiles);
                if (model.SelectedDefaultTiles != null) uGroup.DefaultTiles.AddRange(model.SelectedDefaultTiles);

                _dUserGroupRepository.Add(uGroup);
            }
            _dataContext.SaveChanges();
        }

        public DUserGroupCRUDViewModel GetDashboardUserGroupCRUDViewModel(int id)
        {
            var uGroup = _dUserGroupRepository.Find(id);
            var model = new DUserGroupCRUDViewModel()
            {
                Description = uGroup.Description,
                Name = uGroup.Name,
                Id = uGroup.Id,
                DTiles = GetDashboardTiles(false),
                SelectedRequiredTiles = new List<int>()
            };
            model.SelectedRequiredTiles = uGroup.RequiredTiles.ToList();
            model.SelectedDefaultTiles = uGroup.DefaultTiles.ToList();
            return model;
        }

        public void DeleteOrActivateDUserGroup(int id, bool toDelete = true)
        {
            var model = _dUserGroupRepository.Find(id);
            model.IsDeleted = toDelete;
            _dUserGroupRepository.Update(model);
            _dataContext.SaveChanges();
        }
        /// <summary>
        /// Getting User Settings for futher Customisation
        /// </summary>
        /// <param name="loadRequiredTiles"></param>
        /// <param name="loadAvailableTiles"></param>
        /// <returns></returns>
        public UserDashboardSettingsViewModel GetUserDashboardSettingsViewModelForCurrentUser(bool loadRequiredTiles, bool loadAvailableTiles=false)
        {
            var userSettings =
                _userRepository.Get(x => x.EmployeeId == curEmpId)
                    .Include(x => x.UserDashboardSettings)
                    .Include(x => x.UserDashboardSettings.DUserGroup)
                    .Select(x => x.UserDashboardSettings)
                    .FirstOrDefault();
            var model = new UserDashboardSettingsViewModel()
                            {
                                DashboardUserGroup = userSettings?.DUserGroup.Name,
                                RequiredTileList = new List<DTileShowroomViewModel>()
                            };
            var requiredList = new List<DTileShowroomViewModel>();
            var myList = new List<DTileShowroomViewModel>();
            var availableList = new List<DTileShowroomViewModel>();
            myList = LoadMyTiles(userSettings);
            var reqTileIds = userSettings?.DUserGroup?.RequiredTiles.ToList();
            if (reqTileIds.Any())
            {
                var tileList = _dTileRepository.Get(x => reqTileIds.Contains(x.Id)).ToList();
                foreach (var tile in tileList)
                {
                    var tileVM = new DTileShowroomViewModel()
                                     {
                                         Id = tile.Id,
                                         Name = tile.Name,
                                         Description = tile.Description,
                                         IsSelected = true,
                                         Style = tile.TileStyle,
                                         Type = tile.TileType,
                                         SubType = tile.TileSubType,
                                         IsDisabled = true,
                                         MockData = new TileMockData()
                                     };
                    //Fetch Mock Data for Tiles
                    tileVM.MockData = _mock.FetchDataForShowroomTile(tileVM); 

                    requiredList.Add(tileVM);
                }
            }
            model.RequiredTileList = requiredList;
            if (loadAvailableTiles)
            {
                availableList = LoadAvailableTiles(
                    myList.Select(x => x.Id).ToList(),
                    model.RequiredTileList.Select(x => x.Id).ToList());
            }

            model.AvailableTileList = availableList;
            model.MyTileList = myList;

            return model;
        }

        public List<DTileShowroomViewModel> LoadAvailableTiles(List<int> myTileIds, List<int> reqTileIds)
        {
            var list = new List<DTileShowroomViewModel>();
            var tileList = _dTileRepository.Get(x => !myTileIds.Contains(x.Id) && !reqTileIds.Contains(x.Id) && !x.IsDeleted).ToList();
            foreach (var tile in tileList)
            {
                var tileVM = new DTileShowroomViewModel()
                                 {
                                     Id = tile.Id,
                                     Name = tile.Name,
                                     Description = tile.Description,
                                     IsSelected = false,
                                     Style = tile.TileStyle,
                                     SubType = tile.TileSubType,
                                     Type = tile.TileType,
                                     IsDisabled = false,
                                     MockData = new TileMockData()
                                 };
                tileVM.MockData = _mock.FetchDataForShowroomTile(tileVM);
                list.Add(tileVM);
            }
            return list;
        }

        public List<DTileShowroomViewModel> LoadMyTiles(UserDashboardSettings userSettings)
        {
            var myTileIds = userSettings.MyTiles.ToList();
            var myTileList = new List<DTileShowroomViewModel>();
            if (myTileIds.Any())
            {
                var tileList = _dTileRepository.Get(x => myTileIds.Contains(x.Id)).ToList();
                foreach (var tile in tileList)
                {
                    var tileVM = new DTileShowroomViewModel()
                    {
                        Id = tile.Id,
                        Name = tile.Name,
                        Description = tile.Description,
                        IsSelected = true,
                        Style = tile.TileStyle,
                        SubType = tile.TileSubType,
                        Type = tile.TileType,
                        IsDisabled = false,
                        MockData = new TileMockData()
                    };
                    tileVM.MockData = _mock.FetchDataForShowroomTile(tileVM);
                    myTileList.Add(tileVM);
                }
                return myTileList;
            }
            return new List<DTileShowroomViewModel>();
        }

        public List<DTileShowroomViewModel> LoadMyTiles()
        {
            var userSettings =
                _userRepository.Get(x => x.EmployeeId == curEmpId)
                    .Include(x => x.UserDashboardSettings)
                    .Select(x => x.UserDashboardSettings)
                    .FirstOrDefault();
            return LoadMyTiles(userSettings);
        }

        public string SaveMyTiles(List<int> tileIds)
        {
           // var list = _dTileRepository.Get(x => tileIds.Contains(x.Id)).ToList();
            try
            {
                var userSettings =
                    _userRepository.Get(x => x.EmployeeId == curEmpId)
                        .Include(x => x.UserDashboardSettings).Include(x => x.UserDashboardSettings.DUserGroup)
                        .Select(x => x.UserDashboardSettings)
                        .FirstOrDefault();
                userSettings.MyTiles =new PersistableIntCollection();
                userSettings.MyTiles.AddRange(tileIds);
                _userDashboardSettingsRepository.Update(userSettings);
                _dataContext.SaveChanges();
                return "Saved successfully!";
            }
            catch (Exception e)
            {
                _logger.Error(e,"SaveMyTiles(user:{curEmpId})", curEmpId);
                return "Error on saving!";
            }
        }

        public bool HasUserAnyPresetTilesForDashboard()
        {
            if (curEmpId == 0) return false;
            var userSettings =
                _userRepository.Get(x => x.EmployeeId == curEmpId && !x.UserInfo.IsDeleted)
                    .Select(x => x.UserDashboardSettings)
                    .Include(x => x.DUserGroup)
                    .First();
            if (userSettings != null)
            {
                if (userSettings.MyTiles.Any() || userSettings.DUserGroup.DefaultTiles.Any() || userSettings.DUserGroup.RequiredTiles.Any()) return true;
            }
            return false;
        }

        public EpoTile InitEpoTileData(DTile tile)
        {
            var eTile = GetEPOTilebySubType(tile.TileSubType, curEmpId);
            return eTile;
        }

        public TnATile InitTnaTileData(DTile tile)
        {
            var tTile = GetTnaTilebySubType(tile.TileSubType, curEmpId);
            return tTile;
        }

        public CasefillDashboardViewModel InitCasefillTileData(DTile tile)
        {
            var cTile = GetCSLTilebySubType(tile.TileSubType);
            return cTile;
        }

        public SmsTileViewModel InitSmsTileData(DTile tile)
        {
            var sTile = GetSmsTilebySubType(tile.TileSubType);
            return sTile;
        }

        private SmsTileViewModel GetSmsTilebySubType(TileSubType tileSubType)
        {
            try
            {
                switch (tileSubType)
                {
                    case TileSubType.SAY:
                        return GetSafetyActionsForEmployee(DateTime.Today);
                    case TileSubType.STSAR:
                        return GetSafetyActionsRegisterByMonth(DateTime.Today);
                    case TileSubType.STIN:
                        return GetSafetyIncidentsTable(DateTime.Today);
                    case TileSubType.STSWAT:
                        return GetSafetyWalkAndTalksTable(DateTime.Today);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(tileSubType), tileSubType, null);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e,"Data.Dashboard.GetSmsTilebySubType(type:{tileSubType})", tileSubType);
            }
            return new SmsTileViewModel();
        }

        private SmsTileViewModel GetSafetyWalkAndTalksTable(DateTime today)
        {
            var model = new SmsTileViewModel()
            {
                Header = "Safety Walk & Talks",
                TileColor = "grayblue-bg",
                SmsTileItems = new List<TileItem>()
            };

            try
            {
                var dailyData = _safetyManagementSystem.vSafetyWalkTalksByDays.Where(x => x.Day == today).ToList();
                var monthlyData =_safetyManagementSystem.vSafetyWalkTalksByMonths.Where(x => x.Month == today.Month && x.Year == today.Year).ToList();
                var weekNum = WeekOfYearISO8601(today);
                var weeklyData =_safetyManagementSystem.vSafetyWalkTalksByWeeks.Where(x => x.Year == today.Year && x.Week == weekNum).ToList();
                foreach (var monthData in monthlyData)
                {
                    var item = new TileItem() { SiteName = monthData.Site, ValueArray = new List<int>(), ValueNameArray = new List<string>() };
                    var dayData = dailyData.Where(x => x.Site == monthData.Site).Select(x => x.Count).FirstOrDefault();
                    item.ValueArray.Add(dayData ?? 0);
                    item.ValueNameArray.Add("Day");
                    var weekData = weeklyData.Where(x => x.Site == monthData.Site).Select(x => x.Count).FirstOrDefault();
                    item.ValueArray.Add(weekData ?? 0);
                    item.ValueNameArray.Add("Week");
                    //var monthData = monthlyData.Where(x => x.Site == dayData.Site).Select(x => x.Count).FirstOrDefault();
                    item.ValueArray.Add(monthData.Count ?? 0);
                    item.ValueNameArray.Add("Month");
                    model.SmsTileItems.Add(item);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "GetSafetyWalkAndTalksTable(date:{date})",today);
            }
            return model;
        }

        private SmsTileViewModel GetSafetyIncidentsTable(DateTime today)
        {
            var model = new SmsTileViewModel()
                            {
                                Header = "Safety Inсidents",
                                TileColor = "grayblue-bg",
                                SmsTileItems = new List<TileItem>()
                            };
            try
            {
                var dailyData = _safetyManagementSystem.vIncidentsByDays.Where(x => x.Day == today).ToList();
                var monthlyData =_safetyManagementSystem.vIncidentsByMonths.Where(x => x.Month == today.Month && x.Year == today.Year).ToList();
                var weekNum = WeekOfYearISO8601(today);
                var weeklyData =
                    _safetyManagementSystem.vIncidentsByWeeks.Where(x => x.Year == today.Year && x.Week == weekNum).ToList();
                foreach (var monthData in monthlyData)
                {
                    var item = new TileItem() { SiteName = monthData.Site, ValueArray = new List<int>(), ValueNameArray = new List<string>()};
                    var dayData = dailyData.Where(x => x.Site == monthData.Site).Select(x => x.Count).FirstOrDefault();
                    item.ValueArray.Add(dayData ?? 0);
                    item.ValueNameArray.Add("Day");
                    var weekData = weeklyData.Where(x => x.Site == monthData.Site).Select(x=>x.Count).FirstOrDefault();
                    item.ValueArray.Add(weekData?? 0);
                    item.ValueNameArray.Add("Week");
                    //var monthData = monthlyData.Where(x => x.Site == dayData.Site).Select(x => x.Count).FirstOrDefault();
                    item.ValueArray.Add(monthData.Count ?? 0);
                    item.ValueNameArray.Add("Month");
                    model.SmsTileItems.Add(item);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "GetSafetyIncidentsTable(date:{date})",today);
            }
            return model;
        }

        private SmsTileViewModel GetSafetyActionsRegisterByMonth(DateTime today)
        {
            var model = new SmsTileViewModel() { SmsTileItems = new List<TileItem>(), TileColor = "grayblue-bg" };

            try
            {
                var data =
                    _safetyManagementSystem.vSafetyActionsByMonthBySites.Where(
                        x => x.Month == today.Month && x.Year == today.Year)
                        .Select(x => new { x.Site, x.Count.Value })
                        .OrderBy(x => x.Site)
                        .ToList();
                foreach (var row in data)
                {
                    var item = new TileItem() { SiteName = row.Site, ProgressValue = row.Value };
                    model.SmsTileItems.Add(item);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "GetSafetyActionsRegisterByMonth(date:{date})",today);
            }

            return model;
        }

        private SmsTileViewModel GetSafetyActionsForEmployee(DateTime today)
        {
            var model = new SmsTileViewModel() { TileIcon = "fa fa-universal-access", Header = "Safety Actions", TileColor = "grayblue-bg",SortNumber = 1, TypeKey = 1};
            try
            {
                var sa = _safetyManagementSystem.vSafetyActionsByMonthByUsers
                        .Where(x => x.Month == today.Month && x.Year == today.Year && x.CurrentOwnerEmpNo == curEmpId.ToString()).Select(x=>x.Count.Value).First();
                model.Value = sa;

            }
            catch (Exception e)
            {
                _logger.Error(e,"Data.DashboardGetSafetyActionsForEmployee(date:{date})",today);
            }
            return model;
        }

        private CasefillDashboardViewModel GetCSLTilebySubType(TileSubType tileSubType)
        {
            try
            {
                switch (tileSubType)
                {
                    case TileSubType.YCSL:
                        return GetCasefillDashboardViewModel(DateTime.Today);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e,"Data.Dashboard.GetCSLTilebySubType(subType:{subType})",tileSubType );
            }
            return new CasefillDashboardViewModel();
        }

        public static int WeekOfYearISO8601(DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date.AddDays(4 - (day == 0 ? 7 : day)), CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }
    }
}
