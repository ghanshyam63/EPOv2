
using DomainModel.Entities;
using Interfaces;

namespace EPOv2.Repositories.Interfaces
{
	public partial interface IAccountRepository : IRepository<Account>
	{
	}
	public partial interface IAccountCategoryRepository : IRepository<AccountCategory>
	{
	}
	public partial interface IAccountToCategoryRepository : IRepository<AccountToCategory>
	{
	}
	public partial interface IAccountToCostCentreRepository : IRepository<AccountToCostCentre>
	{
	}
	public partial interface IApproverRepository : IRepository<Approver>
	{
	}
	public partial interface ICapexRepository : IRepository<Capex>
	{
	}
	public partial interface ICapexApproverRepository : IRepository<CapexApprover>
	{
	}
	public partial interface ICapexRouteRepository : IRepository<CapexRoute>
	{
	}
	public partial interface ICostCentreRepository : IRepository<CostCentre>
	{
	}
	public partial interface ICostCentreToEntityRepository : IRepository<CostCentreToEntity>
	{
	}
	public partial interface ICurrencyRepository : IRepository<Currency>
	{
	}
	public partial interface IDCalendarEventRepository : IRepository<DCalendarEvent>
	{
	}
	public partial interface IDCalendarEventTypeRepository : IRepository<DCalendarEventType>
	{
	}
	public partial interface IDeliveryAddressRepository : IRepository<DeliveryAddress>
	{
	}
	public partial interface IDivisionRepository : IRepository<Division>
	{
	}
	public partial interface IDNewsRepository : IRepository<DNews>
	{
	}
	public partial interface IDTileRepository : IRepository<DTile>
	{
	}
	public partial interface IDUserGroupRepository : IRepository<DUserGroup>
	{
	}
	public partial interface IEntityRepository : IRepository<Entity>
	{
	}
	public partial interface IGroupRepository : IRepository<Group>
	{
	}
	public partial interface IGroupMemberRepository : IRepository<GroupMember>
	{
	}
	public partial interface ILevelRepository : IRepository<Level>
	{
	}
	public partial interface IMatchOrderRepository : IRepository<MatchOrder>
	{
	}
	public partial interface IOrderRepository : IRepository<Order>
	{
	}
	public partial interface IOrderItemRepository : IRepository<OrderItem>
	{
	}
	public partial interface IOrderItemKitRepository : IRepository<OrderItemKit>
	{
	}
	public partial interface IOrderItemLogRepository : IRepository<OrderItemLog>
	{
	}
	public partial interface IOrderLogRepository : IRepository<OrderLog>
	{
	}
	public partial interface IRouteRepository : IRepository<Route>
	{
	}
	public partial interface IStateRepository : IRepository<State>
	{
	}
	public partial interface IStatusRepository : IRepository<Status>
	{
	}
	public partial interface ISubstituteApproverRepository : IRepository<SubstituteApprover>
	{
	}
	public partial interface IUserDashboardSettingsRepository : IRepository<UserDashboardSettings>
	{
	}
	public partial interface IUserInfoRepository : IRepository<UserInfo>
	{
	}
	public partial interface IUserOrderSettingsRepository : IRepository<UserOrderSettings>
	{
	}
	public partial interface IVoucherRepository : IRepository<Voucher>
	{
	}
	public partial interface IVoucherStatusRepository : IRepository<VoucherStatus>
	{
	}
	public partial interface IVoucherDocumentRepository : IRepository<VoucherDocument>
	{
	}
	public partial interface IVoucherDocumentTypeRepository : IRepository<VoucherDocumentType>
	{
	}
	public partial interface IVoucherRouteRepository : IRepository<VoucherRoute>
	{
	}
} // end namespace