namespace EPOv2.Business
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using DomainModel.DataContext;
    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.Repositories.Interfaces;
    using EPOv2.ViewModels;
    using Intranet.ViewModels;
    using Microsoft.VisualBasic;

    using Serilog;

    //using PurchaseOrderOldContext = DomainModel.DataContext.PurchaseOrderOldContext;

    public partial class Main :IMain//, IDisposable
    {
        private readonly ILogger _logger = Serilog.Log.Logger;
        public IDataContext db { get; set; }

        //private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static OldPurchaseOrderContext oldDb = new OldPurchaseOrderContext();

        private readonly string _curUser = HttpContext.Current.User.Identity.Name.Replace("ONEHARVEST\\", "");
        
        private readonly QADLiveEntities _qadLive = new QADLiveEntities();
        public int CurEmpId { get; set; }

        public readonly IData Data;

        public static  IAd Ad;

        private readonly IOutput _output;

        private readonly IDNewsRepository _dNewsRepository;

        private readonly IDUserGroupRepository _dUserGroupRepository;

        private readonly IDTileRepository _dTileRepository;

        // private readonly IRouting _routing;

        public ControllerContext controllerContext { get; set; }
        private const int StartOrderNumber = 89000;
        private const int StartTempOrderNumber = 10;

#region Repository

        private readonly IOrderRepository _orderRepository;

        private readonly IEntityRepository _entityRepository;

        private readonly ICostCentreRepository _costCentreRepository;

        private readonly IStatusRepository _statusRepository;

        private readonly IUserRepository _userRepository;

        private readonly IDeliveryAddressRepository _deliveryAddressRepository;

        private readonly IOrderItemKitRepository _orderItemKitRepository;

        private readonly IOrderItemRepository _orderItemRepository;

        private readonly IGroupRepository _groupRepository;

        private readonly IAccountRepository _accountRepository;

        private readonly IMatchOrderRepository _matchOrderRepository;

        private readonly ICurrencyRepository _currencyRepository;

        private readonly IRouteRepository _routeRepository;

        private readonly IOrderItemLogRepository _orderItemLogRepository;

        private readonly ICapexRepository _capexRepository;

        private readonly ICapexRouteRepository _capexRouteRepository;

        private readonly ICapexApproverRepository _capexApproverRepository;

        private readonly IVoucherDocumentRepository _voucherDocumentRepository;

        private readonly IVoucherRepository _voucherRepository;

        private readonly IVoucherDocumentTypeRepository _voucherDocumentTypeRepository;

        private readonly IVoucherStatusRepository _voucherStatusRepository;

        private readonly IGroupMemberRepository _groupMemberRepository;

        private readonly ISubstituteApproverRepository _substituteApproverRepository;

        private readonly IDCalendarEventRepository _dCalendarEventRepository;

        private readonly IOrderLogRepository _orderLogRepository;

        private readonly IVoucherRouteRepository _voucherRouteRepository;

        private readonly IApproverRepository _approverRepository;

        #endregion

        public Main(ControllerContext context, IDataContext db, IOrderRepository orderRepository, 
            IStatusRepository statusRepository, IEntityRepository entityRepository, 
            ICostCentreRepository centreRepository, IUserRepository userRepository, 
            IDeliveryAddressRepository deliveryAddressRepository, ICostCentreRepository costCentreRepository, 
            IOrderItemKitRepository orderItemKitRepository, IOrderItemRepository orderItemRepository, 
            IAccountRepository accountRepository, IGroupRepository groupRepository, ICurrencyRepository currencyRepository, 
            IMatchOrderRepository matchOrderRepository, IRouteRepository routeRepository,
            IOrderItemLogRepository orderItemLogRepository, ICapexRepository capexRepository, 
            ICapexRouteRepository capexRouteRepository, ICapexApproverRepository capexApproverRepository, 
            IVoucherDocumentRepository voucherDocumentRepository, IVoucherRepository voucherRepository, 
            IVoucherDocumentTypeRepository voucherDocumentTypeRepository, IVoucherStatusRepository voucherStatusRepository,
            IGroupMemberRepository groupMemberRepository, ISubstituteApproverRepository substituteApproverRepository,
            IDCalendarEventRepository dCalendarEventRepository, IOrderLogRepository orderLogRepository, IVoucherRouteRepository voucherRouteRepository, IApproverRepository approverRepository,
            IData data, IAd ad, IOutput output, IDNewsRepository dNewsRepository, IDUserGroupRepository dUserGroupRepository, IDTileRepository dTileRepository)
        {
            controllerContext = context;
            this.db = db;
            _orderRepository = orderRepository;
            _statusRepository = statusRepository;
            _entityRepository = entityRepository;
            _userRepository = userRepository;
            _deliveryAddressRepository = deliveryAddressRepository;
            _costCentreRepository = costCentreRepository;
            _orderItemKitRepository = orderItemKitRepository;
            _orderItemRepository = orderItemRepository;
            _accountRepository = accountRepository;
            _groupRepository = groupRepository;
            _currencyRepository = currencyRepository;
            _matchOrderRepository = matchOrderRepository;
            _routeRepository = routeRepository;
            _orderItemLogRepository = orderItemLogRepository;
            _capexRepository = capexRepository;
            _capexRouteRepository = capexRouteRepository;
            _capexApproverRepository = capexApproverRepository;
            _voucherDocumentRepository = voucherDocumentRepository;
            _voucherRepository = voucherRepository;
            _voucherDocumentTypeRepository = voucherDocumentTypeRepository;
            _voucherStatusRepository = voucherStatusRepository;
            _groupMemberRepository = groupMemberRepository;
            _substituteApproverRepository = substituteApproverRepository;
            _dCalendarEventRepository = dCalendarEventRepository;
            _orderLogRepository = orderLogRepository;
            _voucherRouteRepository = voucherRouteRepository;
            _approverRepository = approverRepository;
            Data = data;
            Ad = ad;
            _output = output;
            _dNewsRepository = dNewsRepository;
            _dUserGroupRepository = dUserGroupRepository;
            _dTileRepository = dTileRepository;
            CurEmpId = Convert.ToInt32(Ad.GetADUserEmpIDbyUserName(_curUser));
           // CurEmpId = 6394;
        }

        /// <summary>
        /// Brand New Order
        /// </summary>
        /// <returns></returns>
        public NewPOViewModel GetNewPoViewModel()
        {
            var order = RaiseOrder();
            var newPO = new NewPOViewModel()
                              {
                                  Id = order.Id,
                                  Author = order.CreatedBy,
                                  OrderDate =order.DateCreated.ToShortDateString(),
                                  StatusName = StatusEnum.Draft.ToString(),
                                  RevisionQty = order.RevisionQty,
                                  OrderNumber = "-",
                                  TempOrderNumber = order.TempOrderNumber,
                                  CompanyBox = GetCompanyBoxViewModel(),
                                  DeliveryBox = GetDeliveryBoxViewModel(),
                                  SupplierBox = GetSupplierBoxViewModel(false),
                                  Items = new List<OrderItemTableViewModel>(),
                                  CurrencyName = "AUD$",
                                  CurrencySign = "$"
                              };
            newPO.UserId = newPO.DeliveryBox.CurrentUserId;
            return newPO;
        }

        /// <summary>
        /// Load existing PO
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderAction"></param>
        /// <param name="isMatching">is that view model for Matching order</param>
        /// <param name="isChangedByRevision"></param>
        /// <returns></returns>
        public NewPOViewModel GetExistingPoViewModel(int orderId, OrderAction orderAction, bool isMatching=false, bool isChangedByRevision= false)
        {
            var order = _orderRepository.Find(orderId);
            var isRevision = order.Status.Name == Data.GetStatusName(StatusEnum.Approved);
            isChangedByRevision = isRevision && isChangedByRevision;
            var revisionQty = order.RevisionQty;
            if (orderAction == OrderAction.Edit && isRevision) revisionQty++;
            var isBlocked = order.OrderItems.Count > 0;
            var po = new NewPOViewModel
                         {
                             Id = order.Id,
                             isRevision = isRevision,
                             IsChangedByRevision = isChangedByRevision,
                             Author = order.Author!=null ? order.Author.GetFullName() : string.Empty,
                             OrderDate = order.OrderDate.ToShortDateString(),
                             StatusName =
                                 order.Status != null
                                     ? order.Status.Name
                                     : StatusEnum.Draft.ToString(),
                             RevisionQty = revisionQty,
                             OrderNumber =
                                 ((order.Status.Name == Data.GetStatusName(StatusEnum.Draft))
                                  || (order.Status.Name == Data.GetStatusName(StatusEnum.Pending)))
                                     ? "-"
                                     : order.OrderNumber.ToString(),
                             TempOrderNumber = order.TempOrderNumber,
                             EntityId = order.Entity != null ? order.Entity.Id : 0,
                             CostCenterId = order.CostCentre != null ? order.CostCentre.Id : 0,
                             Transmission = order.TransmissionMethod,
                             UserId = order.User != null ? order.User.Id : "0",
                             SupplierId = order.SupplierId,
                             GroupId = order.ReceiptGroup != null ? order.ReceiptGroup.Id : 0,
                             DeliveryAddressId =
                                 order.DeliveryAddress != null ? order.DeliveryAddress.Id : 0,
                             CapexId = order.Capex_Id,
                             DeliveryBox =
                                 order.DeliveryAddress != null
                                     ? GetDeliveryBoxViewModel(
                                         order.DeliveryAddress.Id,
                                         order.User != null ? order.User.Id : "0",
                                         isBlocked)
                                     : GetDeliveryBoxViewModel(),
                             Items = GetItemTableViewModelList(order),
                             TotalOrder = order.Total,
                             TotalExGST = order.TotalExGST,
                             TotalGST = order.TotalGST,
                             Comment = order.Comment,
                             SupplierEmail = order.SupplierEmail,
                             InternalComment = order.InternalComment,
                             CompanyBox =
                                 GetCompanyBoxViewModel(
                                     order.Entity != null ? order.Entity.Id : 0,
                                     order.CostCentre != null ? order.CostCentre.Id : 0,
                                     order.Capex_Id,
                                     order.ReceiptGroup != null ? order.ReceiptGroup.Id : 0,
                                     isBlocked)
                         };
            if (po.Items.Count > 0)
            {
                po.IsForeignCurrency = po.Items[0].isForeignCurrency;
                po.CurrencyName = po.Items[0].CurrencyName;
                po.CurrencySign = po.Items[0].CurrencyName.Substring(3,1);
            }
            else
            {
                po.IsForeignCurrency = false;
                po.CurrencyName = "AUD$";
                po.CurrencySign = "$";
            }
            po.SupplierBox = GetSupplierBoxViewModel(
                order.SupplierId,
                po.EntityId,
                order.SupplierEmail,
                order.TransmissionMethod,
                isBlocked);
            po.IsRestrictedEdit = (po.StatusName == Data.GetStatusName(StatusEnum.Receipt_Partial) || (po.StatusName == Data.GetStatusName(StatusEnum.Receipt_in_Full)));
            //if (PO.IsRestrictedEdit)
            //{
            //    foreach (var item in PO.Items)
            //    {
            //        item.IsRestrictedEdit = true;
            //    }
            //}
            //Adding Matching table view
            if (!isMatching)
            {
                return po;
            }
            po.IsMatching = true;
            po.MatchingItems = GetItemMatchTableViewModelList(order);
            return po;
        }

     

        public Order RaiseOrder()
        {
            var tempOrderNumber = Guid.NewGuid().ToString();
            var order = new Order()
                            {
                                CreatedBy = _curUser,
                                DateCreated = DateTime.Now,
                                LastModifiedDate = DateTime.Now,
                                LastModifiedBy = _curUser,
                                CostCentre = null,
                                User = null,
                                RevisionQty = 0,
                                TempOrderNumber = tempOrderNumber,
                                OrderDate = DateTime.Now,
                                OrderNumber = 0
                            };
            var orderNumber = _orderRepository.Get(x=>x.OrderNumber<StartOrderNumber).Select(x => x.OrderNumber).Max();
            order.OrderNumber = orderNumber + 1;
            _orderRepository.Add(order);
            //TODO: ANY idea how to make smarter and more safty?
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                order.OrderNumber+=2;
                db.SaveChanges();
            }
            return order;
        }

        public List<OrderItemTableViewModel> GetItemTableViewModelList(Order order)
        {
            var itemVmList = new List<OrderItemTableViewModel>();
            foreach (var item in order.OrderItems)
            {
                 itemVmList.Add(GetOrderItemTableViewModel(item));
            }
            return itemVmList;
        }

        public List<OrderItemTableViewModel> GetItemMatchTableViewModelList(Order order)
        {
            var itemVmList = new List<OrderItemTableViewModel>();
            foreach (var item in order.OrderItems)
            {
                itemVmList.Add(GetOrderItemMatchTableViewModel(item));
            }
            return itemVmList;
        }

        public OrderItemTableViewModel GetOrderItemTableViewModel(OrderItem m)
        {
            var model = new OrderItemTableViewModel()
            {
                Id = m.Id,
                Qty = m.Qty,
                RevisionQty = m.RevisionQty,
                Description = m.Description,
                DueDate = m.DueDate.ToShortDateString(),
                Total = m.Total,
                TotalExTax = m.TotalExTax,
                TotalTax = m.TotalTax,
                Line = m.LineNumber,
                UnitPrice = m.UnitPrice,
                AccountName = m.Capex_Id != null ? m.Account.Code + " - " + m.Account.Name + " / " + oldDb.tblCapexes.Where(x => x.CapexID == m.Capex_Id).Select(x => x.CapexNumber).FirstOrDefault()
                                                : m.Account.Code + " - " + m.Account.Name,
                SubAccountName = m.SubAccount != null ? m.SubAccount.Code + " - " + m.SubAccount.Name : string.Empty,
                CurrencyId = m.Currency.Id,
                isForeignCurrency = m.Currency.Id!=1,
                CurrencyName = m.Currency.Name,
                ConvertedTotal = m.CurrencyRate!=0 ? m.Total*m.CurrencyRate : m.Total
            };
            return model;
        }

        public OrderItemTableViewModel GetOrderItemMatchTableViewModel(OrderItem m)
        {
            var qtyAlreadyReceived =
                _matchOrderRepository.Get(x => x.OrderItem.Id == m.Id && !x.IsDeleted).Sum(x => (double?)x.Qty ) ?? 0;
            var model = GetOrderItemTableViewModel(m);
            model.QtyAlreadyReceived = qtyAlreadyReceived;
            model.ReceviedDate = DateAndTime.Now.ToShortDateString();
            model.QtyReceived = 0;
            
            return model;
        }

        public CompanyBoxViewModel GetCompanyBoxViewModel()
        {
            //TODO: нужно с оптимизировать, нет смысла грузить DDLs если они заблокированы
            var defaultSettings =
                _userRepository.Get(x => x.EmployeeId == Data.curEmpId)
                    .Select(x => x.UserOrderSettings).Include(x=>x.DefaultEntity).Include(x=>x.DefaultCostCentre).Include(x=>x.DefaultGroup)
                    .FirstOrDefault();
            var companyBoxVm = new CompanyBoxViewModel()
                                   {
                                       ABN = string.Empty,
                                       ACN = string.Empty,
                                       CostCentres = Data.GetCostCentresVM(),
                                       Capexes = Data.GetCapexViewModels(true),
                                       Entities = Data.GetEntities(),
                                       SelectedEntity = defaultSettings?.DefaultEntity?.Id ?? 0,
                                       SelectedCostCenter = defaultSettings?.DefaultCostCentre?.Id ?? 0,
                                       SelectedReceiptGroup = defaultSettings?.DefaultGroup?.Id ?? 0
                                   };
            if (defaultSettings != null)
            {
                companyBoxVm.ReceiptGroups = Data.GetGroups();
            }
            return companyBoxVm;
        }

        public CompanyBoxViewModel GetCompanyBoxViewModel(int entityId, int ccId =0, int capexId=0, int groupId=0, bool isBlocked=false)
        {
            var entity = _entityRepository.Find(entityId);
            var defaultSettings =
               _userRepository.Get(x => x.EmployeeId == Data.curEmpId)
                   .Select(x => x.UserOrderSettings).Include(x=>x.DefaultCostCentre).Include(x=>x.DefaultGroup)
                   .FirstOrDefault();
            var model = new CompanyBoxViewModel()
            {
                ABN = entity.ABN,
                ACN = entity.ACN,
                SelectedEntity = entityId,
                CostCentres = Data.GetCostCentresVM(entityId),
                SelectedCostCenter = ccId,
                Capexes = ccId == 1 ? Data.GetCapexViewModels(false) : Data.GetCapexViewModels(true),
                SelectedCapexId = ccId == 1 ? capexId : 0,
                ReceiptGroups = Data.GetGroups(),
                Entities = Data.GetEntities(),
                SelectedReceiptGroup = groupId,
                IsBlocked = isBlocked,
                isCapex = ccId==1
            };
            if (defaultSettings != null)
            {
                if (ccId == 0) model.SelectedCostCenter = defaultSettings.DefaultCostCentre?.Id ?? 0;
                if (groupId == 0) model.SelectedReceiptGroup = defaultSettings.DefaultGroup?.Id ?? 0;
            }
            return model;
        }

        public DeliveryBoxViewModel GetDeliveryBoxViewModel()
        {
            var userId = Ad.GetUserIdByLogin(_curUser);
            var user = _userRepository.Find(userId);
            var deliveryBoxVm = new DeliveryBoxViewModel();
                                    
            if (user.UserOrderSettings?.DefaultDeliveryAddress != null)
            {
                if (user.UserOrderSettings.DefaultDeliveryAddress.Id != 0)
                {
                    deliveryBoxVm = GetDeliveryBoxViewModel(
                        user.UserOrderSettings.DefaultDeliveryAddress.Id,
                        userId);
                }
            }
            else
            {
                deliveryBoxVm = new DeliveryBoxViewModel()
                {
                    DeliveryAddresses = Data.GetDeliveryAddresses(),
                    Address = string.Empty,
                    PostCode = string.Empty,
                    City = string.Empty,
                    State = string.Empty,
                    Users = Ad.GetAllUsers(),
                    UserViewModels = GetUserViewModels(),
                    CurrentUserId = userId,
                    Email = user.UserInfo.Email,
                    Phone = user.UserInfo.PhoneWork
                };
            }
            return deliveryBoxVm;
        }
      

        public DeliveryBoxViewModel GetDeliveryBoxViewModel(int deliveryId, string userID="",bool isBlocked=false)
        {
            var userId = string.Empty;
            var delivery = _deliveryAddressRepository.Find(deliveryId);
            userId = string.IsNullOrEmpty(userID) ? Ad.GetUserIdByLogin(_curUser) : userID;
            var user = _userRepository.Find(userId);
            var model = new DeliveryBoxViewModel()
            {
                SelectedDeliveryAddress = deliveryId,
                DeliveryAddresses = Data.GetDeliveryAddresses(),
                Address = delivery.Address,
                PostCode = delivery.PostCode.ToString(),
                City =delivery.City ?? string.Empty,
                State =delivery.State!=null ? delivery.State.ShortName : string.Empty,
                Users = Ad.GetAllUsers(),
                UserViewModels = GetUserViewModels(),
                CurrentUserId = userId,
                Email = user.UserInfo.Email,
                Phone = user.UserInfo.PhoneWork,
                IsBlocked = isBlocked
            };
            return model;
        }
        public List<UserViewModel> GetUserViewModels()
        {
            //TODO: needs to add ROLE list
            var list = new List<UserViewModel>();
            var users = _userRepository.Get().Include(x => x.UserInfo).Include(x => x.UserOrderSettings).ToList();//Ad.GetAllUsers();
            foreach (var user in users)
            {
                var item = new UserViewModel()
                {
                    Id = user.Id,
                    FullName = user.UserInfo.FirstName + " " + user.UserInfo.LastName,
                    FirstName = user.UserInfo.FirstName,
                    LastName = user.UserInfo.LastName,
                    EmployeeId = user.EmployeeId.ToString(),
                    Login = user.UserName,
                    Email = user.Email
                };
                list.Add(item);
            }
            return list.OrderBy(x=>x.FullName).ToList();
        }

       

        public SupplierBoxViewModel GetSupplierBoxViewModel(bool doLoad=true)
        {
            var model = new SupplierBoxViewModel()
                            {
                                SupplierViewModels = doLoad ? GetSupplierViewModels() : new List<SupplierViewModel>(),
                                Address = string.Empty,
                                PostCode = string.Empty,
                                City = string.Empty,
                                EmailForSupplier = string.Empty,
                                State = string.Empty,
                                Contact = string.Empty,
                                Fax = string.Empty,
                                Phone = string.Empty,
                                SelectedTransmission = Business.Data.TransmissionMethod.Email.ToString()
                            };
            return model;
        }

        public SupplierBoxViewModel FilterSuppliersViewModel(int entityId)
        {
            var entityCode = _entityRepository.Get(x => x.Id == entityId).Select(x => x.CodeNumber).FirstOrDefault();
            var defaultSettings =
                   _userRepository.Get(x => x.EmployeeId == Data.curEmpId)
                       .Select(x => x.UserOrderSettings).FirstOrDefault();
            var model = new SupplierBoxViewModel();

            if (defaultSettings?.DefaultSupplierId != 0 && entityId == defaultSettings?.DefaultEntity?.Id)
            {
                    model = GetSupplierBoxViewModel(defaultSettings.DefaultSupplierId, entityId, "");
            }
            else
            {
                model = new SupplierBoxViewModel()
                {
                    SupplierViewModels = GetSupplierViewModels(entityCode),
                    Address = string.Empty,
                    PostCode = string.Empty,
                    City = string.Empty,
                    EmailForSupplier = string.Empty,
                    State = string.Empty,
                    Contact = string.Empty,
                    Fax = string.Empty,
                    Phone = string.Empty,
                    SelectedTransmission = Business.Data.TransmissionMethod.Email.ToString()
                };
            }
            return model;
        }

        public SupplierBoxViewModel GetSupplierBoxViewModel(int supplierId,int entityId, string email,string method="",bool isBlocked=false)
        {
            var entityCode = _entityRepository.Get(x => x.Id == entityId).Select(x => x.CodeNumber).FirstOrDefault();
            var supplier = oldDb.tblSuppliers.FirstOrDefault(x => x.SupplierID==supplierId);
            if (supplier == null)
            {
                var model = GetSupplierBoxViewModel();
                model.SelectedTransmission = method;
                return model;
            }
            else
            {
                var supplierList = GetSupplierViewModels(entityCode);
                if (supplierList.Select(x => x.Id).ToList().Contains(supplierId))
                {
                    var model = new SupplierBoxViewModel()
                                    {
                                        SelectedSupplier = supplierId,
                                        SupplierViewModels = supplierList,
                                        Address = supplier.Address1 + " " + supplier.Address2,
                                        PostCode = supplier.PostCode,
                                        City = supplier.City,
                                        EmailForSupplier =
                                            email != string.Empty ? email : supplier.Email,
                                        State = supplier.State,
                                        Contact = supplier.Contact,
                                        Fax = supplier.Fax,
                                        Phone = supplier.Phone,
                                        SelectedTransmission =
                                            method != string.Empty
                                                ? method
                                                : Business.Data.TransmissionMethod.Email.ToString(),
                                        IsBlocked = isBlocked
                                    };

                    return model;
                }
                else
                {
                   var model = new SupplierBoxViewModel()
                    {
                        SupplierViewModels = GetSupplierViewModels(entityCode),
                        Address = string.Empty,
                        PostCode = string.Empty,
                        City = string.Empty,
                        EmailForSupplier = string.Empty,
                        State = string.Empty,
                        Contact = string.Empty,
                        Fax = string.Empty,
                        Phone = string.Empty,
                        SelectedTransmission = Business.Data.TransmissionMethod.Email.ToString()
                    };
                    return model;
                }
            }
            
        }

        public List<SupplierViewModel> GetSupplierViewModels()
        {
            var list = new List<SupplierViewModel>();
            var suppliers = Data.GetSuppliers();
            foreach (var supplier in suppliers)
            {
                var item = new SupplierViewModel()
                               {
                                   Id = supplier.SupplierID,
                                   Code = supplier.SupplierCode,
                                   FullName = supplier.SupplierName + " - " + supplier.SupplierCode
                               };
                list.Add(item);
            }
            return list;
        }

        public List<SupplierViewModel> GetSupplierViewModels(int entityCode)
        {
            var list = new List<SupplierViewModel>();
            var suppliers = Data.GetSuppliers(entityCode);
            foreach (var supplier in suppliers)
            {
                var item = new SupplierViewModel()
                {
                    Id = supplier.SupplierID,
                    Code = supplier.SupplierCode,
                    FullName = supplier.SupplierName + " - " + supplier.SupplierCode
                };
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// Generate new Order Item
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public OrderItemViewModel GetOrderItemViewModel(NewPOViewModel m)
        {
            var cc = _costCentreRepository.Find(m.CostCenterId);
            var isPossibleItemKit = CheckForItemKit(cc.Id);
            //var order = _orderRepository.FirstOrDefault(x=>x.Id==m.Id);
            var order = _orderRepository.Find( m.Id);
            var line = order.OrderItems.Count()+1;
            var model = new OrderItemViewModel()
                            {
                                CostCentre = cc.Code + " - " + cc.Name,
                                Status = StatusEnum.Draft.ToString(),
                                RevisionQty = m.RevisionQty,
                                Line = line,
                                AccountViewModels = GetAccountViewModels(cc.Id),
                                SubAccountViewModels = GetAccountViewModels(cc.Id,1),
                                SelectedCapexId = m.CapexId,
                                Currencies = Data.GetCurrencies(),
                                SelectedCurrency = 1,//AUD by default
                                OrderId = m.Id,
                                IsTaxable = true,
                                IsGSTInclusive = false,
                                IsGSTFree=false,
                                TaxPercent = 10,
                                CurrencyRate = 1,
                                IsItemKit = isPossibleItemKit
                            };
            return model;
        }
        //var order = _orderRepository.FirstOrDefault(x => x.Id == m.Id);

        private bool CheckForItemKit(int ccid)
        {
            var first = _orderItemKitRepository.Get(x => !x.IsDeleted && x.CostCentre.Id == ccid).FirstOrDefault();
            if (first != null) return true;
            return false;
        }

        /// <summary>
        /// Prepare View Model for existing Item
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public OrderItemViewModel GetOrderItemViewModelForEdit(NewPOViewModel m)
        {
            var item = _orderItemRepository.Find(m.EditingItemId);
            var cc = _costCentreRepository.Find(m.CostCenterId);
            var isPossibleItemKit= CheckForItemKit(cc.Id);
            var model = new OrderItemViewModel()
            {
                itemId = item.Id,
                CostCentre = cc.Code + " - " + cc.Name,
                Status = item.Status.Name,//Data.StatusEnum.Draft.ToString(),
                RevisionQty = m.RevisionQty,//apply RevisionQty same as Order
                Line = item.LineNumber,
                AccountViewModels = GetAccountViewModels(cc.Id),
                SelectedAccount = item.Account.Id,
               // SelectedCapexId = item.Capex_Id,
                SubAccountViewModels = GetAccountViewModels(cc.Id, 1),
                SelectedSubAccount = item.SubAccount!=null ? item.SubAccount.Id :0,
                Currencies = Data.GetCurrencies(),
                SelectedCurrency = item.Currency.Id,
                CurrencyRate = Math.Abs(item.CurrencyRate) > 0 ? item.CurrencyRate : 1,
                OrderId = m.Id,
                IsTaxable = item.IsTaxable,
                IsGSTInclusive = item.IsGSTInclusive,
                IsGSTFree=item.IsGSTFree,
                TaxPercent = 10,
                Total = item.Total,
                TotalExTax = item.TotalExTax,
                TotalTax = item.TotalTax,
                Description = item.Description,
                Qty = item.Qty,
                UnitPrice = item.UnitPrice,
                DueDate = item.DueDate.ToShortDateString(),
                IsRevision = m.isRevision,
                IsItemKit = isPossibleItemKit,
                IsRestrictedEdit = m.IsRestrictedEdit
            };
            return model;
        }

        public List<AccountViewModel> GetAccountViewModels(int ccId, int type = 0)
        {
            var list = new List<AccountViewModel>();
            var accounts = type == 0 ? Data.GetCCAccountsByType(ccId) : Data.GetCCAccountsByType(ccId,1);
            foreach (var account in accounts)
            {
                var item = new AccountViewModel()
                               {
                                   Id = account.Id,
                                   FullName = account.Code + " - " + account.Name,
                                   Code = account.Code
                               };
                list.Add(item);
            }
            return list;
        }

        public void DeleteOrderItem(NewPOViewModel m)
        {
            var item = _orderItemRepository.Find(m.DeletingItemId);
            var order = _orderRepository.Find(m.Id);
            //this.db.Entry(order).State = EntityState.Modified;
            order.OrderItems.Remove(item);
            //this.db.Entry(item).State=EntityState.Modified;
            item.IsDeleted = true;
            item.LastModifiedBy = _curUser;
            item.LastModifiedDate = DateTime.Now;
            //TODO: необходима корректировка Line#
            order.Total -= item.Total;
            order.TotalExGST -= item.TotalExTax;
            order.TotalGST -= item.TotalTax;
            db.SaveChanges();
        }
        
        /// <summary>
        /// Pre-Save Order
        /// </summary>
        /// <param name="inputModel"></param>
        public void PreSaveOrder(NewPOViewModel inputModel)
        {
            var model = _orderRepository.Find(inputModel.Id);
            model.DeliveryAddress = _deliveryAddressRepository.Find(inputModel.DeliveryAddressId);
            model.Entity = _entityRepository.Find(inputModel.EntityId);
            model.ReceiptGroup = _groupRepository.Find(inputModel.GroupId);
            model.CostCentre = _costCentreRepository.Find(inputModel.CostCenterId);
            model.SupplierId = inputModel.SupplierId;
            model.User = _userRepository.Find(inputModel.UserId);
            model.Author = _userRepository.Get(x => x.UserName == _curUser).FirstOrDefault();
            model.TransmissionMethod = inputModel.Transmission;
            model.OrderDate = Convert.ToDateTime(inputModel.OrderDate);
            model.LastModifiedDate=DateTime.Now;
            model.LastModifiedBy = _curUser;
            model.Comment = inputModel.Comment;
            model.InternalComment = inputModel.InternalComment;
            model.Status = _statusRepository.Get(x => x.Name == inputModel.StatusName).FirstOrDefault();
            model.SupplierEmail = inputModel.SupplierEmail;
            model.Capex_Id = inputModel.CapexId;
            if(!inputModel.isRevision) model.RevisionQty =inputModel.RevisionQty;
            db.SaveChanges();
        }

        public Order SaveOrder(NewPOViewModel model)
        {
            var order = _orderRepository.Find(model.Id);
            //this.db.Entry(order).State = EntityState.Modified;
            order.DeliveryAddress = _deliveryAddressRepository.Find(model.DeliveryAddressId);
            order.Entity = _entityRepository.Find(model.EntityId);
            order.ReceiptGroup = _groupRepository.Find(model.GroupId);
            order.CostCentre = _costCentreRepository.Find(model.CostCenterId);
            order.SupplierId = model.SupplierId;
            order.User = _userRepository.Find(model.UserId);
            order.TransmissionMethod = model.Transmission;
            order.LastModifiedDate = DateTime.Now;
            order.LastModifiedBy = _curUser;
            order.Comment = model.Comment;
            order.InternalComment = model.InternalComment;
            order.Status = _statusRepository.Get(x => x.Name == StatusEnum.Draft.ToString()).FirstOrDefault();
            order.SupplierEmail = model.SupplierEmail;
            order.Capex_Id = model.CapexId;
            order.RevisionQty = model.RevisionQty;
            db.SaveChanges();
            return order;
        }

        public int SaveOrderItem(OrderItemViewModel m)
        {
            var item = _orderItemRepository.Find(m.itemId);
            var order = _orderRepository.Find(m.OrderId);
           
            if (item != null)
            {
                item.RevisionQty = m.RevisionQty;
                item.Qty = m.Qty;
                item.Account = _accountRepository.Find(m.SelectedAccount);
                item.SubAccount = _accountRepository.Find(m.SelectedSubAccount);
                item.Currency = _currencyRepository.Find(m.SelectedCurrency);
                item.CurrencyRate = m.CurrencyRate;
                item.Description = m.Description;
                item.DueDate = Convert.ToDateTime(m.DueDate);
                item.IsGSTInclusive = m.IsGSTInclusive;
                item.IsGSTFree = m.IsGSTFree;
                item.IsTaxable = m.IsTaxable;
                item.Total = m.Total;
                item.TotalExTax = m.TotalExTax;
                item.TotalTax = m.TotalTax;
                item.UnitPrice = m.UnitPrice;
                item.LastModifiedBy = _curUser;
                item.LastModifiedDate = DateTime.Now;
                item.Capex_Id = m.SelectedCapexId;

                if (order.OrderItems.Count < 2) //if just 1 item then apply Order Totals straight from Item
                {
                    order.Total = m.Total;
                    order.TotalExGST = m.TotalExTax;
                    order.TotalGST = m.TotalTax;
                }
                else
                {
                    order.Total = 0;
                    order.TotalExGST = 0;
                    order.TotalGST = 0;
                    foreach (var i in order.OrderItems)
                    {
                        order.Total += i.Total;
                        order.TotalExGST += i.TotalExTax;
                        order.TotalGST += i.TotalTax;
                    }
                }
            }
            else
            {
                item = CreateOrderItemFromViewModel(m);
                order.OrderItems.Add(item);
                order.Total += m.Total;
                order.TotalExGST += m.TotalExTax;
                order.TotalGST += m.TotalTax;
            }
           
            order.LastModifiedBy = _curUser;
            order.LastModifiedDate=DateTime.Now;
            _orderRepository.Update(order);
            db.SaveChanges();
            return order.Id;
        }


        public OrderItem CreateOrderItemFromViewModel(OrderItemViewModel m)
        {
           var item = new OrderItem()
            {
                LineNumber = m.Line,
                RevisionQty = m.RevisionQty,
                Account = _accountRepository.Find(m.SelectedAccount),
                SubAccount = _accountRepository.Find(m.SelectedSubAccount),
                Currency = _currencyRepository.Find(m.SelectedCurrency),
                CurrencyRate = m.CurrencyRate,
                Description = m.Description,
                DueDate = Convert.ToDateTime(m.DueDate),
                Qty = m.Qty,
                IsGSTInclusive = m.IsGSTInclusive,
                IsTaxable = m.IsTaxable,
                IsGSTFree=m.IsGSTFree,
                Total = m.Total,
                TotalExTax = m.TotalExTax,
                TotalTax = m.TotalTax,
                UnitPrice = m.UnitPrice,
                CreatedBy = _curUser,
                DateCreated = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                LastModifiedBy = _curUser,
                Status = _statusRepository.Get(x => x.Name == m.Status).FirstOrDefault(),
                Capex_Id = m.SelectedCapexId
            };
            return item;
        }

        public List<Order> GetOrderList()
        {
            var list = _orderRepository.Get().ToList();
            return list;
        }

        public void DeleteOrder(NewPOViewModel model)
        {
            if (model.Items != null)
            {
                foreach (var item in model.Items)
                {
                    DeleteItem(item);
                }
            }
            var order = _orderRepository.Find(model.Id);
            order.IsDeleted = true;
            order.LastModifiedBy = _curUser;
            order.LastModifiedDate = DateTime.Now;
            db.SaveChanges();
        }

        public void DeleteItem(OrderItemTableViewModel item)
        {
            var orderItem = _orderItemRepository.Find(item.Id);
            orderItem.IsDeleted = true;
            orderItem.LastModifiedBy = _curUser;
            orderItem.LastModifiedDate=DateTime.Now;
        }

       

        

        public List<Order> GetOrdersByAuthoriser(string authoriserId)
        {
            var orderList = new List<Order>();
            var list = _routeRepository.Get(x => x.Approver.User.Id == authoriserId && !x.Order.IsDeleted).ToList();
            foreach (var route in list)
            {
                var num =_routeRepository.Get(x => x.Order.Id == route.Order.Id).Max(x => x.Number);
                if (route.Number==num) orderList.Add(route.Order);
            }
            return orderList;
        }

        public void SaveMatchOrder(NewPOViewModel model)
        {
            try
            {
                var order = _orderRepository.Find(model.Id);
                foreach (var item in model.MatchingItems)
                {
                    var alreadyMatched =_matchOrderRepository.Get(x => !x.IsDeleted && x.Order.Id == model.Id && x.OrderItem.Id == item.Id).Select(x=>x.Qty).DefaultIfEmpty(0).Sum();
                    if (item.Qty >= alreadyMatched + item.QtyReceived)
                    {
                        var matchOrder = new MatchOrder()
                                             {
                                                 Qty = item.QtyReceived,
                                                 ReceviedDate = Convert.ToDateTime(item.ReceviedDate),
                                                 Order = order,
                                                 OrderItem = _orderItemRepository.Find(item.Id)
                                             };

                        _matchOrderRepository.Add(matchOrder);
                    }
                }
                db.SaveChanges(); //Need to Save MatchOrder objects before compare and making status decision 
                if (CheckOrderItemsQty(model.Id))
                {
                    Data.GenerateOrderLog(order, OrderLogSubject.EPOMatching);
                    order.Status = _statusRepository.Get(x => x.Name == "Receipt Partial").FirstOrDefault();
                }
                else
                {
                    Data.GenerateOrderLog(order, OrderLogSubject.EPOMatching);
                    order.Status = _statusRepository.Get(x => x.Name == "Receipt in Full").FirstOrDefault();
                    if (CheckForInvoice(order))
                    {
                        Data.GenerateOrderLog(order, OrderLogSubject.EPOMatching);
                        order.Status =_statusRepository.Get(x => x.Name == StatusEnum.Closed.ToString()).FirstOrDefault();
                    }
                }
                _orderRepository.Update(order);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Main.SaveMatchOrder(orderID:{orderID}",model.Id);   
                throw;
            }
        }

        public bool CheckOrderItemsQty(int orderId)
        {
            var matchQty = _matchOrderRepository.Get(x => x.Order.Id == orderId && !x.IsDeleted).Sum(x => (double?)x.Qty) ?? 0;
            var orderQty = _orderRepository.Get(x => x.Id == orderId).SelectMany(x => x.OrderItems).Sum(x=>(double?)x.Qty) ?? 0;
            return matchQty != orderQty; //if matchQty != orderQty then it's "PartialReceived"
        }

        public List<AccountViewModel> GetAccountViewModels_All()
        {
            var accounts = Data.GetAccountsAll();
            var list = accounts.Select(account => new AccountViewModel() { Id = account.Id, FullName = account.Code + " - " + account.Name, Code = account.Code }).ToList();
            return list;
        }

        #region Search EPO

        public SearchEPOResult SearchEPO(SearchViewModel model)
        {
            var orderList = new List<Order>();
            var searchResult = new SearchEPOResult();
            var searchEpoResultList = new List<SearchEPOResultItem>();

            if (!string.IsNullOrEmpty(model.OrderNumber)) orderList = FilterOrdersByOrderNumber(model.OrderNumber);
            if (!string.IsNullOrEmpty(model.DateFrom) || !string.IsNullOrEmpty(model.DateTo)) orderList = FilterOrdersByDates(model.DateFrom, model.DateTo, orderList);
            if (model.SelectedEntity != 0) orderList = FilterOrdersByEntity(model.SelectedEntity,orderList);
            if (model.SelectedCostCenter != 0) orderList = FilterOrdersByCostCentre(model.SelectedCostCenter, orderList);
            if (model.SelectedAccount != 0) orderList = FilterOrdersByAccount(model.SelectedAccount, orderList);
            if (model.SelectedSupplier != 0) orderList = FilterOrderBySupplier(model.SelectedSupplier, orderList);
            if (!string.IsNullOrEmpty(model.SelectedAuthor)) orderList = FilterOrdersByAuthor(model.SelectedAuthor, orderList);
            if (!string.IsNullOrEmpty(model.SelectedAuthoriser)) orderList = FilterOrderByAuthoriser(model.SelectedAuthoriser, orderList);
            if (model.SelectedStatus != 0) orderList = FilterOrderByStatus(model.SelectedStatus, orderList);
            if (!string.IsNullOrEmpty(model.Details)) orderList = FilterOrdersByDetails(model.Details, orderList);
            if (model.SelectedCapexId!=0) orderList = FilterOrderByCapex(model.SelectedCapexId, orderList);
            if (!string.IsNullOrEmpty(model.DateDueFrom) || !string.IsNullOrEmpty(model.DateDueTo)) orderList = FilterOrdersByDueDates(model.DateDueFrom, model.DateDueTo, orderList);
            if (!string.IsNullOrEmpty(model.DateRecFrom) || !string.IsNullOrEmpty(model.DateRecTo)) orderList = FilterOrdersByReceiptDates(model.DateRecFrom, model.DateRecTo, orderList);
            searchEpoResultList = searchEpoResultList.OrderByDescending(x => x.Date).ToList();
            searchEpoResultList = ConvertToSearchEPOResult(orderList);
            searchResult.SearchEpoResultItems = searchEpoResultList.OrderByDescending(x => x.Date).ToList();
            //TODO: Not 100% sure that I have to exclude "Draft" orders
            searchResult.OrderCount =
                searchResult.SearchEpoResultItems.Count(x => x.Status != StatusEnum.Draft.ToString());
            searchResult.Total =
                searchResult.SearchEpoResultItems.Where(x => x.Status != StatusEnum.Draft.ToString()).Sum(x=>x.Total);

            searchResult.UserRoles = new List<string>();
            searchResult.UserRoles = Data._ad.GetCurrentUserRoles();

            return searchResult;
        }

        private List<Order> FilterOrderByCapex(int capexId, List<Order> orderList)
        {
            orderList = orderList.Count == 0
                            ? _orderRepository.Get(x => x.Capex_Id == capexId && !x.IsDeleted).ToList()
                            : orderList.Where(x => x.Capex_Id == capexId).ToList();
            return orderList;
        }

        private List<Order> FilterOrdersByDetails(string details, List<Order> orderList)
        {
            orderList = orderList.Count == 0
                            ? _orderRepository.Get(
                                x =>
                                !x.IsDeleted && x.OrderItems.Any(y => !y.IsDeleted)
                                && x.OrderItems.Any(y => y.Description.Contains(details))).ToList()
                            : orderList.Where(
                                x =>
                                !x.IsDeleted && x.OrderItems.Any(y => !y.IsDeleted)
                                && x.OrderItems.Any(y => y.Description.Contains(details))).ToList();
            return orderList;
        }

        private List<Order> FilterOrderByStatus(int statusId, List<Order> orderList)
        {
            orderList = orderList.Count == 0
                            ? _orderRepository.Get(x => x.Status.Id == statusId && !x.IsDeleted).ToList()
                            : orderList.Where(x => x.Status!=null && x.Status.Id == statusId).ToList();
            return orderList;
        }

        private List<Order> FilterOrderByAuthoriser(string authoriserId, List<Order> orderList)
        {
            if (orderList.Count == 0)
            {
                orderList = GetOrdersByAuthoriser(authoriserId);
            }
            else
            {
                var orderIds = orderList.Select(x => x.Id).ToList();
                orderList =
                    _routeRepository.Get(x => x.Approver.User.Id == authoriserId && !x.Order.IsDeleted && orderIds.Contains(x.Order.Id))
                        .Select(x => x.Order)
                        .ToList();
            }
             
            return orderList;
        }

        private List<Order> FilterOrdersByAuthor(string authorId, List<Order> orderList)
        {
            orderList = orderList.Count == 0
                            ? _orderRepository.Get(x => !x.IsDeleted && x.Author.Id == authorId).ToList()
                            : orderList.Where(x => !x.IsDeleted && x.Author!=null && x.Author.Id == authorId).ToList();
            return orderList;
        }

        private List<Order> FilterOrderBySupplier(int supplierId, List<Order> orderList)
        {
            orderList = orderList.Count == 0
                            ? _orderRepository.Get(x => x.SupplierId == supplierId && !x.IsDeleted).ToList()
                            : orderList.Where(x => x.SupplierId == supplierId && !x.IsDeleted).ToList();
            return orderList;
        }

        private List<Order> FilterOrdersByAccount(int accountId, List<Order> orderList)
        {
            orderList = orderList.Count == 0
                            ? _orderRepository.Get(x => x.OrderItems.Any(y => y.Account.Id == accountId) && !x.IsDeleted)
                                  .ToList()
                            : orderList.Where(x => x.OrderItems.Any(y => y.Account.Id == accountId) && !x.IsDeleted)
                                  .ToList();
            return orderList;
        }

        private List<Order> FilterOrdersByCostCentre(int ccId, List<Order> orderList)
        {
            orderList = orderList.Count == 0
                            ? _orderRepository.Get(x => !x.IsDeleted && x.CostCentre.Id == ccId).ToList()
                            : orderList.Where(x => !x.IsDeleted && x.CostCentre!=null && x.CostCentre.Id == ccId).ToList();
            return orderList;
        }

        private List<Order> FilterOrdersByEntity(int entityId, List<Order> orderList)
        {
            orderList = orderList.Count == 0 ? _orderRepository.Get(x => !x.IsDeleted && x.Entity.Id == entityId).ToList() : orderList.Where(x => !x.IsDeleted && x.Entity.Id == entityId).ToList();
            return orderList;
        }

        private List<Order> FilterOrdersByOrderNumber(string orderNumber)
        {
            var ordNumber = Convert.ToInt32(orderNumber);
            return _orderRepository.Get(x => x.OrderNumber == ordNumber && !x.IsDeleted).ToList();
        }

        private List<Order> FilterOrdersByReceiptDates(string recDateFrom, string recDateTo, List<Order> orderList)
        {
            var statusList = new List<string>()
                                 {
                                     Data.GetStatusName(StatusEnum.Receipt_in_Full),
                                     Data.GetStatusName(StatusEnum.Receipt_Partial)
                                 };
            if (orderList.Count == 0)
            {
                if (!string.IsNullOrEmpty(recDateFrom) && !string.IsNullOrEmpty(recDateTo))
                {
                    var dateF = Convert.ToDateTime(recDateFrom);
                    var dateT = Convert.ToDateTime(recDateTo);
                    orderList = _orderRepository.Get(x => x.LastModifiedDate >= dateF && x.LastModifiedDate <= dateT && !x.IsDeleted && x.Author != null && statusList.Contains(x.Status.Name)).ToList();
                }
                else if (!string.IsNullOrEmpty(recDateFrom))
                {
                    var dateF = Convert.ToDateTime(recDateFrom);
                    orderList = _orderRepository.Get(x => x.LastModifiedDate >= dateF && !x.IsDeleted && x.Author != null && statusList.Contains(x.Status.Name)).ToList();
                }
                else if (!string.IsNullOrEmpty(recDateTo))
                {
                    var dateT = Convert.ToDateTime(recDateTo);
                    orderList =
                        _orderRepository.Get(x => x.LastModifiedDate <= dateT && !x.IsDeleted && x.Author != null && statusList.Contains(x.Status.Name)).ToList();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(recDateFrom) && !string.IsNullOrEmpty(recDateTo))
                {
                    var dateF = Convert.ToDateTime(recDateFrom);
                    var dateT = Convert.ToDateTime(recDateTo);
                    orderList =
                        orderList.Where(x => x.LastModifiedDate >= dateF && x.LastModifiedDate <= dateT && !x.IsDeleted && x.Author != null && statusList.Contains(x.Status.Name)).ToList();
                }
                else if (!string.IsNullOrEmpty(recDateFrom))
                {
                    var dateF = Convert.ToDateTime(recDateFrom);
                    orderList =
                       orderList.Where(x => x.LastModifiedDate >= dateF && !x.IsDeleted && x.Author != null && statusList.Contains(x.Status.Name)).ToList();
                }
                else if (!string.IsNullOrEmpty(recDateTo))
                {
                    var dateT = Convert.ToDateTime(recDateTo);
                    orderList =
                        orderList.Where(x => x.LastModifiedDate <= dateT && !x.IsDeleted && x.Author != null && statusList.Contains(x.Status.Name)).ToList();
                }
            }

            return orderList;
        }

        private List<Order> FilterOrdersByDueDates(string dueDateFrom, string dueDateTo, List<Order> orderList)
        {
            if (orderList.Count == 0)
            {
                if (!string.IsNullOrEmpty(dueDateFrom) && !string.IsNullOrEmpty(dueDateTo))
                {
                    var dateF = Convert.ToDateTime(dueDateFrom);
                    var dateT = Convert.ToDateTime(dueDateTo);
                    orderList = _orderRepository.Get(x => !x.IsDeleted && x.Author != null && x.OrderItems.Any(y=>y.DueDate>=dateF && y.DueDate<=dateT)).ToList();
                }
                else if (!string.IsNullOrEmpty(dueDateFrom))
                {
                    var dateF = Convert.ToDateTime(dueDateFrom);
                    orderList = _orderRepository.Get(x => !x.IsDeleted && x.Author != null && x.OrderItems.Any(y => y.DueDate >= dateF)).ToList();
                }
                else if (!string.IsNullOrEmpty(dueDateTo))
                {
                    var dateT = Convert.ToDateTime(dueDateTo);
                    orderList =
                        _orderRepository.Get(x => !x.IsDeleted && x.Author != null && x.OrderItems.Any(y => y.DueDate <= dateT)).ToList();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dueDateFrom) && !string.IsNullOrEmpty(dueDateTo))
                {
                    var dateF = Convert.ToDateTime(dueDateFrom);
                    var dateT = Convert.ToDateTime(dueDateTo);
                    orderList =
                        orderList.Where(x => !x.IsDeleted && x.Author != null && x.OrderItems.Any(y => y.DueDate >= dateF && y.DueDate <= dateT)).ToList();
                }
                else if (!string.IsNullOrEmpty(dueDateFrom))
                {
                    var dateF = Convert.ToDateTime(dueDateFrom);
                    orderList =
                       orderList.Where(x => !x.IsDeleted && x.Author != null && x.OrderItems.Any(y => y.DueDate >= dateF)).ToList();
                }
                else if (!string.IsNullOrEmpty(dueDateTo))
                {
                    var dateT = Convert.ToDateTime(dueDateTo);
                    orderList =
                        orderList.Where(x => !x.IsDeleted && x.Author != null && x.OrderItems.Any(y => y.DueDate <= dateT)).ToList();
                }
            }

            return orderList;
        }

        private List<Order> FilterOrdersByDates(string dateFrom, string dateTo, List<Order> orderList)
        {
            if (orderList.Count==0)
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    var dateF = Convert.ToDateTime(dateFrom);
                    var dateT = Convert.ToDateTime(dateTo);
                    orderList =
                        _orderRepository.Get(x => x.OrderDate >= dateF && x.OrderDate <= dateT && !x.IsDeleted && x.Author != null).ToList();
                }
                else if (!string.IsNullOrEmpty(dateFrom))
                {
                    var dateF = Convert.ToDateTime(dateFrom);
                     orderList =
                        _orderRepository.Get(x => x.OrderDate >= dateF && !x.IsDeleted && x.Author != null).ToList();
                }
                else if (!string.IsNullOrEmpty(dateTo))
                {
                    var dateT = Convert.ToDateTime(dateTo);
                    orderList =
                        _orderRepository.Get(x => x.OrderDate <= dateT && !x.IsDeleted && x.Author != null).ToList();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    var dateF = Convert.ToDateTime(dateFrom);
                    var dateT = Convert.ToDateTime(dateTo);
                    orderList =
                        orderList.Where(x => x.OrderDate >= dateF && x.OrderDate <= dateT && !x.IsDeleted).ToList();
                }
                else if (!string.IsNullOrEmpty(dateFrom))
                {
                    var dateF = Convert.ToDateTime(dateFrom);
                    orderList =
                       orderList.Where(x => x.OrderDate >= dateF && !x.IsDeleted).ToList();
                }
                else if (!string.IsNullOrEmpty(dateTo))
                {
                    var dateT = Convert.ToDateTime(dateTo);
                    orderList =
                        orderList.Where(x => x.OrderDate <= dateT && !x.IsDeleted).ToList();
                }
            }

            return orderList;
        }

        public static List<SearchEPOResultItem> ConvertToSearchEPOResult(List<Order> orderList)
        {
            var list = new List<SearchEPOResultItem>();
            foreach (var order in orderList)
            {
                list.Add(ConvertToSearchEPOResult(order));
            }
            return list;
        }

        public static SearchEPOResultItem ConvertToSearchEPOResult(Order order)
        {
            var item = new SearchEPOResultItem()
            {
                Id = order.Id.ToString(),
                OrderNumber = order.Status != null ? ((order.Status.Name == StatusEnum.Draft.ToString()) || (order.Status.Name == StatusEnum.Pending.ToString())) ? "-" : order.OrderNumber.ToString() : "-",
                TempNumber = order.TempOrderNumber.Substring(0,6),
                Status = order.Status != null ? order.Status.Name : string.Empty,
                Total = order.Total,
                Date = order.OrderDate,
                CostCentre = order.CostCentre != null ? order.CostCentre.Code.ToString() + " - " + order.CostCentre.Name : string.Empty,
                Supplier = oldDb.tblSuppliers.Where(x => x.SupplierID == order.SupplierId)
                        .Select(x => new { Name = x.SupplierName + " - " + x.SupplierCode }).Select(x => x.Name).FirstOrDefault(),
                Entity = order.Entity != null ? order.Entity.CodeNumber + " - " + order.Entity.Name : string.Empty,
                Author = order.Author != null ? order.Author.UserInfo.FirstName + " " + order.Author.UserInfo.LastName : string.Empty,
                isEditLocked =order.Status!=null && (order.Status.Name!=StatusEnum.Pending.ToString() && order.Status.Name!=StatusEnum.Draft.ToString())
            };
            item.OrderNumberInt =item.OrderNumber!="-" ? order.OrderNumber : 0;
            return item;
        }

       

