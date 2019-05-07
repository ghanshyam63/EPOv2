
using DomainModel.Entities;
using EPOv2.Repositories.Interfaces;
using Interfaces;
using DomainModel.DataContext;

namespace Repositories
{
	public partial class AccountRepository : BaseRepository<Account>, IAccountRepository
	{
		public AccountRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class AccountCategoryRepository : BaseRepository<AccountCategory>, IAccountCategoryRepository
	{
		public AccountCategoryRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class AccountToCategoryRepository : BaseRepository<AccountToCategory>, IAccountToCategoryRepository
	{
		public AccountToCategoryRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class AccountToCostCentreRepository : BaseRepository<AccountToCostCentre>, IAccountToCostCentreRepository
	{
		public AccountToCostCentreRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class ApproverRepository : BaseRepository<Approver>, IApproverRepository
	{
		public ApproverRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class CapexRepository : BaseRepository<Capex>, ICapexRepository
	{
		public CapexRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class CapexApproverRepository : BaseRepository<CapexApprover>, ICapexApproverRepository
	{
		public CapexApproverRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class CapexRouteRepository : BaseRepository<CapexRoute>, ICapexRouteRepository
	{
		public CapexRouteRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class CostCentreRepository : BaseRepository<CostCentre>, ICostCentreRepository
	{
		public CostCentreRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class CostCentreToEntityRepository : BaseRepository<CostCentreToEntity>, ICostCentreToEntityRepository
	{
		public CostCentreToEntityRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class CurrencyRepository : BaseRepository<Currency>, ICurrencyRepository
	{
		public CurrencyRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class DCalendarEventRepository : BaseRepository<DCalendarEvent>, IDCalendarEventRepository
	{
		public DCalendarEventRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class DCalendarEventTypeRepository : BaseRepository<DCalendarEventType>, IDCalendarEventTypeRepository
	{
		public DCalendarEventTypeRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class DeliveryAddressRepository : BaseRepository<DeliveryAddress>, IDeliveryAddressRepository
	{
		public DeliveryAddressRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class DivisionRepository : BaseRepository<Division>, IDivisionRepository
	{
		public DivisionRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class DNewsRepository : BaseRepository<DNews>, IDNewsRepository
	{
		public DNewsRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class DTileRepository : BaseRepository<DTile>, IDTileRepository
	{
		public DTileRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class DUserGroupRepository : BaseRepository<DUserGroup>, IDUserGroupRepository
	{
		public DUserGroupRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class EntityRepository : BaseRepository<Entity>, IEntityRepository
	{
		public EntityRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class GroupRepository : BaseRepository<Group>, IGroupRepository
	{
		public GroupRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class GroupMemberRepository : BaseRepository<GroupMember>, IGroupMemberRepository
	{
		public GroupMemberRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class LevelRepository : BaseRepository<Level>, ILevelRepository
	{
		public LevelRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class MatchOrderRepository : BaseRepository<MatchOrder>, IMatchOrderRepository
	{
		public MatchOrderRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class OrderRepository : BaseRepository<Order>, IOrderRepository
	{
		public OrderRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
	{
		public OrderItemRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class OrderItemKitRepository : BaseRepository<OrderItemKit>, IOrderItemKitRepository
	{
		public OrderItemKitRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class OrderItemLogRepository : BaseRepository<OrderItemLog>, IOrderItemLogRepository
	{
		public OrderItemLogRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class OrderLogRepository : BaseRepository<OrderLog>, IOrderLogRepository
	{
		public OrderLogRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class RouteRepository : BaseRepository<Route>, IRouteRepository
	{
		public RouteRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class StateRepository : BaseRepository<State>, IStateRepository
	{
		public StateRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class StatusRepository : BaseRepository<Status>, IStatusRepository
	{
		public StatusRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class SubstituteApproverRepository : BaseRepository<SubstituteApprover>, ISubstituteApproverRepository
	{
		public SubstituteApproverRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class UserDashboardSettingsRepository : BaseRepository<UserDashboardSettings>, IUserDashboardSettingsRepository
	{
		public UserDashboardSettingsRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class UserInfoRepository : BaseRepository<UserInfo>, IUserInfoRepository
	{
		public UserInfoRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class UserOrderSettingsRepository : BaseRepository<UserOrderSettings>, IUserOrderSettingsRepository
	{
		public UserOrderSettingsRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class VoucherRepository : BaseRepository<Voucher>, IVoucherRepository
	{
		public VoucherRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class VoucherStatusRepository : BaseRepository<VoucherStatus>, IVoucherStatusRepository
	{
		public VoucherStatusRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class VoucherDocumentRepository : BaseRepository<VoucherDocument>, IVoucherDocumentRepository
	{
		public VoucherDocumentRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class VoucherDocumentTypeRepository : BaseRepository<VoucherDocumentType>, IVoucherDocumentTypeRepository
	{
		public VoucherDocumentTypeRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

	public partial class VoucherRouteRepository : BaseRepository<VoucherRoute>, IVoucherRouteRepository
	{
		public VoucherRouteRepository(IDataContext contextmanager)
			: base(contextmanager)
		{
		}
	}

} // end namespace