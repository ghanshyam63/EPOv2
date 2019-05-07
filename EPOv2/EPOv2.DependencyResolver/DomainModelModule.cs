using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOv2.DependencyResolver
{
    using System.Data.Entity;
    using System.Data.Entity.Core.Common.CommandTrees;

    using DomainModel.DataContext;

    using EPOv2.Business;
    using EPOv2.Business.Interfaces;
    using EPOv2.Repositories.Interfaces;

    using global::Repositories;

    using Interfaces;

    using Ninject.Modules;
    using Ninject.Web.Common;

    public class DomainModelModule : NinjectModule
    {
        public override void Load()
        {
           // Bind<IDbContextManager>().To<DbContextManager<DbContext>>();
            Bind<IDataContext>().To<PurchaseOrderContext>().InRequestScope();

            Bind<IAccountRepository>().To<AccountRepository>();
            Bind<IAccountCategoryRepository>().To<AccountCategoryRepository>();
            Bind<IAccountToCostCentreRepository>().To<AccountToCostCentreRepository>();
            Bind<IAccountToCategoryRepository>().To<AccountToCategoryRepository>();
            Bind<IApproverRepository>().To<ApproverRepository>();

            Bind<ICapexRepository>().To<CapexRepository>();
            Bind<ICapexApproverRepository>().To<CapexApproverRepository>();
            Bind<ICapexRouteRepository>().To<CapexRouteRepository>();
            Bind<ICostCentreRepository>().To<CostCentreRepository>();
            Bind<ICostCentreToEntityRepository>().To<CostCentreToEntityRepository>();
            Bind<ICurrencyRepository>().To<CurrencyRepository>();

            Bind<IDeliveryAddressRepository>().To<DeliveryAddressRepository>();
            Bind<IDivisionRepository>().To<DivisionRepository>();

            Bind<IEntityRepository>().To<EntityRepository>();

            Bind<IGroupRepository>().To<GroupRepository>();
            Bind<IGroupMemberRepository>().To<GroupMemberRepository>();

            Bind<ILevelRepository>().To<LevelRepository>();

            Bind<IMatchOrderRepository>().To<MatchOrderRepository>();

            Bind<IOrderRepository>().To<OrderRepository>();
            Bind<IOrderLogRepository>().To<OrderLogRepository>();
            Bind<IOrderItemKitRepository>().To<OrderItemKitRepository>();
            Bind<IOrderItemLogRepository>().To<OrderItemLogRepository>();
            Bind<IOrderItemRepository>().To<OrderItemRepository>();

            Bind<IRoleRepository>().To<RoleRepository>();
            Bind<IRouteRepository>().To<RouteRepository>();

            Bind<IStatusRepository>().To<StatusRepository>();
            Bind<IStateRepository>().To<StateRepository>();
            Bind<ISubstituteApproverRepository>().To<SubstituteApproverRepository>();

            Bind<IUserInfoRepository>().To<UserInfoRepository>();
            Bind<IUserRepository>().To<UserRepository>();
            Bind<IUserOrderSettingsRepository>().To<UserOrderSettingsRepository>();

            Bind<IVoucherRepository>().To<VoucherRepository>();
            Bind<IVoucherDocumentRepository>().To<VoucherDocumentRepository>();
            Bind<IVoucherDocumentTypeRepository>().To<VoucherDocumentTypeRepository>();
            Bind<IVoucherRouteRepository>().To<VoucherRouteRepository>();
            Bind<IVoucherStatusRepository>().To<VoucherStatusRepository>();

            Bind<IVBasedataFinancialCalendarRepository>().To<vBasedataFinancialCalendarRepository>();
            Bind<IVRockyEmployeesRepository>().To<vRockyEmployeesRepository>();
            Bind<IDCalendarEventRepository>().To<DCalendarEventRepository>();
            Bind<IDCalendarEventTypeRepository>().To<DCalendarEventTypeRepository>();
            Bind<IDNewsRepository>().To<DNewsRepository>();
            Bind<IDTileRepository>().To<DTileRepository>();
            Bind<IUserDashboardSettingsRepository>().To<UserDashboardSettingsRepository>();
            Bind<IDUserGroupRepository>().To<DUserGroupRepository>();


            Bind<IVGLTrialBalanceWithBudgetRepository>().To<vGLTrialBalanceWithBudgetRepository>();



        }
    }
}