#endregion


        public string GetSupplierFullNameByOldId(int oldId)
        {
            return oldDb.tblSuppliers.Where(x => x.SupplierID == oldId).Select(x => new { Name = x.SupplierName + " - " + x.SupplierCode }).Select(x => x.Name)
                .FirstOrDefault();
        }
        public string GetSupplierFullNameByCode(string code)
        {
            return oldDb.tblSuppliers.Where(x => x.SupplierCode == code).Select(x => new { Name = x.SupplierName + " - " + x.SupplierCode }).Select(x => x.Name)
                .FirstOrDefault();
        }

        private string GetSupplierCodeById(int supplierId)
        {
            return
                oldDb.tblSuppliers.Where(x => x.SupplierID == supplierId && x.Active)
                    .Select(x => x.SupplierCode)
                    .FirstOrDefault();
        }


       public void CancelOrder(int orderId)
        {
            var order = _orderRepository.Find(orderId);
            if (order != null)
            {
                Data.GenerateOrderLog(order,OrderLogSubject.EPO);
                var status = _statusRepository.Get(x => x.Name == StatusEnum.Cancelled.ToString()).FirstOrDefault();
                order.Status = status;
                //order.LastModifiedDate = DateTime.Now;
                //order.LastModifiedBy = _curUser;
                foreach (var item in order.OrderItems)
                {
                    item.Status = status;
                    item.LastModifiedDate = DateTime.Now;
                    item.LastModifiedBy = _curUser;
                }
                _orderRepository.Update(order);
                db.SaveChanges();
                GenerateAndSendCancelledOrder(order);
            }
        }

        public void GenerateAndSendCancelledOrder(Order order)
        {
            //var output = new Output(order);
            _output.Order = order;
            _output.CreatePDFContainer();
            var filePath = _output.CreatePDFFile(_output.PdfPOContainer, controllerContext);
            _output.SetCancelledStampOnPO(filePath);
            _output.SendEmailWithCancelledOrder(filePath);
        }

        public IEnumerable<ItemKitDdlViewModel> GetItemKitDdl(int accountId, int orderId)
        {
            var ccId = _orderRepository.Get(x=>x.Id==orderId).Select(x=>x.CostCentre.Id).FirstOrDefault();
            var list =
                _orderItemKitRepository.Get(x => !x.IsDeleted && x.Account.Id == accountId && x.CostCentre.Id == ccId)
                    .Select(x => new ItemKitDdlViewModel() { Id = x.Id, Name = x.Part })
                    .ToList();
            if(list.Count>0) return list;
            return null;
        }

        public void TestMethod()
        {
            var item = _orderItemRepository.Find(0);
            var order = _orderRepository.Find(3111);
            item = new OrderItem()
                       {
                           LineNumber = 1,
                           RevisionQty = 0,
                           Account = _accountRepository.Find(347),
                           SubAccount = _accountRepository.Find(0),
                           Currency = _currencyRepository.Find(1),
                           CurrencyRate = 1,
                           Description = "Test",
                           DueDate = DateTime.Now,
                           Qty = 1,
                           IsGSTInclusive = true,
                           IsTaxable = true,
                           Total = 967.5,
                           TotalExTax = 67.5,
                           TotalTax = 7.5,
                           UnitPrice = 12,
                           CreatedBy = _curUser,
                           DateCreated = DateTime.Now,
                           LastModifiedDate = DateTime.Now,
                           LastModifiedBy = _curUser,
                           Status = _statusRepository.Get(x => x.Id == 1).FirstOrDefault(),
                           Capex_Id = 0
                       };
            //_orderItemRepository.Add(item);
            _orderItemRepository.Add(item);
                order.OrderItems.Add(item);
                order.Total += 967.5;
                order.TotalExGST += 67.5;
                order.TotalGST += 7.5;

            order.LastModifiedBy = _curUser;
            order.LastModifiedDate = DateTime.Now;
            //_orderRepository.Update(order);
            db.SaveChanges();
        }

        public void CloseOrder(int orderId)
        {
            var order = _orderRepository.Get(x => x.Id == orderId).FirstOrDefault();
            Data.GenerateOrderLog(order,OrderLogSubject.EPO);
            if (order != null)
            {
                order.Status = Data.GetStatus(StatusEnum.Closed);
                _orderRepository.Update(order);
            }
            db.SaveChanges();
        }

        public string RenderPartialViewToString<T>(Controller controller, string viewName, T model)
        {
            controller.ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, null);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.ToString();
            }
        }

        public void SavaDefaultOrderSettings(DefaultOrderSettingsViewModel viewModel)
        {
            var user = _userRepository.Get(x => x.EmployeeId == Data.curEmpId).FirstOrDefault();
            if (user != null)
            {
                if (user.UserOrderSettings != null)
                {
                    user.UserOrderSettings.DefaultCostCentre = viewModel.DefaultCostCentreId!=0 ? Data.GetCostCentre(viewModel.DefaultCostCentreId) : user.UserOrderSettings.DefaultCostCentre;
                    user.UserOrderSettings.DefaultEntity = viewModel.DefaultEntityId!=0 ? Data.GetEntity(viewModel.DefaultEntityId) : user.UserOrderSettings.DefaultEntity;
                    user.UserOrderSettings.DefaultGroup = viewModel.DefaultGroupId!=0 ? Data.GetGroup(viewModel.DefaultGroupId) : user.UserOrderSettings.DefaultGroup;
                    user.UserOrderSettings.DefaultDeliveryAddress = viewModel.DefaultDeliveryAddressId!=0 ? Data.GetDeliveryAddress(viewModel.DefaultDeliveryAddressId) : user.UserOrderSettings.DefaultDeliveryAddress;
                    user.UserOrderSettings.DefaultSupplierId = viewModel.DefaultSupplierId != 0 ? viewModel.DefaultSupplierId : user.UserOrderSettings.DefaultSupplierId;
                }
                else
                {
                    user.UserOrderSettings = new UserOrderSettings()
                                                 {
                                                     DefaultCostCentre= Data.GetCostCentre(viewModel.DefaultCostCentreId),
                                                     DefaultEntity = Data.GetEntity(viewModel.DefaultEntityId),
                                                     DefaultGroup =Data.GetGroup(viewModel.DefaultGroupId),
                                                     DefaultDeliveryAddress = Data.GetDeliveryAddress(viewModel.DefaultDeliveryAddressId),
                                                     DefaultSupplierId = viewModel.DefaultSupplierId
                    };
                }
                user.UserOrderSettings.LastModifiedDate = DateTime.Now;
                user.UserOrderSettings.LastModifiedBy = _curUser;
                db.SaveChanges();
            }
        }

        public List<int> GetSubUsersEmployeeId(int curEmpId)
        {
            var curDate = DateTime.Today;
            var list = _substituteApproverRepository.Get(x => !x.IsDeleted && x.SubstitutionUser.EmployeeId == curEmpId && x.Start<=curDate && x.End>=curDate).Select(x=>x.ApproverUser.EmployeeId).ToList();
            return list;
        }

        
    }
}