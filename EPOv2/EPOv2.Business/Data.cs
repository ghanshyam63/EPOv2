namespace EPOv2.Business
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Reflection;
    using System.Web;

    using DomainModel.DataContext;
    using DomainModel.Entities;

    using EPOv2.Business.Interfaces;
    using EPOv2.Repositories.Interfaces;
    using EPOv2.ViewModels;

    using Intranet.ViewModels;

    using MdxClient;
    using Serilog;

    public partial class Data : IData
    {

        #region Repositories

        private readonly IDataContext _dataContext;

        private readonly IEntityRepository _entityRepository;

        private readonly ICostCentreRepository _costCentreRepository;

        private readonly IAccountRepository _accountRepository;

        private readonly IStateRepository _stateRepository;

        private readonly IDeliveryAddressRepository _deliveryAddressRepository;

        private readonly IUserRepository _userRepository;

        private readonly IGroupRepository _groupRepository;

        private readonly IGroupMemberRepository _groupMemberRepository;

        private readonly ICostCentreToEntityRepository _costCentreToEntityRepository;

        private readonly IAccountToCostCentreRepository _accountToCostCentreRepository;

        private readonly ICurrencyRepository _currencyRepository;

        private readonly IOrderRepository _orderRepository;

        private readonly IStatusRepository _statusRepository;

        private readonly ILevelRepository _levelRepository;

        private readonly ICapexRepository _capexRepository;

        private readonly ICapexApproverRepository _capexApproverRepository;

        private readonly IDivisionRepository _divisionRepository;

        private readonly IVoucherDocumentRepository _voucherDocumentRepository;

        private readonly IVoucherDocumentTypeRepository _voucherDocumentTypeRepository;

        private readonly IVoucherRepository _voucherRepository;

        private readonly IVoucherStatusRepository _voucherStatusRepository;

        private readonly ISubstituteApproverRepository _substituteApproverRepository;

        private readonly IApproverRepository _approverRepository;

        private readonly IRouteRepository _routeRepository;

        private readonly IRoleRepository _roleRepository;

        private readonly IOrderItemRepository _orderItemRepository;

        private readonly IOrderItemKitRepository _orderItemKitRepository;

        private readonly IUserOrderSettingsRepository _userOrderSettingsRepository;

        private readonly IUserInfoRepository _userInfoRepository;

        private readonly IMatchOrderRepository _matchOrderRepository;

        private readonly IVBasedataFinancialCalendarRepository _financialCalendarRepository;

        private readonly IVRockyEmployeesRepository _rockyEmployeesRepository;

        private readonly IDCalendarEventRepository _dCalendarEventRepository;

        private readonly IDCalendarEventTypeRepository _dCalendarEventTypeRepository;

        private readonly IVGLTrialBalanceWithBudgetRepository _glBalanceWithBudgetRepository;

        private readonly IAccountCategoryRepository _accountCategoryRepository;

        private readonly IAccountToCategoryRepository _accountToCategoryRepository;

        private readonly IOrderLogRepository _orderLogRepository;

        private readonly IVoucherRouteRepository _voucherRouteRepository;

        private readonly ICapexRouteRepository _capexRouteRepository;

        private readonly IDNewsRepository _dNewsRepository;

        private readonly IDTileRepository _dTileRepository;

        private readonly IDUserGroupRepository _dUserGroupRepository;

        private readonly IUserDashboardSettingsRepository _userDashboardSettingsRepository;

        private readonly IMock _mock;

        #endregion

        private readonly OldPurchaseOrderContext _oldDb = new OldPurchaseOrderContext();
        //private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ILogger _logger = Serilog.Log.Logger;
        private readonly QADLiveEntities _qadLive = new QADLiveEntities();

        private readonly OldTNAContext _oldTnaContext = new OldTNAContext();

        private SafetyManagementSystemEntities _safetyManagementSystem =new SafetyManagementSystemEntities();

        public IAd _ad { get; set; }

        public string capexFilesPath => @"\\dnas01\\EPO\Capex";

        private readonly DateTime _launchAppDate = Convert.ToDateTime("2015-06-28");

        private readonly string _curUser = HttpContext.Current?.User.Identity.Name.Replace("ONEHARVEST\\", "");

        private const string SystemUser = "system";

        public int curEmpId { get; set; }

        //public const string capexFilesPath = @"\\dnas01\\EPO\Capex";
        public enum TransmissionMethod
        {
            Email,

            Print
        }; //Fax

        public enum CapexType
        {
            Disposal,

            Purchase
        }

        public Data()
        {
            
        }
        public UserTilesForDashboard _userTilesForDashboard { get; set; }

        public Data(
            IDataContext dataContext,
            IEntityRepository entityRepository,
            ICostCentreRepository costCentreRepository,
            IAccountRepository accountRepository,
            IDeliveryAddressRepository deliveryAddressRepository,
            IStateRepository stateRepository,
            IUserRepository userRepository,
            IGroupRepository groupRepository,
            IGroupMemberRepository groupMemberRepository,
            ICostCentreToEntityRepository costCentreToEntityRepository,
            IAccountToCostCentreRepository accountToCostCentreRepository,
            ICurrencyRepository currencyRepository,
            IOrderRepository orderRepository,
            IStatusRepository statusRepository,
            ILevelRepository levelRepository,
            ICapexRepository capexRepository,
            ICapexApproverRepository capexApproverRepository,
            IDivisionRepository divisionRepository,
            IVoucherDocumentRepository voucherDocumentRepository,
            IVoucherDocumentTypeRepository voucherDocumentTypeRepository,
            IVoucherRepository voucherRepository,
            IVoucherStatusRepository voucherStatusRepository,
            ISubstituteApproverRepository substituteApproverRepository,
            IApproverRepository approverRepository,
            IRouteRepository routeRepository,
            IRoleRepository roleRepository,
            IOrderItemRepository orderItemRepository,
            IOrderItemKitRepository orderItemKitRepository,
            IUserOrderSettingsRepository userOrderSettingsRepository,
            IUserInfoRepository userInfoRepository,
            IMatchOrderRepository matchOrderRepository,
            IVBasedataFinancialCalendarRepository financialCalendarRepository,
            IVRockyEmployeesRepository rockyEmployeesRepository,
            IDCalendarEventRepository dCalendarEventRepository,
            IDCalendarEventTypeRepository dCalendarEventTypeRepository, 
            IVGLTrialBalanceWithBudgetRepository glBalanceWithBudgetRepository,
            IAccountCategoryRepository accountCategoryRepository,
            IAccountToCategoryRepository accountToCategoryRepository,
            IOrderLogRepository orderLogRepository, IVoucherRouteRepository voucherRouteRepository,
            ICapexRouteRepository capexRouteRepository, IDNewsRepository dNewsRepository, IDTileRepository dTileRepository, IDUserGroupRepository dUserGroupRepository, 
            IUserDashboardSettingsRepository userDashboardSettingsRepository,
            IMock mock,
            IAd ad)
        {
            _dataContext = dataContext;
            _entityRepository = entityRepository;
            _costCentreRepository = costCentreRepository;
            _accountRepository = accountRepository;
            _deliveryAddressRepository = deliveryAddressRepository;
            _stateRepository = stateRepository;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
            _costCentreToEntityRepository = costCentreToEntityRepository;
            _accountToCostCentreRepository = accountToCostCentreRepository;
            _currencyRepository = currencyRepository;
            this._orderRepository = orderRepository;
            this._statusRepository = statusRepository;
            this._levelRepository = levelRepository;
            this._capexRepository = capexRepository;
            this._capexApproverRepository = capexApproverRepository;
            this._divisionRepository = divisionRepository;
            this._voucherDocumentRepository = voucherDocumentRepository;
            this._voucherDocumentTypeRepository = voucherDocumentTypeRepository;
            this._voucherRepository = voucherRepository;
            this._voucherStatusRepository = voucherStatusRepository;
            this._substituteApproverRepository = substituteApproverRepository;
            this._approverRepository = approverRepository;
            this._routeRepository = routeRepository;
            this._roleRepository = roleRepository;
            this._orderItemRepository = orderItemRepository;
            this._orderItemKitRepository = orderItemKitRepository;
            this._userOrderSettingsRepository = userOrderSettingsRepository;
            this._userInfoRepository = userInfoRepository;
            this._matchOrderRepository = matchOrderRepository;
            this._financialCalendarRepository = financialCalendarRepository;
            this._rockyEmployeesRepository = rockyEmployeesRepository;
            this._dCalendarEventRepository = dCalendarEventRepository;
            this._dCalendarEventTypeRepository = dCalendarEventTypeRepository;
            this._glBalanceWithBudgetRepository = glBalanceWithBudgetRepository;
            this._accountCategoryRepository = accountCategoryRepository;
            this._accountToCategoryRepository = accountToCategoryRepository;
            this._orderLogRepository = orderLogRepository;
            _voucherRouteRepository = voucherRouteRepository;
            _capexRouteRepository = capexRouteRepository;
            _dNewsRepository = dNewsRepository;
            _dTileRepository = dTileRepository;
            _dUserGroupRepository = dUserGroupRepository;
            _userDashboardSettingsRepository = userDashboardSettingsRepository;
            _mock = mock;
            this._ad = ad;
            this.curEmpId = Convert.ToInt32(this._ad.GetADUserEmpIDbyUserName(this._curUser));
        }

        #region Fetch Data

        //TODO: Все фетч функции сделать с Update.
        public void FetchEntity()
        {
            var entityList = this._oldDb.tblEntities.Where(x => x.Active).ToList();

            foreach (var tblentity in entityList)
            {
                var entity = new Entity()
                                 {
                                     ABN = tblentity.ABN,
                                     ACN = tblentity.ACN,
                                     Code = tblentity.Code,
                                     CodeNumber = tblentity.EntityID,
                                     Name = tblentity.Name,
                                     Prefix = tblentity.Prefix,
                                     Phone = tblentity.APPhone,
                                     Fax = tblentity.APFax,
                                     Email = tblentity.APEmail,
                                     CreatedBy = this._curUser,
                                     DateCreated = DateTime.Now,
                                     LastModifiedBy = this._curUser,
                                     LastModifiedDate = DateTime.Now
                                 };
                this._entityRepository.Add(entity);
            }
            this._dataContext.SaveChanges();
        }

        public void FetchCC()
        {
            var ccList = this._oldDb.tblCostCentres.Where(x => x.Active).ToList();
            foreach (var costCentre in ccList)
            {
                var cc = new CostCentre()
                             {
                                 Code = costCentre.CostCentreCode,
                                 Name = costCentre.Description,
                                 CreatedBy = this._curUser,
                                 LastModifiedBy = this._curUser,
                                 DateCreated = DateTime.Now,
                                 LastModifiedDate = DateTime.Now
                             };
                this._costCentreRepository.Add(cc);
            }
            this._dataContext.SaveChanges();
        }

        /// <summary>
        /// Fetch Accounts & SubAccounts
        /// </summary>
        public void FetchAccount()
        {
            var accList = this._oldDb.tblAccounts.Where(x => x.Active).ToList();
            var subaccList = this._oldDb.tblSubAccounts.Where(x => x.Active).ToList();

            foreach (var acc in accList)
            {
                var account = new Account()
                                  {
                                      Code = acc.AccountCode,
                                      Name = acc.Description,
                                      CreatedBy = this._curUser,
                                      DateCreated = DateTime.Now,
                                      LastModifiedBy = this._curUser,
                                      LastModifiedDate = DateTime.Now,
                                      Type = 0
                                  };
                this._accountRepository.Add(account);
            }
            foreach (var subacc in subaccList)
            {
                var subAccount = new Account()
                                     {
                                         Name = subacc.Description,
                                         Code = subacc.SubAccountCode,
                                         Type = 1,
                                         DateCreated = DateTime.Now,
                                         LastModifiedDate = DateTime.Now,
                                         CreatedBy = this._curUser,
                                         LastModifiedBy = this._curUser
                                     };
                this._accountRepository.Add(subAccount);
            }
            this._dataContext.SaveChanges();
        }

        public void FetchDeliveryAddress()
        {
            var daList = this._oldDb.tblPODeliveries.Where(x => x.Active).ToList();
            foreach (var poDelivery in daList)
            {
                var state =
                    this._stateRepository.Get(x => x.ShortName.ToUpper().Contains(poDelivery.State.ToUpper()))
                        .FirstOrDefault();
                var da = new DeliveryAddress()
                             {
                                 Address = poDelivery.Address1 + " " + poDelivery.Address2,
                                 Name = poDelivery.DeliveryName,
                                 PostCode = Convert.ToInt32(poDelivery.PostCode),
                                 City = poDelivery.City,
                                 State = state,
                                 CreatedBy = this._curUser,
                                 LastModifiedBy = this._curUser,
                                 DateCreated = DateTime.Now,
                                 LastModifiedDate = DateTime.Now
                             };
                this._deliveryAddressRepository.Add(da);
            }
            this._dataContext.SaveChanges();
        }

        public void FetchReceiptGroup()
        {
            var oldGroups = this._oldDb.tblGroups.Where(x => x.Active == true).ToList();
            foreach (var oldGroup in oldGroups)
            {
                var group = new Group()
                                {
                                    Name = oldGroup.GroupName,
                                    CreatedBy = this._curUser,
                                    LastModifiedBy = this._curUser,
                                    DateCreated = DateTime.Now,
                                    LastModifiedDate = DateTime.Now
                                };
                this._groupRepository.Add(group);
            }
            this._dataContext.SaveChanges();

            var oldGrMems = this._oldDb.tblGroupMembers.Where(x => x.Active == true).ToList();
            foreach (var oldGrMem in oldGrMems)
            {
                var oldGroup = this._oldDb.tblGroups.Find(oldGrMem.GroupID);
                var group = this._groupRepository.Get(x => x.Name == oldGroup.GroupName).FirstOrDefault();
                var user =
                    this._userRepository.Get(x => x.UserName == oldGrMem.MemberID.Replace("oneharvest\\", ""))
                        .FirstOrDefault();
                if (user == null || group == null)
                {
                    continue;
                }
                var member = new GroupMember()
                                 {
                                     Group = @group,
                                     User = user,
                                     CreatedBy = this._curUser,
                                     LastModifiedBy = this._curUser,
                                     DateCreated = DateTime.Now,
                                     LastModifiedDate = DateTime.Now
                                 };
                this._groupMemberRepository.Add(member);
            }
            this._dataContext.SaveChanges();
        }

        public void FetchCcToEntity()
        {
            var entccList =
                this._oldDb.tblEntityCCAccs.Where(x => x.Active)
                    .GroupBy(x => new { x.EntityID, x.CostCentreID })
                    .Select(x => new { x.Key.CostCentreID, x.Key.EntityID })
                    .ToList();
            var oldccList = this.GetOldCostCentres();
            // var oldentityList = this.GetOldEntities();
            var ccList = this.GetCostCentres();
            //var entityList = this.GetEntities();
            foreach (var entCc in entccList)
            {
                var oldcc = oldccList.FirstOrDefault(x => x.CostCentreID == entCc.CostCentreID);
                // var oldent = oldentityList.FirstOrDefault(x => x.EntityID == entCc.EntityID);
                var cc = ccList.FirstOrDefault(x => x.Code == oldcc.CostCentreCode);
                var entity = this._entityRepository.Get(x => x.CodeNumber == entCc.EntityID).FirstOrDefault();
                var ccToEnt = new CostCentreToEntity()
                                  {
                                      CostCentre = cc,
                                      Entity = entity,
                                      CreatedBy = this._curUser,
                                      DateCreated = DateTime.Now,
                                      LastModifiedDate = DateTime.Now,
                                      LastModifiedBy = this._curUser
                                  };
                this._costCentreToEntityRepository.Add(ccToEnt);
            }
            this._dataContext.SaveChanges();
        }

        public void FetchAccountToCc()
        {
            var entccList =
                this._oldDb.tblEntityCCAccs.Where(x => x.Active)
                    .GroupBy(x => new { x.AccountID, x.CostCentreID })
                    .Select(x => new { x.Key.CostCentreID, x.Key.AccountID })
                    .ToList();

            var oldccList = this.GetOldCostCentres();
            var oldaccList = this.GetOldAccounts();
            var ccList = this.GetCostCentres();
            var accList = this.GetAccounts();
            foreach (var entCc in entccList)
            {
                var oldcc = oldccList.FirstOrDefault(x => x.CostCentreID == entCc.CostCentreID);
                var oldAcc = oldaccList.FirstOrDefault(x => x.AccountID == entCc.AccountID);
                if (oldAcc == null || oldcc == null)
                {
                    continue;
                }
                var cc = ccList.FirstOrDefault(x => x.Code == oldcc.CostCentreCode);
                var account = accList.FirstOrDefault(x => x.Code == oldAcc.AccountCode && x.Type == 0);
                var accToCc = new AccountToCostCentre()
                                  {
                                      CostCentre = cc,
                                      Account = account,
                                      CreatedBy = this._curUser,
                                      DateCreated = DateTime.Now,
                                      LastModifiedDate = DateTime.Now,
                                      LastModifiedBy = this._curUser
                                  };
                this._accountToCostCentreRepository.Add(accToCc);
            }
            this._dataContext.SaveChanges();
        }

        public void FetchSubAccountToCc()
        {
            var entccList =
                this._oldDb.tblEntityCCAccs.Where(x => x.Active)
                    .GroupBy(x => new { x.SubAccountID, x.CostCentreID })
                    .Select(x => new { x.Key.CostCentreID, x.Key.SubAccountID })
                    .ToList();

            var oldccList = this.GetOldCostCentres();
            var oldsaccList = this.GetOldSubAccounts();
            var ccList = this.GetCostCentres();
            var accList = this.GetAccounts();
            foreach (var entCc in entccList)
            {
                var oldcc = oldccList.FirstOrDefault(x => x.CostCentreID == entCc.CostCentreID);
                var oldAcc = oldsaccList.FirstOrDefault(x => x.SubAccountID == entCc.SubAccountID);
                if (oldAcc == null || oldcc == null)
                {
                    continue;
                }
                var cc = ccList.FirstOrDefault(x => x.Code == oldcc.CostCentreCode);
                var account = accList.FirstOrDefault(x => x.Code == oldAcc.SubAccountCode && x.Type == 1);
                var accToCc = new AccountToCostCentre()
                                  {
                                      CostCentre = cc,
                                      Account = account,
                                      CreatedBy = this._curUser,
                                      DateCreated = DateTime.Now,
                                      LastModifiedDate = DateTime.Now,
                                      LastModifiedBy = this._curUser
                                  };
                this._accountToCostCentreRepository.Add(accToCc);
            }
            this._dataContext.SaveChanges();
        }

        #endregion

        #region Save SMTH

        public void SaveState(State model)
        {
            model.CreatedBy = this._curUser;
            model.DateCreated = DateTime.Now;
            model.LastModifiedBy = this._curUser;
            model.LastModifiedDate = DateTime.Now;
            this._stateRepository.Add(model);
            this._dataContext.SaveChanges();
        }



        public void SaveCurrency(Currency model)
        {
            model.CreatedBy = this._curUser;
            model.DateCreated = DateTime.Now;
            model.LastModifiedBy = this._curUser;
            model.LastModifiedDate = DateTime.Now;
            this._currencyRepository.Add(model);
            this._dataContext.SaveChanges();
        }

        #endregion

        #region GET SMTH

        public List<State> GetStates()
        {
            var states = this._stateRepository.Get().ToList();
            return states;
        }

        public List<Currency> GetCurrencies()
        {
            var list = this._currencyRepository.Get().ToList();
            return list;
        }

        /// <summary>
        /// Get Active Entities. Sorted by CodeNumber
        /// </summary>
        /// <returns></returns>
        public List<Entity> GetEntities()
        {
            var listEntities = this._entityRepository.Get(x => !x.IsDeleted).OrderBy(x => x.CodeNumber).ToList();
            return listEntities;
        }

        public List<CostCentre> GetCostCentres()
        {
            var listCC = this._costCentreRepository.Get(x => !x.IsDeleted).OrderBy(x => x.Code).ToList();
            return listCC;
        }

        public List<CostCentre> GetAllCostCentres()
        {
            var listCC = this._costCentreRepository.Get().OrderBy(x => x.Code).ToList();
            return listCC;
        }

        public List<CostCentreViewModel> GetCostCentresVM(bool isFullList = false)
        {
            var modelList = new List<CostCentreViewModel>();
            var listCC = _costCentreRepository.Get();
            if (!isFullList) listCC = listCC.Where(x => !x.IsDeleted);
            foreach (var cc in listCC.ToList())
            {
                var ccVM = new CostCentreViewModel()
                               {
                                   Id = cc.Id,
                                   Code = cc.Code,
                                   Name = cc.Name,
                                   FullName = cc.Code + " - " + cc.Name
                               };
                modelList.Add(ccVM);
            }
            return modelList.OrderBy(x => x.Code).ToList();
        }

        public List<CostCentreViewModel> GetCostCentresWithOwnerVM(bool isFullList = false)
        {
            var modelList = new List<CostCentreViewModel>();
            var listCC = _costCentreRepository.Get();
            if (!isFullList) listCC = listCC.Where(x => !x.IsDeleted);
            foreach (var cc in listCC.ToList())
            {
                if (cc.Owner != null)
                {
                    if (cc.Owner.UserInfo.IsDeleted)
                    {
                        var ccVM = new CostCentreViewModel()
                        {
                            Id = cc.Id,
                            Code = cc.Code,
                            Name = cc.Name,
                            FullName = cc.GetFullName(),
                            Owner = "No Owner (was: "+cc.Owner.GetFullName()+")",
                            isOwnerFree = true
                        };
                        modelList.Add(ccVM);
                    }
                    else
                    {
                        var rfModel =
                             this._rockyEmployeesRepository.Get(x => x.EmpNo == cc.Owner.EmployeeId.ToString() && x.Active == 1)
                                 .FirstOrDefault();
                        if (rfModel == null)
                        {
                            var ccVM = new CostCentreViewModel()
                                           {
                                               Id = cc.Id,
                                               Code = cc.Code,
                                               Name = cc.Name,
                                               FullName = cc.GetFullName(),
                                               isOwnerFree = true,
                                               Owner =
                                                   "Deactivated or Deleted in a RockFast (was: " + cc.Owner.GetFullName() + ")"
                                           };
                            modelList.Add(ccVM);
                        }
                        else
                        {
                            var ccVM = new CostCentreViewModel()
                            {
                                Id = cc.Id,
                                Code = cc.Code,
                                Name = cc.Name,
                                FullName = cc.GetFullName(),
                                Owner =cc.Owner.GetFullName()
                            };
                            modelList.Add(ccVM);
                        }
                    }
                    
                }
                else
                {
                    var ccVM = new CostCentreViewModel()
                    {
                        Id = cc.Id,
                        Code = cc.Code,
                        Name = cc.Name,
                        FullName = cc.GetFullName(),
                        isOwnerFree = true,
                        Owner = "No Owner!"
                    };
                    modelList.Add(ccVM);
                }
            }
            return modelList.OrderBy(x => x.Code).ToList();
        }

        public List<CostCentreViewModel> GetCostCentresVM(int entityId)
        {
            var listCC =
                this._costCentreToEntityRepository.Get(x => x.Entity.Id == entityId)
                    .Select(
                        x =>
                        new CostCentreViewModel()
                            {
                                Id = x.CostCentre.Id,
                                Code = x.CostCentre.Code,
                                Name = x.CostCentre.Name,
                                FullName = x.CostCentre.Code + " - " + x.CostCentre.Name
                            })
                    .OrderBy(x => x.Code)
                    .ToList();
            return listCC;
        }

        public List<Account> GetAccounts()
        {
            var list = this._accountRepository.Get(x => !x.IsDeleted).OrderBy(x => x.Code).ToList();
            return list;
        }

        public List<Account> GetAccountsAll()
        {
            var list = this._accountRepository.Get().OrderBy(x => x.Code).ToList();
            return list;
        }

        public List<Account> GetCCAccountsByType(int ccId, int type = 0)
        {
            var list =
                this._accountToCostCentreRepository.Get(x => x.CostCentre.Id == ccId && x.Account.Type == type && x.IsDeleted==false)
                    .Select(x => x.Account)
                    .OrderBy(x => x.Code)
                    .ToList();
            return list;
        }

        public Order GetOrder(int orderId)
        {
            return this._orderRepository.Find(orderId);
        }

        public List<tblCostCentre> GetOldCostCentres()
        {
            var list = this._oldDb.tblCostCentres.Where(x => x.Active).ToList();
            return list;
        }

        public List<tblEntity> GetOldEntities()
        {
            var list = this._oldDb.tblEntities.Where(x => x.Active).ToList();
            return list;
        }

        public List<tblAccount> GetOldAccounts()
        {
            var list = this._oldDb.tblAccounts.Where(x => x.Active).ToList();
            return list;
        }

        public List<tblSubAccount> GetOldSubAccounts()
        {
            var list = this._oldDb.tblSubAccounts.Where(x => x.Active).ToList();
            return list;
        }

        public List<DeliveryAddress> GetDeliveryAddresses()
        {
            var list =
                this._deliveryAddressRepository.Get(x => !x.IsDeleted)
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Address)
                    .ToList();
            return list;
        }

        public List<tblSupplier> GetSuppliers()
        {
            var list = this._oldDb.tblSuppliers.Where(x => x.Active).OrderBy(x => x.SupplierName).ToList();
            return list;
        }

        public List<tblSupplier> GetSuppliers(int entityCode)
        {
            var list =
                this._oldDb.tblSuppliers.Where(x => x.Active && x.Entity == entityCode)
                    .OrderBy(x => x.SupplierName)
                    .ToList();
            return list;
        }

        public tblSupplier GetSupplier(int supplierId)
        {
            return this._oldDb.tblSuppliers.FirstOrDefault(x => x.SupplierID == supplierId);
        }

        public List<Group> GetGroups()
        {
            var list = this._groupRepository.Get(x => !x.IsDeleted).OrderBy(x => x.Name).ToList();
            return list;
        }

        public List<Group> GetGroups_all()
        {
            var list = this._groupRepository.Get().OrderBy(x => x.Name).ToList();
            return list;
        }

        #endregion

        public List<Status> GetStatuses()
        {
            var modellist = this._statusRepository.Get().ToList();
            return modellist;
        }

        public void SaveStatus(Status model)
        {
            model.CreatedBy = this._curUser;
            model.DateCreated = DateTime.Now;
            model.LastModifiedBy = this._curUser;
            model.LastModifiedDate = DateTime.Now;
            this._statusRepository.Add(model);
            this._dataContext.SaveChanges();
        }

        public List<Level> GetLevels()
        {
            var list = this._levelRepository.Get().OrderBy(x => x.Code).ToList();
            return list;
        }

        public void SaveLevel(Level model)
        {
            model.CreatedBy = this._curUser;
            model.LastModifiedBy = this._curUser;
            model.DateCreated = DateTime.Now;
            model.LastModifiedDate = DateTime.Now;
            this._levelRepository.Add(model);
            this._dataContext.SaveChanges();
        }

        public void FetchCCOwners()
        {
            var listCC = this._costCentreRepository.Get().ToList();
            foreach (var cc in listCC)
            {
                var ccOld = this._oldDb.tblStagingCostCentresDEVs.FirstOrDefault(x => x.CostCentreCode == cc.Code);
                if (ccOld != null)
                {
                    var owner =
                        this._userRepository.Get(
                            x =>
                            x.UserInfo.FirstName.ToUpper() + " " + x.UserInfo.LastName.ToUpper()
                            == ccOld.Owner.ToUpper()).FirstOrDefault();

                    //this._costCentreRepository..Entry(cc).State = EntityState.Modified;
                    cc.Owner = owner;
                    cc.LastModifiedBy = this._curUser;
                    cc.LastModifiedDate = DateTime.Now;
                    this._dataContext.SaveChanges();
                }
            }

        }

        public List<tblCapex> GetCapexes()
        {
            var list = this._oldDb.tblCapexes.Where(x => x.Active && x.StatusID == 2).ToList();
                //status 2 means, Capex is Approved
            return list;
        }

        public List<tblCapex> GetFullListCapexesFromOldSystem()
        {
            var minDate = Convert.ToDateTime("2014-07-01");//1415 financial year
            var list = this._oldDb.tblCapexes.Where(x => x.Active && (x.StatusID == 2 || x.StatusID == 6) && x.Created>minDate).ToList();
            //status 2 -Approved, 6 - Closed
            return list;
        }

        public List<Capex> GetApprovedCapexes()
        {
            var list =
                this._capexRepository.Get(x => !x.IsDeleted && x.Status.Name == StatusEnum.Approved.ToString()).ToList();
                //status 2 means, Capex is Approved
            return list;
        }

        public List<Capex> GetAllCapexes()
        {
            var list =
                this._capexRepository.Get(x => !x.IsDeleted).ToList();
            return list;
        }

        public CostCentre GetCostCentreForCapex(int capexId)
        {
            var ccCode = _oldDb.tblCapexes.Where(x => x.CapexID == capexId).Select(x => x.CostCentreID).FirstOrDefault();
            if (ccCode == null)
            {
                var costCentre = _capexRepository.Get(x => x.Id == capexId).Select(x => x.CostCentre).FirstOrDefault();
                //cc = this.db.CostCentres.FirstOrDefault(x => x.Code == ccCode);
                return costCentre;
            }

            var cc = this._costCentreRepository.Get(x => x.Code == ccCode).FirstOrDefault();
            return cc;
        }

        public List<CapexViewModel> GetCapexViewModels(bool isEmpty, bool fullList=false)
        {
            var list = new List<CapexViewModel>();

            var capexList = fullList? GetFullListCapexesFromOldSystem() : GetCapexes();
            foreach (var tblCapex in capexList)
            {
                var item = new CapexViewModel()
                               {
                                   Id = tblCapex.CapexID,
                                   Description = tblCapex.Description,
                                   Number = tblCapex.CapexNumber,
                                   Title = tblCapex.Title,
                                   FullName = tblCapex.CapexNumber + " - " + tblCapex.Title
                               };
                list.Add(item);
            }

            var capexes =fullList ? GetAllCapexes() : GetApprovedCapexes();
            foreach (var capex in capexes)
            {
                var item = new CapexViewModel()
                               {
                                   Id = capex.Id,
                                   Description = capex.Description,
                                   Number = capex.CapexNumber,
                                   Title = capex.Title,
                                   FullName = capex.CapexNumber + " - " + capex.Title
                               };
                list.Add(item);
            }
            return list.OrderByDescending(x => x.Number).ToList();
        }

        public void FillAuthorId()
        {
            var list = this._orderRepository.Get().ToList();
            foreach (var order in list)
            {
                //this.db.Entry(order).State=EntityState.Modified;
                order.Author = order.User;
            }
            this._dataContext.SaveChanges();
        }

        public string GetCostCentreOwner(int ccId)
        {
            var cc = this._costCentreRepository.Find(ccId);
            return cc.Owner.Id;
        }

        public CostCentreOwnerViewModel GetCostCentreOwnerVM()
        {
            //var main = new Main(db);
            var ownerList = GetUserViewModels();

            var ccVMList = this.GetCostCentresVM();

            var model = new CostCentreOwnerViewModel() { CostCentres = ccVMList, Owners = ownerList };
            return model;
        }

        public bool SaveCostCentreOwner(CostCentreOwnerViewModel model)
        {
            try
            {
                var cc = this._costCentreRepository.Find(Convert.ToInt32(model.SelectedCostCenterId));
                var user = this._userRepository.Find(model.SelectedOwnerId);
                //this.db.Entry(cc).State=EntityState.Modified;
                cc.Owner = user;
                cc.LastModifiedDate = DateTime.Now;
                cc.LastModifiedBy = this._curUser;
                this._dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.Error(e, $"SaveCostCentreOwner({model.SelectedCostCenterId})");
                return false;
            }
            return true;
        }

        public string GetUserFullName(string id)
        {
            return
                this._userRepository.Get(x => x.Id == id)
                    .Select(x => new { FullName = x.UserInfo.FirstName + " " + x.UserInfo.LastName })
                    .Select(x => x.FullName)
                    .FirstOrDefault();
        }

        private CapexDetailBox GetCapexDetailBox()
        {
            var model = new CapexDetailBox();
            return model;
        }

        public Status GetStatus(StatusEnum status)
        {
            switch (status)
            {
                case StatusEnum.Draft:
                    return this._statusRepository.Get(x => x.Name == status.ToString()).First();
                case StatusEnum.Pending:
                    return this._statusRepository.Get(x => x.Name == status.ToString()).First();
                case StatusEnum.Approved:
                    return this._statusRepository.Get(x => x.Name == status.ToString()).First();
                case StatusEnum.Authorised:
                    return this._statusRepository.Get(x => x.Name == status.ToString()).First();
                case StatusEnum.Declined:
                    return this._statusRepository.Get(x => x.Name == status.ToString()).First();
                case StatusEnum.Revised:
                    return this._statusRepository.Get(x => x.Name == status.ToString()).First();
                case StatusEnum.Transmitted:
                    return this._statusRepository.Get(x => x.Name == status.ToString()).First();
                case StatusEnum.Receipt_in_Full:
                    break;
                case StatusEnum.Receipt_Partial:
                    break;
                case StatusEnum.Invoiced:
                    break;
                case StatusEnum.Cancelled:
                    break;
                case StatusEnum.Closed:
                    return this._statusRepository.Get(x => x.Name == status.ToString()).First();
                default:
                    return null;
            }
            return null;
        }

        

        public string GetStatusName(StatusEnum status)
        {
            switch (status)
            {
                case StatusEnum.Draft:
                    return status.ToString();
                case StatusEnum.Pending:
                    return status.ToString();
                case StatusEnum.Approved:
                    return status.ToString();
                case StatusEnum.Authorised:
                    return status.ToString();
                case StatusEnum.Declined:
                    return status.ToString();
                case StatusEnum.Revised:
                    return status.ToString();
                case StatusEnum.Transmitted:
                    return status.ToString();
                case StatusEnum.Receipt_in_Full:
                    return status.ToString().Replace("_", " ");
                case StatusEnum.Receipt_Partial:
                    return status.ToString().Replace("_", " ");
                case StatusEnum.Invoiced:
                    return status.ToString();
                case StatusEnum.Cancelled:
                    return status.ToString();
                case StatusEnum.Closed:
                    return status.ToString();
                default:
                    return string.Empty;
            }
            //return null;
        }

        public List<MatchOrder> GetMatchingList(Order order)
        {
            var list = _matchOrderRepository.Get(x => !x.IsDeleted && x.Order.Id == order.Id).ToList();
            return list;
        }



        public string GetAuthoriser(int orderId)
        {
            var approverList = this._routeRepository.Get(x => x.Order.Id == orderId).ToList();
            var user =
                approverList.Where(x => x.Number == approverList.Max(y => y.Number))
                    .Select(x => x.Approver.User)
                    .FirstOrDefault();
            if (user != null)
            {
                return user.UserInfo.FirstName + " " + user.UserInfo.LastName;
            }
            return string.Empty;
        }

        public List<UserViewModel> GetUserViewModels() //it's duplication, but have no idea how to make in another way
        {
            //TODO: needs to add ROLE list
            var list = new List<UserViewModel>();
            var users = _userRepository.Get().Include(x => x.UserInfo).Include(x => x.UserOrderSettings).ToList();
                //Ad.GetAllUsers();
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
            return list.OrderBy(x => x.FullName).ToList();
        }

        public DataTable MDxQueryRun(string connectionString, string query)
        {
            var dataTable = new DataTable();
            var connection = new MdxConnection { ConnectionString = connectionString };
            using (connection)
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    using (var dataAdapter = new MdxDataAdapter())
                    {
                        dataAdapter.SelectCommand = command;
                        dataAdapter.Fill(dataTable);
                    }
                }
            }
            return dataTable;
        }

        public string GenerateMDXQueryCasefillByDay(DateTime date)
        {
            var dtStr = String.Format("{0:s}", date);
            string query = @"select CROSSJOIN ({[Sites].[Site].[Site].[BDL], [Sites].[Site].[Site].[BNE], [Sites].[Site].[Site].[PTH], 
            [Sites].[Site].[Site].[SYD], [Sites].[Site].[All]}, {[Measures].[Casefill], [Measures].[Casefill KPI Status]} ) on columns,
            CROSSJOIN ({[Customers].[Partner Code].&[AL], [Customers].[Partner Code].&[CC], [Customers].[Partner Code].&[CO], [Customers].[Partner Code].&[EX], 
            [Customers].[Partner Code].&[IG], [Customers].[Partner Code].&[MD], [Customers].[Partner Code].&[MK], [Customers].[Partner Code].&[TD], 
            [Customers].[Partner Code].&[WW], [Customers].[Partner Code].[All]}, 
            {[Customers].[Partner Sub Code].[All], [Customers].[Partner Sub Code].&[Deli], [Customers].[Partner Sub Code].&[Produce]}) on rows 
            from [Casefill]  
            WHERE StrToMember('[Financial Calendar].[Date].&["+ dtStr +"]')";
            return query;
        }

        public string GenerateMDXQueryCasefillTrend(DateTime dateFrom, DateTime dateTo)
        {
            var date = DateTime.Today;
            var daysCnt = Math.Abs((dateTo - dateFrom).TotalDays);
            var dayQueryParam = "";
            string query = @"select CROSSJOIN ({";
            for (int i = 0; i <= daysCnt-1; i++)
            {
                var dt = String.Format("{0:s}", date.AddDays(i*-1));
                dayQueryParam = @"[Financial Calendar].[Date].&["+ dt +"]";
                query += dayQueryParam;
                if (i != daysCnt - 1) query += ",";
            }
            query += @"},{[Measures].[Casefill]}) on columns, 
              {[Sites].[Site].[Site].&[BDL], [Sites].[Site].[Site].&[BNE], [Sites].[Site].[Site].&[PTH], [Sites].[Site].[Site].&[SYD], [Sites].[Site].[All]} on rows 
              from [Casefill]";
            return query;
        }

        public string GenerateMDXQueryCasefillDetail(DateTime date)
        {
            var dt = String.Format("{0:s}",date);
            string query = @"WITH MEMBER Shorts AS IIF( [Measures].[Short Cartons] <> 0, [Measures].[Short Cartons], NULL) 
               MEMBER OrdQty AS IIF( [Measures].[Short Cartons] <> 0, [Measures].[Ordered Cartons], NULL)
               MEMBER InvQty AS IIF( [Measures].[Short Cartons] <> 0, [Measures].[Invoice Cartons], NULL)
               MEMBER ShortTot AS IIF( [Measures].[Short Cartons] <> 0, [Measures].[Short Total], NULL) 
               SELECT {[OrdQty], [InvQty], [Shorts], [ShortTot]} ON COLUMNS, 
               NON EMPTY CROSSJOIN({[Sites].[Site].&[BDL],[Sites].[Site].&[BNE],[Sites].[Site].&[PTH],[Sites].[Site].&[SYD]}, 
               [Customers].[Customer Code].[Customer Code], [Customers].[Customer Name].[Customer Name], 
               [SalesOrderNumber_CF].[Sales Order Number].[Sales Order Number], 
               [PartMaster].[Item].[Item], [PartMaster].[Description].[Description]) on rows 
              from [Casefill] WHERE [Financial Calendar].[Date].&[" + dt +"]";
            return query;
        }

        public void GenerateOrderLog(int orderId, OrderLogSubject subject = OrderLogSubject.Unknown)
        {
            var order = this._orderRepository.Find(orderId);
            GenerateOrderLog(order, subject);
        }

        public void GenerateOrderLog(Order order, OrderLogSubject subject = OrderLogSubject.Unknown)
        {
            try
            {
                var orderLog = new OrderLog()
                                   {
                                       Subject = subject.ToString(),
                                       LatestOrder = order,
                                       Status = order.Status,
                                       OrderCreatedBy = order.CreatedBy,
                                       OrderDateCreated = order.DateCreated,
                                       OrderLastModifiedBy = order.LastModifiedBy,
                                       OrderLastModifiedDate = order.LastModifiedDate
                                   };
                _orderLogRepository.Add(orderLog);
                _dataContext.SaveChanges();
            }
            catch (Exception e)
            {
                Log.Error("Data.GenerateOrderLog(orderId:"+order.Id+")", e);
            }
        }

        public Status GetPreviousOrderStatus(Order order)
        {
            return GetPreviousOrderStatus(order.Id);
        }

        public Status GetPreviousOrderStatus(int orderId)
        {
            var list = _orderLogRepository.Get(x => x.LatestOrder.Id == orderId && !x.IsDeleted).Include(x=>x.Status).ToList();
            return list.Count>0 ? list.OrderByDescending(x => x.OrderLastModifiedDate).Select(x => x.Status).First() : null;
        }
    }
}