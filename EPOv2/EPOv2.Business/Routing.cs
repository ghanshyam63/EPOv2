namespace EPOv2.Business
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using DomainModel.DataContext;
    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.Repositories.Interfaces;
    using EPOv2.ViewModels;


    public partial class Routing : IDisposable, IRouting
    {
        #region Global Variable
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public IDataContext Db { get; set; }

        private readonly OldPurchaseOrderContext _oldDb = new OldPurchaseOrderContext();

        private readonly IAd _ad;

        private string _curUser = HttpContext.Current.User.Identity.Name.Replace("ONEHARVEST\\", "");

        private readonly IData _data;

        private readonly IMain _main;

        private readonly IOutput _output;

        public bool IsRevision { get; set; }

        public bool IsCapex = false;

        private readonly Dictionary<string, string> _routingErrorsDictionary = new Dictionary<string, string>()
                                                                                   {
                                                                                        { "Author",""}
                                                                                   };

        private CostCentre _orderCostCentre;
        public  ControllerContext ControllerContext { get; set; }
        public string RoutingResult { get; set; }
        private class FakeController : Controller{}
        

        private static readonly PrincipalContext PrincipalContext= new PrincipalContext(ContextType.Domain,"oneharvest.com.au");

        private UserPrincipal _userPrincipal = new UserPrincipal(PrincipalContext);

        public int CurEmpId { get; set; }

        public List<ApproverViewModel> ApproverListVM { get; set; }
        #endregion

        #region Repository

        private readonly IOrderRepository _orderRepository;
        private readonly ICostCentreRepository _costCentreRepository;

        private readonly IVoucherDocumentRepository _voucherDocumentRepository;

        private readonly IVoucherRepository _voucherRepository;

        private readonly IVoucherRouteRepository _voucherRouteRepository;

        private readonly IVoucherStatusRepository _voucherStatusRepository;

        private readonly IOrderItemRepository _orderItemRepository;

        private readonly IStatusRepository _statusRepository;

        private readonly IOrderItemKitRepository _orderItemKitRepository;

        private readonly ILevelRepository _levelRepository;

        private readonly IVRockyEmployeesRepository _rockyEmployeesRepository;

        private readonly IApproverRepository _approverRepository;

        private readonly IRouteRepository _routeRepository;

        private readonly IUserRepository _userRepository;

        private readonly ICapexRouteRepository _capexRouteRepository;

        private readonly ICapexRepository _capexRepository;

        private readonly ICapexApproverRepository _capexApproverRepository;

        private readonly IAccountRepository _accountRepository;

        private ICurrencyRepository _currencyRepository;

#endregion
      
        public Routing(IDataContext db, ControllerContext controllerContext, IAd ad, IData data,IMain main, IOutput output, IOrderRepository orderRepository, 
            IAccountRepository accountRepository, ICurrencyRepository currencyRepository, IStatusRepository statusRepository, IOrderItemKitRepository orderItemKitRepository, 
            ILevelRepository levelRepository, IVRockyEmployeesRepository rockyEmployeesRepository, IApproverRepository approverRepository, 
            IRouteRepository routeRepository, IUserRepository userRepository, ICapexRouteRepository capexRouteRepository, ICapexRepository capexRepository, 
            ICapexApproverRepository capexApproverRepository, ICostCentreRepository costCentreRepository, IVoucherDocumentRepository voucherDocumentRepository, 
            IVoucherRepository voucherRepository, IVoucherRouteRepository voucherRouteRepository, IVoucherStatusRepository voucherStatusRepository, 
            IOrderItemRepository orderItemRepository)
        {
            Db = db;
            _data = data;//new Data(_db);
            _main = main;
            _output = output;
            _orderRepository = orderRepository;
            _accountRepository = accountRepository;
            _currencyRepository = currencyRepository;
            _statusRepository = statusRepository;
            _orderItemKitRepository = orderItemKitRepository;
            _levelRepository = levelRepository;
            _rockyEmployeesRepository = rockyEmployeesRepository;
            _approverRepository = approverRepository;
            _routeRepository = routeRepository;
            _userRepository = userRepository;
            _capexRouteRepository = capexRouteRepository;
            _capexRepository = capexRepository;
            _capexApproverRepository = capexApproverRepository;
            _costCentreRepository = costCentreRepository;
            _voucherDocumentRepository = voucherDocumentRepository;
            _voucherRepository = voucherRepository;
            _voucherRouteRepository = voucherRouteRepository;
            _voucherStatusRepository = voucherStatusRepository;
            _orderItemRepository = orderItemRepository;
            ControllerContext = controllerContext;
            _ad = ad;
            CurEmpId = 7459;// Convert.ToInt32(_ad.GetADUserEmpIDbyUserName(_curUser));
           
        }


        #region Main Methods

        /// <summary>
        /// prepare for Add/Update routes
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="employee">Need just for TYPE=0</param>
        /// <param name="level"></param>
        /// <param name="type">2 - create routing for employees who staing ABOVE this Manager; 1 - create routing for employees who staing UNDER this manager; 0 - ??? </param>
        /// <param name="costCentre"></param>
        /// <summary>
        /// Add/Update routing - main procedure
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="costCentresLst"></param>
        /// <param name="limit"></param>
        /// <param name="approvalType"></param>
        /// <param name="approvalNumber"></param>
        /// <summary>
        /// Delele user from StagingEmployees and Delete all routes for this Employee
        /// </summary>
        /// <param name="stagingEmployee"></param>
        /// <returns></returns>
        /// <summary>
        /// Refresh All routes by Staging Account CostCentre.  PushRouting -> PushRouting -> MakeRouting
        /// </summary>
        /// <summary>
        /// Insert new record to tblStagingCostCentres.
        /// If StagingCostCentre exist than return existing odject
        /// </summary>
        /// <param name="costCenteCode"></param>
        /// <returns>ID of record, even it's already exist</returns>
        /// <summary>
        /// return Entity ID by using business logic of relations between Entity and Cost Centre
        /// </summary>
        /// <param name="costCentreCode"></param>
        /// <returns></returns>
        /// <summary>
        /// Get Cost Centre from original table
        /// </summary>
        /// <param name="active"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        /// <summary>http://localhost:3867/ManageRouting/ChangeOwner
        /// Get Cost Centre from StagingCostCentre
        /// </summary>
        /// <returns></returns>

        //public SelectList GetCostCentreList()
        //{
        //    var ccLst =
        //        new SelectList(
        //            this._oldDb.tblStagingCostCentresDEVs.Select(
        //                x => new {x.CostCentreCode, fullname = x.CostCentreCode + " - " + x.Description})
        //                .OrderBy(x => x.CostCentreCode), "CostCentreCode",
        //            "fullname", this._oldDb.tblStagingCostCentresDEVs);
        //    return ccLst;
        //}

        //public FakeRouting FakeRoutingInit()
        //{
        //    var entityList = this._oldDb.tblEntities.Where(x => x.Active).ToList();
        //    var costCentreList = this._oldDb.tblCostCentres.Where(x => x.Active).ToList();
        //    var accountList = this._oldDb.tblAccounts.Where(x => x.Active).ToList();
        //    var authoriserList = this._oldDb.tblStagingEmployeesDEVs.Where(x => x.Level > 0).OrderBy(x => x.Employee).Select(x => x.Employee).ToList();
        //    var fakeRouting = new FakeRouting()
        //                          {
        //                              Entities = entityList,
        //                              Accounts = accountList,
        //                              CostCentres = costCentreList,
        //                              Authorisers = authoriserList

        //                          };
        //    return fakeRouting;
        //}

        #endregion

        #region DummyData

        //public Order GetFirstScenarioOrder()
        //{
        //    var item = new OrderItem()
        //    {
        //        LineNumber = 1,
        //        RevisionQty = 0,
        //        Account = this._accountRepository.Find(290),
        //        SubAccount = this._accountRepository.Find(0),
        //        Currency = this._currencyRepository.Find(1),
        //        Description = "Test PO, 1st scenario",
        //        DueDate = Convert.ToDateTime("01/02/2015"),
        //        Qty = 2,
        //        UnitPrice = 500,
        //        IsGSTInclusive = true,
        //        IsTaxable = true,
        //        Total = 1000,
        //        TotalExTax = 900,
        //        TotalTax = 100,
        //        CreatedBy = "akogtev",
        //        DateCreated = DateTime.Now,
        //        LastModifiedDate = DateTime.Now,
        //        LastModifiedBy = "akogtev",
        //        Status =
        //            this._db.Statuses.FirstOrDefault(
        //                x => x.Name == Data.StatusEnum.Pending.ToString())
        //    };
        //    var list = new List<OrderItem>();
        //    list.Add(item);
        //    var order = new Order()
        //    {
        //        CreatedBy = "akogtev",
        //        DateCreated = DateTime.Now,
        //        LastModifiedDate = DateTime.Now,
        //        LastModifiedBy = "akogtev",
        //        CostCentre = this._db.CostCentres.Find(42),
        //        User = this._db.Users.FirstOrDefault(x => x.EmployeeId == 4994),
        //        RevisionQty = 0,
        //        TempOrderNumber = "e354a551-4502-46f4-b97e-7525782a43e2",
        //        OrderDate = DateTime.Now,
        //        Entity = this._db.Entities.Find(7),
        //        OrderItems = list,
        //        Status = this._db.Statuses.FirstOrDefault(
        //               x => x.Name == Data.StatusEnum.Pending.ToString()),
        //        Total = 1000,
        //        TotalExGST = 900,
        //        TotalGST = 100,
        //        TransmissionMethod = Data.TransmissionMethod.Email.ToString(),
        //        SupplierId = 16843,
        //        DeliveryAddress = this._db.DeliveryAddresses.Find(2),
        //        ReceiptGroup = this._db.Groups.Find(54),

        //    };
        //    return order;
        //}

        //public Order GetSecondScenarioOrder()
        //{
        //    var item = new OrderItem()
        //    {
        //        LineNumber = 1,
        //        RevisionQty = 0,
        //        Account = this._db.Accounts.Find(290),
        //        SubAccount = this._db.Accounts.Find(0),
        //        Currency = this._db.Currencies.Find(1),
        //        Description = "Test PO, 2st scenario",
        //        DueDate = Convert.ToDateTime("01/02/2015"),
        //        Qty = 1,
        //        UnitPrice = 400,
        //        IsGSTInclusive = true,
        //        IsTaxable = true,
        //        Total = 400,
        //        TotalExTax = 350,
        //        TotalTax = 50,
        //        CreatedBy = "akogtev",
        //        DateCreated = DateTime.Now,
        //        LastModifiedDate = DateTime.Now,
        //        LastModifiedBy = "akogtev",
        //        Status =
        //            this._db.Statuses.FirstOrDefault(
        //                x => x.Name == Data.StatusEnum.Pending.ToString())
        //    };
        //    var list = new List<OrderItem>();
        //    list.Add(item);
        //    var order = new Order()
        //    {
        //        CreatedBy = "akogtev",
        //        DateCreated = DateTime.Now,
        //        LastModifiedDate = DateTime.Now,
        //        LastModifiedBy = "akogtev",
        //        CostCentre = this._db.CostCentres.Find(42),
        //        User = this._db.Users.FirstOrDefault(x => x.EmployeeId == 4994),
        //        RevisionQty = 0,
        //        TempOrderNumber = "e354a551-4502-46f4-b97e-7525782a43e2",
        //        OrderDate = DateTime.Now,
        //        Entity = this._db.Entities.Find(7),
        //        OrderItems = list,
        //        Status = this._db.Statuses.FirstOrDefault(
        //               x => x.Name == Data.StatusEnum.Pending.ToString()),
        //        Total = 400,
        //        TotalExGST = 350,
        //        TotalGST = 50,
        //        TransmissionMethod = Data.TransmissionMethod.Email.ToString(),
        //        SupplierId = 16843,
        //        DeliveryAddress = this._db.DeliveryAddresses.Find(2),
        //        ReceiptGroup = this._db.Groups.Find(54),
        //        Comment = "Test PO, 2st scenario",
        //        InternalComment = "Test PO, 2st scenario",

        //    };
        //    return order;
        //}

        //public Order GetThirdScenarioOrder()
        //{
        //    var item = new OrderItem()
        //    {
        //        LineNumber = 1,
        //        RevisionQty = 0,
        //        Account = this._db.Accounts.Find(290),
        //        SubAccount = this._db.Accounts.Find(0),
        //        Currency = this._db.Currencies.Find(1),
        //        Description = "Test PO, 3rd scenario",
        //        DueDate = Convert.ToDateTime("01/02/2015"),
        //        Qty = 5,
        //        UnitPrice = 10000,
        //        IsGSTInclusive = true,
        //        IsTaxable = true,
        //        Total = 50000,
        //        TotalExTax = 49000,
        //        TotalTax = 1000,
        //        CreatedBy = "akogtev",
        //        DateCreated = DateTime.Now,
        //        LastModifiedDate = DateTime.Now,
        //        LastModifiedBy = "akogtev",
        //        Status =
        //            this._db.Statuses.FirstOrDefault(
        //                x => x.Name == Data.StatusEnum.Pending.ToString())
        //    };
        //    var list = new List<OrderItem>();
        //    list.Add(item);
        //    var order = new Order()
        //    {
        //        CreatedBy = "akogtev",
        //        DateCreated = DateTime.Now,
        //        LastModifiedDate = DateTime.Now,
        //        LastModifiedBy = "akogtev",
        //        CostCentre = this._db.CostCentres.Find(42),
        //        User = this._db.Users.FirstOrDefault(x => x.EmployeeId == 4994),
        //        RevisionQty = 0,
        //        TempOrderNumber = "e354a551-4502-46f4-b97e-7525782a43e2",
        //        OrderDate = DateTime.Now,
        //        Entity = this._db.Entities.Find(7),
        //        OrderItems = list,
        //        Status = this._db.Statuses.FirstOrDefault(
        //               x => x.Name == Data.StatusEnum.Pending.ToString()),
        //        Total = 50000,
        //        TotalExGST = 49000,
        //        TotalGST = 1000,
        //        TransmissionMethod = Data.TransmissionMethod.Email.ToString(),
        //        SupplierId = 16843,
        //        DeliveryAddress = this._db.DeliveryAddresses.Find(2),
        //        ReceiptGroup = this._db.Groups.Find(54),
        //        Comment = "Test PO, 3rd scenario",
        //        InternalComment = "Test PO, 3rd scenario",

        //    };
        //    return order;
        //}

        //public Order GetFourthScenarioOrder()
        //{
        //    var item = new OrderItem()
        //    {
        //        LineNumber = 1,
        //        RevisionQty = 0,
        //        Account = this._db.Accounts.Find(290),
        //        SubAccount = this._db.Accounts.Find(0),
        //        Currency = this._db.Currencies.Find(1),
        //        Description = "Test PO, 4th scenario",
        //        DueDate = Convert.ToDateTime("01/02/2015"),
        //        Qty = 2,
        //        UnitPrice = 4500,
        //        IsGSTInclusive = true,
        //        IsTaxable = true,
        //        Total = 9000,
        //        TotalExTax = 8000,
        //        TotalTax = 1000,
        //        CreatedBy = "jschulz",
        //        DateCreated = DateTime.Now,
        //        LastModifiedDate = DateTime.Now,
        //        LastModifiedBy = "jschulz",
        //        Status =
        //            this._db.Statuses.FirstOrDefault(
        //                x => x.Name == Data.StatusEnum.Pending.ToString())
        //    };
        //    var list = new List<OrderItem>();
        //    list.Add(item);
        //    var order = new Order()
        //    {
        //        CreatedBy = "jschulz",
        //        DateCreated = DateTime.Now,
        //        LastModifiedDate = DateTime.Now,
        //        LastModifiedBy = "jschulz",
        //        CostCentre = this._db.CostCentres.Find(42),
        //        User = this._db.Users.FirstOrDefault(x => x.EmployeeId == 3605),
        //        RevisionQty = 0,
        //        TempOrderNumber = "e354a551-4502-46f4-b97e-7525782a43e2",
        //        OrderDate = DateTime.Now,
        //        Entity = this._db.Entities.Find(7),
        //        OrderItems = list,
        //        Status = this._db.Statuses.FirstOrDefault(
        //               x => x.Name == Data.StatusEnum.Pending.ToString()),
        //        Total = 9000,
        //        TotalExGST = 8000,
        //        TotalGST = 1000,
        //        TransmissionMethod = Data.TransmissionMethod.Email.ToString(),
        //        SupplierId = 16843,
        //        DeliveryAddress = this._db.DeliveryAddresses.Find(2),
        //        ReceiptGroup = this._db.Groups.Find(54),
        //        Comment = "Test PO, 4th scenario",
        //        InternalComment = "Test PO, 4th scenario",

        //    };
        //    return order;
        //}

        //public Order GetFifthScenarioOrder()
        //{
        //    var item = new OrderItem()
        //    {
        //        LineNumber = 1,
        //        RevisionQty = 0,
        //        Account = this._db.Accounts.Find(290),
        //        SubAccount = this._db.Accounts.Find(0),
        //        Currency = this._db.Currencies.Find(1),
        //        Description = "Test PO, 5th scenario",
        //        DueDate = Convert.ToDateTime("01/02/2015"),
        //        Qty = 1,
        //        UnitPrice = 200,
        //        IsGSTInclusive = true,
        //        IsTaxable = true,
        //        Total = 200,
        //        TotalExTax = 180,
        //        TotalTax = 20,
        //        CreatedBy = "abryan",
        //        DateCreated = DateTime.Now,
        //        LastModifiedDate = DateTime.Now,
        //        LastModifiedBy = "abryan",
        //        Status =
        //            this._db.Statuses.FirstOrDefault(
        //                x => x.Name == Data.StatusEnum.Pending.ToString())
        //    };
        //    var list = new List<OrderItem>();
        //    list.Add(item);
        //    var order = new Order()
        //    {
        //        CreatedBy = "abryan",
        //        DateCreated = DateTime.Now,
        //        LastModifiedDate = DateTime.Now,
        //        LastModifiedBy = "abryan",
        //        CostCentre = this._db.CostCentres.Find(14),
        //        User = this._db.Users.FirstOrDefault(x => x.EmployeeId == 26),
        //        RevisionQty = 0,
        //        TempOrderNumber = "e354a551-4502-46f4-b97e-7525782a43e2",
        //        OrderDate = DateTime.Now,
        //        Entity = this._db.Entities.Find(7),
        //        OrderItems = list,
        //        Status = this._db.Statuses.FirstOrDefault(
        //               x => x.Name == Data.StatusEnum.Pending.ToString()),
        //        Total = 200,
        //        TotalExGST = 180,
        //        TotalGST = 20,
        //        TransmissionMethod = Data.TransmissionMethod.Email.ToString(),
        //        SupplierId = 16843,
        //        DeliveryAddress = this._db.DeliveryAddresses.Find(2),
        //        ReceiptGroup = this._db.Groups.Find(54),
        //        Comment = "Test PO, 5th scenario",
        //        InternalComment = "Test PO, 5th scenario",

        //    };
        //    return order;
        //}

        #endregion

        #region New Routing
        public bool Start(Order order)
        {
            var result = false;
            try
            {
             

                IsCapex = order.Capex_Id != 0;

                _orderCostCentre = IsCapex ? _data.GetCostCentreForCapex(order.Capex_Id) : order.CostCentre;

                var ownerEmpId = _orderCostCentre.Owner.UserInfo.EmployeeId;
                var authorEmpId = 7459;//order.Author.UserInfo.EmployeeId;
                var authorData = GetRockieLevelData(authorEmpId.ToString());
                if (authorData == null)
                {
                    RoutingResult = "";
                    return false; //no data for that user in RockFast
                    //TODO needs something else to show user error
                }
                var ownerData = GetRockieLevelData(ownerEmpId.ToString());
                var ownerlevel = Convert.ToInt32(ownerData.Level);

                var authorLevel = Convert.ToInt32(authorData.Level);
                var owner = GetOwnerViewModel(order, ownerlevel);
              
                var author = GetAuthorViewModel(order, authorLevel);
                RemoveExistingRouting(order.Id);
                if (CheckOrderKitPermission(author))
                {
                    if (isOrderKit(order))
                    {
                        AutoApprove(order, author);
                        return false;
                    }
                }
                var compareResult = CompareOwnerLevelWithPO(order, owner);
                if (compareResult > 0)
                {
                    GreaterWay(owner, order, ReportingStructure.Within);
                    // this._db.Entry(order).State = EntityState.Modified;
                    order.Status = _statusRepository.Get(x => x.Name == StatusEnum.Pending.ToString()).FirstOrDefault();
                    order.LastModifiedBy = _curUser;
                    order.LastModifiedDate = DateTime.Now;
                    _orderRepository.Update(order);

                    Db.SaveChanges();
                    _data.CheckForSubstitution(order);
                    result = false; //Means all good, no MultiApprovers
                }
                else
                {
                    result = LessWay(author, owner, order);
                }
                _data.CheckForSubstitution(order);
            }
            catch (Exception e)
            {
                  log.Error("Routing.Start(orderId:"+order.Id+")",e);
            }
            
            return result;
        }

        private void RemoveExistingRouting(int orderId)
        {
            var existingRoutes = _routeRepository.Get(x => x.Order.Id == orderId).Include(x => x.Approver).ToList();
            if (!existingRoutes.Any()) return;
            foreach (var route in existingRoutes)
            {
                //                route.IsDeleted = true;
                //              route.Approver.IsDeleted = true;
                _approverRepository.Delete(route.Approver);
                _routeRepository.Delete(route);
            }
            Db.SaveChanges();
        }

        public bool isOrderKit(Order order)
        {
            var accountsList = order.OrderItems.Select(x => x.Account.Id).ToList();
            var orderKit =
                _orderItemKitRepository.Get(x =>
                    !x.IsDeleted && x.CostCentre.Id == order.CostCentre.Id
                    && accountsList.Contains(x.Account.Id)).FirstOrDefault();
            if (orderKit != null)
            {
                return true;
            }
            return false;
        }

        public bool CheckOrderKitPermission(AuthorViewModel author)
        {
            var isPermitted = author.User.UserOrderSettings?.AutoApproveItemKit ?? false;
            return isPermitted;
        }

        public AuthorViewModel GetAuthorViewModel(Order order, int level)
        {
            var author = new AuthorViewModel()
            {
                Id = order.Author.Id,
                EmpId = order.Author.UserInfo.EmployeeId,
                Name =
                    order.Author.UserInfo.FirstName + " " + order.Author.UserInfo.LastName,
                CostCentre = _orderCostCentre,
                User = order.Author,
                Limit =
                    _levelRepository.Get(x => x.Code == level)
                    .Select(x => x.Value)
                    .FirstOrDefault(),
                Level = level,
                UserName = order.Author.UserName
            };
            return author;
        }

        public OwnerViewModel GetOwnerViewModel(Order order, int level)
        {
            var owner = new OwnerViewModel()
            {
                Id = _orderCostCentre.Owner.Id,
                EmpId = _orderCostCentre.Owner.UserInfo.EmployeeId,
                Name = _orderCostCentre.Owner.UserInfo.FirstName + " " + _orderCostCentre.Owner.UserInfo.LastName,
                CostCentre = _orderCostCentre,
                UserName = _orderCostCentre.Owner.UserName,
                Level = level,
                User = _orderCostCentre.Owner,
                Limit = _levelRepository.Get(x => x.Code == level).Select(x => x.Value).FirstOrDefault(),
            };
            return owner;
        }

        //public int GetCCOwnerEmpId(CostCentre cc)
        //{
        //    return cc.Owner.EmployeeId;
        //}

        public vRockyEmployees GetRockieLevelData(string empId)
        {
           
           return _rockyEmployeesRepository.Get(x => x.EmpNo == empId && x.Active == 1).FirstOrDefault();
        }

        /// <summary>
        /// Compare owner level with order $$
        /// </summary>
        /// <param name="order"></param>
        /// <param name="owner"></param>
        /// <returns>1 - Greater; 0 - Less</returns>
        public int CompareOwnerLevelWithPO(Order order, OwnerViewModel owner)
        {
            var ownerLevel = Convert.ToInt32(owner.Level);
            var ownerLevelLimit = _levelRepository.Get(x => x.Code == ownerLevel).Select(x => x.Value).FirstOrDefault();
            var isForeignCurrency = order.OrderItems[0].Currency.Id != 1;
            if (isForeignCurrency)
            {
                var convertedTotalExGst = order.TotalExGST * order.OrderItems[0].CurrencyRate;
                if (convertedTotalExGst >= ownerLevelLimit)
                {
                    return 1; //greater
                }
                return 0; //less
            }
            return order.TotalExGST >= ownerLevelLimit ? 1 : 0;
        }

        public void GreaterWay(OwnerViewModel owner, Order order, ReportingStructure rs )
        {
            //надо получить список из 2-х апруверов, владелец и тот кто над ним.
            var approver = new Approver()
                               {
                                   User = owner.User,
                                   Level = owner.Level,
                                   Limit = owner.Limit,
                                   CreatedBy = _curUser,
                                   LastModifiedBy = _curUser,
                                   DateCreated = DateTime.Now,
                                   LastModifiedDate = DateTime.Now
                               };
            _approverRepository.Add(approver);
            CreateRoute(order, approver, 1);
            
            var compareResult = CompareOwnerLevelWithPO(order, owner);
            if (compareResult > 0) 
            {
                GetApproverAboveOwner(owner, order);
            }
            try
            {
                Db.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {

            }
            
        }

        private void CreateRoute(Order order, Approver approver, int number)
        {
            var exRoute = _routeRepository.Get(x => x.Order.Id == order.Id && x.Number == number).FirstOrDefault();
            if (exRoute != null)
            {
                exRoute.Number = number;
                exRoute.Approver = approver;
                //exRoute.LastModifiedBy = _curUser;
                //exRoute.LastModifiedDate=DateTime.Now;
                exRoute.IsDeleted = false;
                _routeRepository.Update(exRoute);
            }
            else { 
            var route = new Route()
                            {
                                Order = order,
                                Number = number,
                                Approver = approver,
                                LastModifiedDate = DateTime.Now,
                                DateCreated = DateTime.Now,
                                CreatedBy = _curUser,
                                LastModifiedBy = _curUser
            };
                _routeRepository.Add(route);
            }
        }

        private void GetApproverAboveOwner(OwnerViewModel owner, Order order)
        {
            CheckApprover(owner.EmpId, order);
        }

        private void CheckApprover(int empId, Order order)
        {
           
            var isForeignCurrency = order.OrderItems[0].Currency.Id != 1;
            var totalExGst = order.TotalExGST;
            if (isForeignCurrency)
            {
                totalExGst = order.TotalExGST * order.OrderItems[0].CurrencyRate;
            }
            var manager =
                _rockyEmployeesRepository.Get(x => x.EmpNo == empId.ToString() && x.Active == 1).Select(x =>new { x.ManagerEmpNo, x.ManagerLevel}).FirstOrDefault();
            var managerLimit =
                _levelRepository.Get(x => x.Code.ToString() == manager.ManagerLevel).Select(x => x.Value).FirstOrDefault();
            if (managerLimit > totalExGst)
            {
                //TODO: какая то лажа, надо это вынести отсюда
                var approver = new Approver()
                                   {
                                       Level = Convert.ToInt32(manager.ManagerLevel),
                                       Limit = managerLimit,
                                       User =
                                           _userRepository.Get(x => x.EmployeeId.ToString() == manager.ManagerEmpNo).FirstOrDefault(),
                                   };
                _approverRepository.Add(approver);
                CreateRoute(order, approver, 2);
                //db.Routes.Add(route);
            }
            else
            {
                if (manager.ManagerEmpNo != "")
                {
                    CheckApprover(Convert.ToInt32(manager.ManagerEmpNo), order);
                }
            }
        }

        public bool LessWay(AuthorViewModel author, OwnerViewModel owner, Order order )
            
        {
           
          
            var isMutliApprove = false;
            //check is Author within CC report structure(RS)
            if (!IsAuthorWithinRS(owner, author))
            {//if no then back to GreaterWay
                GreaterWay(owner, order,ReportingStructure.Outside);
                //this._db.Entry(order).State = EntityState.Modified;
                order.Status = _statusRepository.Get(x => x.Name == StatusEnum.Pending.ToString()).FirstOrDefault();
                _orderRepository.Update(order);
                Db.SaveChanges();
                _data.CheckForSubstitution(order);
            }
            else
            {
                if (IsAutoApprove(author, order))
                {
                    //AUTO APPROVE
                    AutoApprove(order,author);
                    return false; //Means all good
                }
                else
                {
                    var approverList = GetApproverList(author, owner, order);
                    if (approverList.Count > 1)
                    {
                        isMutliApprove = true;
                        ApproverListVM = GetApproverListViewModel(approverList);
                    }
                    else
                    {
                        if (approverList.Count > 0)
                        {
                            _approverRepository.Add(approverList[0]);
                            CreateRoute(order, approverList[0], 1);
                            Db.SaveChanges();
                        }
                    }

                    if (!isMutliApprove) //DO not change Status if it's multi
                    {
                        order.Status = _statusRepository.Get(x => x.Name == StatusEnum.Pending.ToString()).FirstOrDefault();
                        _orderRepository.Update(order);
                        Db.SaveChanges();
                        _data.CheckForSubstitution(order);
                    }
                }
            }
            return isMutliApprove;
        }

        private List<ApproverViewModel> GetApproverListViewModel(List<Approver> approverList)
        {
            var list = new List<ApproverViewModel>();
            foreach (var approver in approverList.Where(x=>x.User!=null))
            {
                var item = new ApproverViewModel()
                               {
                                   Level = approver.Level,
                                   Limit = approver.Limit,
                                   UserId = approver.User.Id,
                                   FullName =
                                       approver.User.UserInfo.FirstName + " "
                                       + approver.User.UserInfo.LastName
                               };
                list.Add(item);
            }
            return list.OrderBy(x => x.FullName).ToList();
        }

        private bool IsAuthorWithinRS(OwnerViewModel owner,AuthorViewModel author)
        {
            var result = false;
          
            //нужно получить всю RS по данному CC
            var rsList = GetRSForCC(owner.EmpId);
            //if (addauthor == true)
            //{
            //    rsList.Add(author.EmpId.ToString());
            //}
            //added to fix issue with person with same level as owner
            if (owner.Level == author.Level && owner.CostCentre == author.CostCentre && owner.Limit == author.Limit)
            {
                rsList.Add(author.EmpId.ToString());
            }
          
            result = rsList.Contains(author.EmpId.ToString());
            return result;
        }

        /// <summary>
        /// Getting list of Employer ID of Repost Structure for the CC
        /// </summary>
        /// <param name="ownerEmpId"></param>
        /// <returns></returns>
        public List<string> GetRSForCC(int ownerEmpId)
        {
            var list = new List<string>();
            
            list.Add(ownerEmpId.ToString());
            var isEnd = false;
            
            //find behind
            list.AddRange(GetBehindList(ownerEmpId.ToString()));

            var empId = ownerEmpId.ToString();
            do//find above
            {
                empId = GoAbove(empId);
                if (!String.IsNullOrEmpty(empId))
                {
                    list.Add(empId);
                }
                else
                {
                    isEnd = true;
                }
            }
            while (!isEnd);

            return list;
        }

        public List<string> GetRSForCCDetailed(int ownerEmpId)
        {
            var empId = ownerEmpId.ToString();
            var list = new List<string>();

            list.Add("0:"+empId);
            var isEnd = false;

            //find behind
            list.AddRange(GetBehindListDetailed(ownerEmpId.ToString(),0));

            var pseudoLevel = 0;
            do//find above
            {
                pseudoLevel++;
                empId = GoAbove(empId);
                if (!String.IsNullOrEmpty(empId))
                {
                    list.Add(pseudoLevel.ToString()+":"+empId);
                }
                else
                {
                    isEnd = true;
                }
            }
            while (!isEnd);

            return list;
        }

        private string GoAbove(string ownerEmpId)
        {
            var empId =
                _rockyEmployeesRepository.Get(x => x.EmpNo == ownerEmpId && x.Active==1).Select(x => x.ManagerEmpNo).FirstOrDefault();
            return empId;
        }

       
        private List<string> GetBehindList(string ownerEmpId, List<vRockyEmployees> vRockyList=null )
        {
            var list = new List<string>();
            //var tmpList = list;
            if (vRockyList == null) vRockyList = _rockyEmployeesRepository.Get(x => x.Active == 1).ToList();
            foreach (var empId in vRockyList.Where(x => x.ManagerEmpNo == ownerEmpId).Select(x => x.EmpNo).ToList())
            {
                list.Add(empId);
                list.AddRange(GetBehindList(empId,vRockyList));
            }
            return list;
        }
        private List<string> GetBehindListDetailed(string ownerEmpId, int pseudoLevel)
        {
            pseudoLevel--;
            var list = new List<string>();
            foreach (var empId in _rockyEmployeesRepository.Get(x => x.ManagerEmpNo == ownerEmpId && x.Active == 1).Select(x => x.EmpNo).ToList())
            {
                list.Add(pseudoLevel.ToString()+":"+empId);
                list.AddRange(GetBehindListDetailed(empId, pseudoLevel));
            }
            return list;
        }

        private List<vRockyEmployees> GetBehindEmployerList(string ownerEmpId, List<vRockyEmployees> vRockyList = null)
        {
            var list = new List<vRockyEmployees>();
            if (vRockyList == null) vRockyList = _rockyEmployeesRepository.Get(x => x.Active == 1).ToList();
            foreach (var emp in vRockyList.Where(x => x.ManagerEmpNo == ownerEmpId).ToList())
            {
                list.Add(emp);
                list.AddRange(GetBehindEmployerList(emp.EmpNo, vRockyList));
            }
            return list;
        }
       
        private bool IsAutoApprove(AuthorViewModel author, Order order)
        {
            var isForeignCurrency = order.OrderItems[0].Currency.Id != 1;
            if (isForeignCurrency)
            {
                var convertedTotalExGst = order.TotalExGST * order.OrderItems[0].CurrencyRate;
                return author.Limit >= convertedTotalExGst;
            }
            return author.Limit >= order.TotalExGST;
        }


        private void AutoApprove(Order order, AuthorViewModel author)
        {
            if (order.Status.Name == StatusEnum.Approved.ToString()) return; //Trying to stop duplicating Order Number
            var approver = new Approver()
            {
                User = author.User,
                Level = author.Level,
                Limit = author.Limit,
                IsDeleted = true,
            };
            CreateRoute(order, approver, 1);
            _approverRepository.Add(approver);
            //db.Routes.Add(route);
            Db.SaveChanges();
            order.Status = _statusRepository.Get(x => x.Name == StatusEnum.Approved.ToString()).FirstOrDefault();
            _orderRepository.Update(order);
            if (!IsRevision) SetOrderNumber(order);
            else Db.SaveChanges();
            SendEmail(order);
        }

        public List<Route> GetRouteForOrder(int orderId)
        {
            var route = _routeRepository.Get(x => x.Order.Id == orderId && !x.IsDeleted).OrderBy(x=>x.Number).ToList();
            return route;
        }

        public List<Approver> GetApproverList(AuthorViewModel author, OwnerViewModel owner, Order order)
        {
            var approveList = new List<Approver>();
            var list = new List<vRockyEmployees>();
            var totalExGst = order.TotalExGST;
            var isForeignCurrency = order.OrderItems[0].Currency.Id != 1;
            if (isForeignCurrency)
            {
                totalExGst = order.TotalExGST * order.OrderItems[0].CurrencyRate;
            }
            var levelGap = 1;
            list.AddRange(GetBehindEmployerList(owner.EmpId.ToString()));
            list.Add(_rockyEmployeesRepository.Get(x => x.EmpNo == owner.EmpId.ToString()).FirstOrDefault());
            var minLevel = _levelRepository.Get(x => x.Value >= totalExGst).Select(x => x.Code).FirstOrDefault();
            if (minLevel == 3 || minLevel==4 || minLevel==7) levelGap = 2;// Level 4 & 5 are same ($10000)
            var maxLevel = owner.Level;
            for (var i = minLevel; i <= maxLevel; i++)
            {
                var tmpList = list.Where(x => x.Level == i.ToString()).ToList();
                if ( tmpList.Count> 0)
                {
                    foreach (var emp in tmpList)
                    {
                        var approver = new Approver()
                        {
                            User = _userRepository.Get(x => x.UserInfo.EmployeeId.ToString() == emp.EmpNo).FirstOrDefault(),
                            Level = Convert.ToInt32(emp.Level),
                            Limit = _levelRepository.Get(x => x.Code == i).Select(x => x.Value).FirstOrDefault(),
                            CreatedBy = _curUser,
                            LastModifiedBy = _curUser,
                            DateCreated = DateTime.Now,
                            LastModifiedDate = DateTime.Now
                        };
                        approveList.Add(approver);
                    }
                    if(i>=minLevel+levelGap) break;  //pick min level and next one enought for approve |
                }
            }
            return approveList.ToList();

        }

        #endregion

        public void StartApproveOrder(NewPOViewModel model)
        {
            try
            {
                var order = _orderRepository.Find(model.Id);
                if (order.Status.Name == StatusEnum.Approved.ToString()) return; //Trying to stop duplicating Order Number                  
                if (ApproveOrder(order))
                {
                    IsRevision = order.RevisionQty > 0; //Do not change Order Number if it was the Revision
                    if(!IsRevision) SetOrderNumber(order);
                    //  GenerateOrderAtOldSystem(order); //Useless function now.
                    SendEmail(order);
                }
            }
            catch (Exception e)
            {
                _main.LogError("Routing.StartApproveOrder(orderID:" + model.Id + ")", e);
            }
        }

        public void SetOrderNumber(Order order)
        {
            try
            {
                var sysOrderNumber = _orderRepository.Get().Select(x => x.OrderNumber).Max() + 1;
                var oNumber = sysOrderNumber;
                while (_orderRepository.Get(x => x.OrderNumber == oNumber).FirstOrDefault()!=null)
                {
                    oNumber++;
                }
                order.OrderNumber = sysOrderNumber;
                _orderRepository.Update(order);
                Db.SaveChanges();
            }
            catch (Exception e)
            {
                _main.LogError("Routing.SetOrderNumber(orderID:" + order.Id + ")", e);
            }
        }

        public void SendEmail(Order order)
        {
            try
            {
                _output.Order = order;
                _output.CreatePDFContainer();
                var filePath = _output.CreatePDFFile(_output.PdfPOContainer, ControllerContext);
                _output.SendEmail(filePath);
            }
            catch (Exception e)
            {
                _main.LogError("Routing.SendEmail(orderID:" + order.Id + ")", e);
            }
        }

/*
        private void GenerateOrderAtOldSystem(Order order)
        {
            //generate approved order and items
            try
            {
                var oldEntityId = oldDb.tblEntities.Where(x => x.Code == order.Entity.Code).Select(x => x.EntityID).FirstOrDefault();
                var oldCCId= oldDb.tblCostCentres.Where(x => x.Active && x.CostCentreCode==order.CostCentre.Code).Select(x=>x.CostCentreID).FirstOrDefault();

                var poHeader = new tblPOHeader()
                                   {
                                       Active = true,
                                       Author = "ONEHARVEST\\" + order.Author.UserName,
                                       Authoriser = db.Routes.Where(x => x.Order.Id == order.Id).Select(x => new { Name = x.Approver.User.UserInfo.FirstName + " " + x.Approver.User.UserInfo.LastName }).Select(x => x.Name).FirstOrDefault(),
                                       Comments = order.Comment,
                                       ContactEmail = order.User.UserInfo.Email,
                                       ContactName = order.User.UserInfo.FirstName + " " + order.User.UserInfo.LastName,
                                       ContactPhone = order.User.UserInfo.PhoneWork,
                                       CostCentreID = oldCCId,
                                       Created = order.DateCreated,
                                       DeliveryID = oldDb.tblPODeliveries.Where(x => x.DeliveryName == order.DeliveryAddress.Name).Select(x => x.DeliveryID).FirstOrDefault(),
                                       EntityID = oldEntityId,
                                       GroupID = oldDb.tblGroups.Where(x => x.GroupName == order.ReceiptGroup.Name).Select(x => x.GroupID).FirstOrDefault(),
                                       OrderDate = order.OrderDate,
                                       RevisionNumber = order.RevisionQty,
                                       StatusID = 2,
                                       SupplierContact = oldDb.tblSuppliers.Where(x => x.SupplierID == order.SupplierId).Select(x => x.Contact).FirstOrDefault(),
                                       SupplierEmail = order.SupplierEmail,
                                       SupplierFax = oldDb.tblSuppliers.Where(x => x.SupplierID == order.SupplierId).Select(x => x.Fax).FirstOrDefault(),
                                       TransmissionMethod = 2,
                                       Updated = order.LastModifiedDate,
                                       UserID = "ONEHARVEST\\" + order.LastModifiedBy,
                                       SupplierID = order.SupplierId,
                                       InternalComments = order.InternalComment,
                                       receiptor = "ONEHARVEST\\" + order.User.UserName,
                                       DecliningComments = String.Empty

                                   };

                foreach (var item in order.OrderItems)
                {
                    var oldAccId= oldDb.tblAccounts.Where(x=>x.Active && x.AccountCode==item.Account.Code).Select(x=>x.AccountID).FirstOrDefault();
                    var oldSubAccId =item.SubAccount!=null ? oldDb.tblSubAccounts.Where(x=>x.Active && x.SubAccountCode==item.SubAccount.Code).Select(x=>x.SubAccountID).FirstOrDefault(): 0;
                    var entityCCAccId = 0;
                    entityCCAccId = oldSubAccId != 0 ? this.oldDb.tblEntityCCAccs.Where(x => x.Active && x.AccountID == oldAccId && x.CostCentreID==oldCCId && x.EntityID==oldEntityId && x.SubAccountID==oldSubAccId).Select(x=>x.EntityCCAccID).FirstOrDefault() : 
                                        this.oldDb.tblEntityCCAccs.Where(x => x.Active && x.AccountID == oldAccId && x.CostCentreID == oldCCId && x.EntityID == oldEntityId).Select(x => x.EntityCCAccID).FirstOrDefault();
                    var poDetail = new tblPODetail()
                                       {
                                           UserID = "ONEHARVEST\\" + order.LastModifiedBy,
                                           Active = true,
                                           Created = item.DateCreated,
                                           Updated = item.LastModifiedDate,
                                           UnitPrice = item.IsGSTInclusive ?  Convert.ToDecimal(item.UnitPrice - (item.TotalTax / item.Qty)) : Convert.ToDecimal(item.UnitPrice),
                                           Taxable = item.IsTaxable,
                                           Status = 2,
                                           Revision = item.RevisionQty,
                                           QuantityOrdered = item.Qty,
                                           QuantityOpen = item.Qty,
                                           LineNumber = item.LineNumber,
                                           GSTAmount = Convert.ToDecimal(item.TotalTax/item.Qty),//ERROR -> should be GST for 1 item
                                           DueDate = item.DueDate,
                                           Details = item.Description ?? String.Empty,
                                           CapexID = order.Capex_Id!=0 ?order.Capex_Id: -1,
                                           CurrencyID = 1,
                                           CurrencyRate = Convert.ToDecimal(1.00),
                                           EntityCCAccID = entityCCAccId,
                                           EstimateOnly = false,
                                           BlockID = string.Empty,
                                           AssetID = string.Empty,
                                           BudgetItem = false,
                                           HDPlanItem = false,
                                           SupplierPartNo = string.Empty
                                       };
                    poHeader.tblPODetails.Add(poDetail);
                }
                var orderNumber = oldDb.tblPOHeaders.Select(x => x.PONumber).Max();
                db.Entry(order).State=EntityState.Modified;
                order.OrderNumber = orderNumber + 1;
                poHeader.PONumber = orderNumber + 1;
                oldDb.tblPOHeaders.Add(poHeader);
                oldDb.SaveChanges();
                db.SaveChanges();

                foreach (var item in poHeader.tblPODetails)
                {
                    var routingTable = new tblRoutingTable()
                                           {
                                               Active = true,
                                               UserID = "ONEHARVEST\\" + order.User.UserName,
                                               Created = DateTime.Now,
                                               Updated = DateTime.Now,
                                               ListID = 991953,
                                               ApprovalDate = DateTime.Now,
                                               ApprovalStatus = 1,
                                               POLineID = item.POLineID
                                           };
                    oldDb.tblRoutingTables.Add(routingTable);
                }

                oldDb.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


        }
*/

        public bool ApproveOrder(Order order)
        {
            try
            {
                var isMyTurn = true;
                var route =
                    _routeRepository.Get(x => !x.IsDeleted && x.Order.Id == order.Id && x.Approver.User.EmployeeId == CurEmpId).FirstOrDefault();

                if (route != null)
                {
                    if (route.Number > 1)
                    {
                        isMyTurn = CheckTurn(route);
                    }
                    if (isMyTurn)
                    {
                        //this._db.Entry(route).State = EntityState.Modified;
                        route.IsDeleted = true;
                        route.Approver.IsDeleted = true;
                        _routeRepository.Update(route);
                        _approverRepository.Update(route.Approver);
                        //route.LastModifiedBy = _curUser;
                        //route.LastModifiedDate = DateTime.Now;
                        
                        //route.Approver.LastModifiedBy = _curUser;
                        //route.Approver.LastModifiedDate = DateTime.Now;

                        Db.SaveChanges();

                        var count = _routeRepository.Get(x => !x.IsDeleted && x.Order.Id == order.Id).ToList().Count;
                        if (count > 0)
                        {
                            _data.CheckForSubstitution(order);
                            return false;
                        }

                        try
                        {
                            _data.GenerateOrderLog(order,OrderLogSubject.EPO);
                            var status =_statusRepository.Get(x => x.Name == StatusEnum.Approved.ToString() && !x.IsDeleted).FirstOrDefault();
                            
                            order.Status = status;
                           
                            foreach (var t in order.OrderItems)
                            {
                                t.Status = status;
                                _orderItemRepository.Update(t);
                                //t.LastModifiedBy = _curUser;
                                //t.LastModifiedDate = DateTime.Now;
                            }
                            _orderRepository.Update(order);
                        }
                        catch (Exception ex)
                        {
                            log.Error("Routing.ApproveOrder",ex);
                        }

                        Db.SaveChanges();

                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Routing.ApproveOrder(orderID:"+order.Id+")",e);
            }
            return true;
        }

        private bool CheckTurn(Route route)
        {
            var list = _routeRepository.Get(x => x.Order.Id == route.Order.Id && !x.IsDeleted && x.Approver.User.EmployeeId != CurEmpId).ToList();
            var res = !(list.Count > 0);
            return res;
        }
       

        public void SetAuthoriserToOrder(int orderId, string approverId)
        {
            var order = _orderRepository.Find(orderId);
            var user = _userRepository.Find(approverId);
            var rockieData = GetRockieLevelData(user.EmployeeId.ToString());

            var approver = new Approver()
                               {
                                   CreatedBy = _curUser,
                                   LastModifiedBy = _curUser,
                                   DateCreated = DateTime.Now,
                                   LastModifiedDate = DateTime.Now,
                                   Level = Convert.ToInt32(rockieData.Level),
                                   User = user
                               };
            approver.Limit = _levelRepository.Get(x => x.Code == approver.Level).Select(x => x.Value).FirstOrDefault();
            _approverRepository.Add(approver);
            CreateRoute(order, approver, 1);
            order.Status = _statusRepository.Get(x => x.Name == StatusEnum.Pending.ToString()).FirstOrDefault();
            order.LastModifiedBy = _curUser;
            order.LastModifiedDate = DateTime.Now;
            Db.SaveChanges();
            _data.CheckForSubstitution(order);

            Db.SaveChanges();
        }

        public RoutingStructureForCCViewModel GetRoutingStructureForCC(int ccId)
        {
            var cc = _costCentreRepository.Find(ccId);
            var ccOwner = _data.GetOwnerViewModel(cc, GetRockieLevelData(cc.Owner.EmployeeId.ToString()));
            var rsForCC = GetRSForCCDetailed(ccOwner.EmpId).OrderBy(x => x).ToList();
            var model = PrepareRoutingStructureForView(rsForCC, ccOwner);
            return model;
        }

        public RoutingStructureForCCViewModel PrepareRoutingStructureForView(List<string> rs, OwnerViewModel ccOwner)
        {
            var model = new RoutingStructureForCCViewModel();
            model.CostCentreCode = ccOwner.CostCentre.Code;
            model.CostCEntreOwnerEmpId = ccOwner.EmpId;
            model.CostCentreName = ccOwner.CostCentre.Name;
            model.CostCentreOwnerLevel = ccOwner.Level;
            model.CostCentreOwnerLimit = ccOwner.Limit;
            model.CostCentreOwner = ccOwner.Name;
            model.AboveList = new List<RoutingStructureElement>();
            model.BehindList = new List<RoutingStructureElement>();
            foreach (var empId in rs)
            {
                var pseudoLevel = Convert.ToInt32(empId.Substring(0, empId.IndexOf(":")));
                if (pseudoLevel > 0)
                {
                    model.AboveList.Add(_data.GetRSElement(empId, pseudoLevel,GetRockieLevelData(empId.Substring(empId.IndexOf(":")+1))));
                }
                else if (pseudoLevel < 0)
                {
                    model.BehindList.Add(_data.GetRSElement(empId, pseudoLevel, GetRockieLevelData(empId.Substring(empId.IndexOf(":") + 1))));
                }
            }
            return model;
        }

        #region Dispose

        bool disposed;
        //private PurchaseOrderContext db;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }





}