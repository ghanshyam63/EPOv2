namespace EPOv2.Business
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;

    using DomainModel.DataContext;
    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.Repositories.Interfaces;
    using EPOv2.ViewModels;

    public class UserInterface : IUserInterface
    {
        //private readonly IDataContext _db;
        private readonly OldPurchaseOrderContext _oldDb = new OldPurchaseOrderContext();
        private readonly string _curUser = HttpContext.Current.User.Identity.Name.Replace("ONEHARVEST\\", "");
        private readonly IData _data;

        private readonly IOrderRepository _orderRepository;

        private readonly IVoucherDocumentRepository _voucherDocumentRepository;
        private readonly IRouteRepository _routeRepository;

        private readonly IGroupMemberRepository _groupMemberRepository;

        private readonly ICapexRepository _capexRepository;

        private readonly ICapexRouteRepository _capexRouteRepository;

        private readonly IOrderItemLogRepository _itemLogRepository;

        private readonly IUserRepository _userRepository;

        private int _orderId;
        private readonly IAd _ad;

        private readonly IMain _main;
        public int curEmpId { get; set; }
        

        public List<int> SubUsersList { get; set; }

        public int orderId
        {
            get
            {
                return _orderId;
            }
            set
            {
                _orderId = value;
            } 
        }

        public UserInterface(IData data, IAd ad, IOrderRepository orderRepository, IRouteRepository routeRepository, IUserRepository userRepository, IVoucherDocumentRepository voucherDocumentRepository, IGroupMemberRepository groupMemberRepository, ICapexRepository capexRepository, ICapexRouteRepository capexRouteRepository, IOrderItemLogRepository itemLogRepository, IMain main)
        {
            //_db = db;
            _data = data;
            _ad = ad;
            _orderRepository = orderRepository;
            _routeRepository = routeRepository;
            _userRepository = userRepository;
            _voucherDocumentRepository = voucherDocumentRepository;
            _groupMemberRepository = groupMemberRepository;
            _capexRepository = capexRepository;
            _capexRouteRepository = capexRouteRepository;
            _itemLogRepository = itemLogRepository;
            _main = main;
            curEmpId = Convert.ToInt32(_ad.GetADUserEmpIDbyUserName(_curUser));

           // For Testing 
            curEmpId = 7459; //6611;  //1868 - Brad; 193 - Andrew ; Peter - 4823 ; Jason Schulz - 3605; Ian = 5009; Graeme Everingham - 217; Daveid Stewart - 738, Sam Robson - 307
        }


        public int GetOldData()
        {
            return _oldDb.tblEntities.Count();
        }

        public List<OrderDashboardViewModel> GetMyOrders()
        {
            var list = new List<OrderDashboardViewModel>();
            var ordersList =
                _orderRepository.Get(
                    x =>
                    x.Author.EmployeeId == curEmpId && !x.IsDeleted && x.Status.Name != StatusEnum.Closed.ToString() && x.Status.Name != StatusEnum.Cancelled.ToString() && x.Status.Name != StatusEnum.Receipt_in_Full.ToString().Replace("_"," "))
                    .ToList();
            foreach (var order in ordersList.OrderBy(x=>x.DateCreated))
            {
                //bool isLocked = order.Status.Name != Data.StatusEnum.Draft.ToString()
                //                 order.Status.Name != Data.StatusEnum.Pending.ToString();
                var odvm = new OrderDashboardViewModel();

                odvm.Id = order.Id;
                odvm.Date = order.DateCreated.ToShortDateString();
                odvm.OrderNumber = ((order.Status?.Name == StatusEnum.Draft.ToString())
                                    || (order.Status?.Name == StatusEnum.Pending.ToString()))
                                       ? "-"
                                       : order.OrderNumber.ToString();
                odvm.Status = order.Status?.Name;
                odvm.Supplier = order.SupplierId!=0 ?
                    _oldDb.tblSuppliers.Where(x => x.SupplierID == order.SupplierId)
                        .Select(x => new { Name = x.SupplierName + " - " + x.SupplierCode })
                        .Select(x => x.Name)
                        .First() : string.Empty;
                odvm.TempOrderNumber = order.TempOrderNumber.Substring(0, 6);
                odvm.Total = order.Total;
                odvm.isDeleteLocked = order.Status?.Name != StatusEnum.Draft.ToString()
                                      && order.Status?.Name != StatusEnum.Pending.ToString();
                odvm.isEditLocked = odvm.isDeleteLocked && order.Status?.Name != StatusEnum.Approved.ToString() && order.Status?.Name!=StatusEnum.Receipt_Partial.ToString().Replace("_"," ") && order.Status?.Name != StatusEnum.Receipt_in_Full.ToString().Replace("_", " ");
                odvm.isReadyForClose = CheckForClose(order);
                if (odvm.isDeleteLocked) odvm.isReadyForMatch = CheckOrderForMatching(order);
                if (order.Status?.Name == StatusEnum.Pending.ToString())
                {
                    var nextApprover = _routeRepository.Get(x => x.Order.Id == order.Id && !x.Approver.IsDeleted).OrderBy(x=>x.Number).Select(x=>x.Approver).FirstOrDefault();

                    if (nextApprover != null)
                    {
                        odvm.Status +="(to: "+ nextApprover.User.UserInfo.FirstName + " " + nextApprover.User.UserInfo.LastName +")";
                    }
                }
                list.Add(odvm);

            }
            return list;
        }

        private bool CheckOrderForMatching(Order order)
        {
            if (order.Status?.Name == StatusEnum.Approved.ToString()
                || order.Status?.Name ==  StatusEnum.Receipt_Partial.ToDescription())
            {
                var myReceiptGroupsId = _groupMemberRepository.Get(x => x.User.UserInfo.EmployeeId == curEmpId && !x.IsDeleted)
                    .GroupBy(x => x.Group)
                    .Select(x => x.Key.Id)
                    .ToList();
                if (!myReceiptGroupsId.Any()) return false;
                {
                    _orderRepository.LoadReference(order,x=>x.ReceiptGroup);
                    return myReceiptGroupsId.Contains(order.ReceiptGroup.Id);
                }
            }
            return false;
        }

        private bool CheckForClose(Order order)
        {
            var isInvoiceAuthorised = false;
            switch (order.Status?.Name)
            {
                case "Receipt Partial":
                    isInvoiceAuthorised = CheckInvoiceForAuthorisation(order.Id);
                    return isInvoiceAuthorised;
                case "Receipt in Full": 
                    isInvoiceAuthorised = CheckInvoiceForAuthorisation(order.Id);
                    return isInvoiceAuthorised;
                default:
                    return false;
            }
        }

        private bool CheckInvoiceForAuthorisation(int orderId)
        {
            var invoices =
                _voucherDocumentRepository.Get(
                    x =>
                    x.DocumentType.Name == DocumentTypeEnum.Purchase_Order.ToString().Replace("_", " ")
                    && x.Reference == orderId.ToString() && x.IsAuthorised).ToList();
            return invoices.Count > 0;
        }

        public List<OrderDashboardViewModel> GetOrdersForApprove()
        {
            var list = new List<OrderDashboardViewModel>();
          
            var isMyTurn = true;
            
            var routes =
                _routeRepository.Get(x => x.Approver.User.EmployeeId == curEmpId && !x.IsDeleted && !x.Approver.IsDeleted && !x.Order.IsDeleted && x.Order.Status.Name==StatusEnum.Pending.ToString())
                    .ToList();

           foreach (var route in routes.OrderBy(x=>x.Order.DateCreated))
            {
                if (route.Number > 1)
                {
                    isMyTurn=CheckTurn(route);
                }
                if (isMyTurn)
                {
                    var odvm = new OrderDashboardViewModel()
                                   {
                                       Id = route.Order.Id,
                                       Date = route.Order.DateCreated.ToShortDateString(),
                                       OrderNumber = ((route.Order.Status.Name == StatusEnum.Draft.ToString()) || (route.Order.Status.Name == StatusEnum.Pending.ToString())) ? "-" : route.Order.OrderNumber.ToString(),
                                       Status = route.Order.Status.Name,
                                       Supplier = route.Order.SupplierId!=0 ? _oldDb.tblSuppliers.Where(
                                               x => x.SupplierID == route.Order.SupplierId)
                                           .Select(x =>new{Name = x.SupplierName + " - " + x.SupplierCode}).Select(x=>x.Name).First() : string.Empty,
                                       TempOrderNumber = route.Order.TempOrderNumber.Substring(0,6),
                                       Total = route.Order.Total
                                   };
                    list.Add(odvm);
                }
                isMyTurn = true;
            }

            return list;
        }

        private bool CheckTurn(Route route)
        {
            var list = _routeRepository.Get(x => x.Order.Id == route.Order.Id && !x.IsDeleted && x.Approver.User.EmployeeId != curEmpId).ToList();
            var res = !(list.Count > 0);
            return res;
        }

        public List<Transaction> GetOrderTransactions()
        {
            var i = 1;
            var order = _orderRepository.Find(_orderId);
            var user = _ad.GetUser(order.CreatedBy);
            var transactionList = new List<Transaction>();
            var transaction = new Transaction()
                                  {
                                      Order = i,
                                      Type = Transaction.TransactionType.Created.ToString(),
                                      Date = order.DateCreated.ToShortDateString() + " " + order.DateCreated.ToShortTimeString(),
                                      User = user.UserInfo.FirstName + " " + user.UserInfo.LastName,
                                      Description = string.Empty
                                  };
            transactionList.Add(transaction);
            
            var routelist = _routeRepository.Get(x => x.Order.Id == _orderId).OrderBy(x=>x.Number).ToList();
            if (routelist.Count >= 0)
            {
                i++;
                var tr = new Transaction();
                tr.Type = Transaction.TransactionType.Route.ToString();
                tr.Order=i;
                foreach (var route in routelist)
                {
                    user = _userRepository.Find(route.Approver.User.Id);

                    tr.Description += user.UserInfo.FirstName + " " + user.UserInfo.LastName;
                    if (route.Number < routelist.Count) tr.Description += " -> ";
                }
                transactionList.Add(tr);
            }

            if (order.Status.Name != StatusEnum.Draft.ToString())
            {
                var approverList = routelist.Where(x => x.Approver.IsDeleted).OrderBy(x => x.Number).ToList();
                foreach (var approver in approverList)
                {
                    i++;
                    user= _ad.GetUser(approver.Approver.User.UserName);
                    var tr = new Transaction()
                                 {
                                     Type = Transaction.TransactionType.Approved.ToString(),
                                     User = user.UserInfo.FirstName + " " + user.UserInfo.LastName,
                                     Order = i,
                                     Date =
                                         approver.LastModifiedDate.ToShortDateString() + " "
                                         + approver.LastModifiedDate.ToShortTimeString(),
                                     Description = string.Empty
                                 };
                    transactionList.Add(tr);
                }
            }

            var matchingList = _data.GetMatchingList(order);
            var dateList = matchingList.GroupBy(x => new{ x.ReceviedDate, x.CreatedBy}).Select(x => new { date= x.Key.ReceviedDate, user=x.Key.CreatedBy, qty = x.Sum(matchOrder => matchOrder.Qty) }).OrderBy(x => x.date).ToList();

            foreach (var matchOrder in dateList)
            {
                i++;
                user = _userRepository.Get(x => x.UserName == matchOrder.user).FirstOrDefault();
                var ordQty = matchingList.Where(x => x.ReceviedDate == matchOrder.date).Sum(x => x.OrderItem.Qty);
                var tr = new Transaction()
                             {
                                 Type = Transaction.TransactionType.Received.ToString(),
                                 User = user != null ? user.GetFullName() : "Unknown",
                                 Date = matchOrder.date.ToShortDateString(),
                                 Order = i,
                                 Description ="Qty:"+ matchOrder.qty+"/"+ordQty
                };
                transactionList.Add(tr);
            }

            transactionList.AddRange(GetItemChangeHistory(order, i));

            return transactionList;
        }


        public List<Transaction> GetItemChangeHistory(Order order, int ord)
        {
            var trList = new List<Transaction>();
            var orderItemList = order.OrderItems.Select(x => x.Id).ToList();
            var list = new List<OrderItemLog>();
            foreach (var itemId in orderItemList)
            {
                var lr = _itemLogRepository.Get(x => x.LatestOrderItem.Id == itemId && !x.IsDeleted).OrderBy(x => x.Id).ToList();
                //list.AddRange(lr);
                for(var i=0; i<=lr.Count-2;i++)
                {
                    if (i == 0)
                    {
                       trList.AddRange(CompareForAnyCurrentOrderItemChanges(order.OrderItems.First(x => x.Id == itemId), lr[i], ord++));
                    }
                    else
                    {
                        
                        trList.AddRange(GetAnyOrderItemChangesInLog(lr[i], lr[i + 1], ord++));
                    }
                }
            }
            trList.Reverse();
            return trList;
        }

        private List<Transaction> GetAnyOrderItemChangesInLog(OrderItemLog itemLogCurr, OrderItemLog itemLogNext, int i)
        {
            var trList = new List<Transaction>();
            if (itemLogCurr.Account.Id != itemLogNext.Account.Id)
            {
                var transaction = new Transaction()
                {
                    Type = Transaction.TransactionType.Changes.ToString(),
                    User = _ad.GetADNamebyLogin(itemLogCurr.LastModifiedBy),
                    Date = itemLogCurr.LastModifiedDate.ToShortDateString(),
                    Order = i,
                    Description =
                                              "Line:" + itemLogCurr.LineNumber + "; Account: from: "
                                              + itemLogNext.Account.Code + " to: " + itemLogCurr.Account.Code
                };
                trList.Add(transaction);
            }
            if (itemLogCurr.SubAccount != null && itemLogNext.SubAccount != null)
            {
                if (itemLogCurr.SubAccount.Id != itemLogNext.SubAccount.Id)
                {
                    var transaction = new Transaction()
                                          {
                                              Type = Transaction.TransactionType.Changes.ToString(),
                                              User = _ad.GetADNamebyLogin(itemLogCurr.LastModifiedBy),
                                              Date = itemLogCurr.LastModifiedDate.ToShortDateString(),
                                              Order = i,
                                              Description =
                                                  "Line:" + itemLogCurr.LineNumber + "; Sub Account: from: "
                                                  + itemLogNext.SubAccount.Code + " to: "
                                                  + itemLogCurr.SubAccount.Code
                                          };
                    trList.Add(transaction);
                }
            }
            return trList;
        }

        private List<Transaction> CompareForAnyCurrentOrderItemChanges(OrderItem curItem, OrderItemLog orderItemLog, int i)
        {
            var trList = new List<Transaction>();
            if (curItem.Account.Id != orderItemLog.Account.Id)
            {
                var transaction = new Transaction()
                                      {
                                          Type = Transaction.TransactionType.Changes.ToString(),
                                          User = _ad.GetADNamebyLogin(curItem.LastModifiedBy),
                                          Date = curItem.LastModifiedDate.ToShortDateString(),
                                          Order = i,
                                          Description =
                                              "Line:" + curItem.LineNumber + "; Account: from: "
                                              + orderItemLog.Account.Code + " to: " + curItem.Account.Code
                                      };
                trList.Add(transaction);
            }
            if (curItem.SubAccount != null && orderItemLog.SubAccount != null)
            {
                if (curItem.SubAccount.Id != orderItemLog.SubAccount.Id)
                {
                    var transaction = new Transaction()
                                          {
                                              Type = Transaction.TransactionType.Changes.ToString(),
                                              User = _ad.GetADNamebyLogin(curItem.LastModifiedBy),
                                              Date = curItem.LastModifiedDate.ToShortDateString(),
                                              Order = i,
                                              Description =
                                                  "Line:" + curItem.LineNumber + "; Sub Account: from: "
                                                  + orderItemLog.SubAccount.Code + " to: "
                                                  + curItem.SubAccount.Code
                                          };
                    trList.Add(transaction);
                }
            }
            return trList;
        }

        [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
        public List<DashboardIncoiceViewModel> GetInvoicesForApprove()
        {
            var list = new List<DashboardIncoiceViewModel>();
            var doclist = _voucherDocumentRepository.Get(x => !x.IsAuthorised && x.Authoriser.EmployeeId == curEmpId
                && (x.DocumentType.Name == DocumentTypeEnum.Invoice.ToString() || x.DocumentType.Name == DocumentTypeEnum.GRNI_Invoice.ToString().Replace("_"," ")) 
                &&  x.Voucher.Status.Name == StatusEnum.Pending.ToString() && !x.IsDeleted && !x.Voucher.IsDeleted).GroupBy(x=>x.Voucher).ToList();
            foreach (var voucherDoc in doclist)
            {
                var invoiceViewModel = new DashboardIncoiceViewModel()
                                           {
                                               VoucherNumber =
                                                   voucherDoc.Key.VoucherNumber,
                                               InvoiceNumber =
                                                   voucherDoc.Key.InvoiceNumber,
                                               Id = voucherDoc.Key.Id,
                                               Supplier = voucherDoc.Key.SupplierCode,
                                               AttacheDateTime = voucherDoc.Key.DateCreated
                                           };
                list.Add(invoiceViewModel);
            }
            return list;
        }

        public SearchViewModel GetSearchViewModel()
        {
            var model = new SearchViewModel();
            model.Accounts = _main.GetAccountViewModels_All();
            model.Authorisers = _main.GetUserViewModels();
            model.Authors = model.Authorisers;
            model.Owners = model.Authors;
            model.CostCentres = _data.GetCostCentresVM(true);
            model.Entities = _data.GetEntities();
            model.Statuses = _data.GetStatuses();
            model.Suppliers = _main.GetSupplierViewModels();
            model.VoucherStatuses = _main.GetVoucherStatuses();
            model.Capexes = _data.GetCapexViewModels(true,true);
            

            return model;
        }

        public List<OrderDashboardViewModel> GetOrdersForMatching()
        {
           
               var list = new List<OrderDashboardViewModel>();

            var myReceiptGroups =
                _groupMemberRepository.Get(x => x.User.UserInfo.EmployeeId == curEmpId && !x.IsDeleted)
                    .GroupBy(x=>x.Group)
                    .Select(x => x.Key)
                    .ToList();
            if (myReceiptGroups.Count !=0)
            {
                var ordersList = new List<Order>();
                foreach (var myReceiptGroup in myReceiptGroups)
                {
                    ordersList.AddRange(_orderRepository.Get(
                        x =>
                        x.ReceiptGroup.Id == myReceiptGroup.Id
                        && !x.IsDeleted
                        && x.Status.Name != StatusEnum.Draft.ToString()
                        && x.Status.Name != StatusEnum.Pending.ToString()
                        && x.Status.Name != StatusEnum.Closed.ToString()
                        && x.Status.Name != StatusEnum.Cancelled.ToString()
                        && x.Status.Name != StatusEnum.Receipt_in_Full.ToString().Replace("_", " ")).ToList());        
                }

                foreach (var order in ordersList.OrderBy(x=>x.DateCreated))
                {
                   var odvm = new OrderDashboardViewModel();

                    odvm.Id = order.Id;
                    odvm.Date = order.DateCreated.ToShortDateString();
                    odvm.OrderNumber = order.OrderNumber.ToString();
                    odvm.Status =order.Status!=null ? order.Status.Name : "";
                    odvm.Supplier = order.SupplierId != 0 ?
                        _oldDb.tblSuppliers.Where(x => x.SupplierID == order.SupplierId)
                            .Select(x => new { Name = x.SupplierName + " - " + x.SupplierCode })
                            .Select(x => x.Name)
                            .First() : string.Empty;
                    odvm.TempOrderNumber = order.TempOrderNumber.Substring(0, 6);
                    odvm.Total = order.Total;
                    odvm.isEditLocked = true;
                    list.Add(odvm);
                }
            }

            return list;
        }

#region Capex Dashboard
        public List<CapexDashboardViewModel> GetMyCapexes()
        {
            var list = new List<CapexDashboardViewModel>();

            var capexList = _capexRepository.Get(x => !x.IsDeleted && (x.Author.EmployeeId == curEmpId || x.Owner.EmployeeId== curEmpId) && x.Status.Name != StatusEnum.Closed.ToString())
                .Include(x => x.CostCentre).Include(_ => _.Entity).Include(x => x.Owner.UserInfo).ToList();

            foreach (var capex in capexList)
            {
                var item = new CapexDashboardViewModel()
                {
                    DateCreated = capex.DateCreated.ToShortDateString(),
                    Status = capex.Status.Name,
                    CapexNumber = capex.CapexNumber,
                    Description = capex.Description,
                    Title = capex.Title,
                    TotalExGST = capex.TotalExGST,
                    CapexType = capex.CapexType,
                    CostCentre = capex.CostCentre.GetFullName(),
                    Entity = capex.Entity.GetFullName(),
                    Id = capex.Id,
                    Owner =capex.Owner.GetFullName(),
                };
                list.Add(item);
            }

            return list;
        }

        public IEnumerable<CapexDashboardViewModel> GetCapexesForApprove()
        {
            var isMyTurn = true;
            var list = new List<CapexDashboardViewModel>();
            var routes = _capexRouteRepository.Get(x => !x.IsDeleted && x.Approver.User.EmployeeId == curEmpId && !x.Capex.IsDeleted && x.Capex.Status.Name==StatusEnum.Pending.ToString()).ToList();
            foreach (var capexRoute in routes)
            {
                isMyTurn = CheckTurn(capexRoute);
                if (isMyTurn)
                {
                    var item = new CapexDashboardViewModel()
                                   {
                                       DateCreated = capexRoute.Capex.DateCreated.ToShortDateString(),
                                       Status = capexRoute.Capex.Status.Name,
                                       CapexNumber = capexRoute.Capex.CapexNumber,
                                       Description = capexRoute.Capex.Description,
                                       Title = capexRoute.Capex.Title,
                                       TotalExGST = capexRoute.Capex.TotalExGST,
                                       CapexType = capexRoute.Capex.CapexType,
                                       CostCentre =
                                           capexRoute.Capex.CostCentre.Code + " - " + capexRoute.Capex.CostCentre.Name,
                                       Entity = capexRoute.Capex.Entity.Code + " - " + capexRoute.Capex.Entity.Name,
                                       Id = capexRoute.Capex.Id,
                                       Owner =
                                           capexRoute.Capex.Owner.UserInfo.FirstName + " "
                                           + capexRoute.Capex.Owner.UserInfo.LastName
                                   };
                    list.Add(item);
                }
            }
            return list;
        }

        private bool CheckTurn(CapexRoute capexRoute)
        {
            var route =
                _capexRouteRepository.Get(x => !x.IsDeleted && x.Capex.Id == capexRoute.Capex.Id).Include(_=>_.Approver)
                    .OrderBy(x => x.Number)
                    .First();
            return route.Approver.Id == capexRoute.Approver.Id;
        }

        #endregion
    }
}