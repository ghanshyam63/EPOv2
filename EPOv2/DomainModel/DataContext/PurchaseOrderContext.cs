namespace DomainModel.DataContext
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Data.Entity.Core;
    using AgileDesign.Utilities;

    using DomainModel.Entities;
    using DomainModel.Mapping;

    using Microsoft.AspNet.Identity.EntityFramework;

    public partial class PurchaseOrderContext : IdentityDbContext<User>, IDataContext
    {
        static PurchaseOrderContext()
        {
            Database.SetInitializer<PurchaseOrderContext>(null);
        }

        public PurchaseOrderContext()
            : base("Name=PurchaseOrderContext")
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 60000;
           
        }

        public PurchaseOrderContext(string connectionString):base(connectionString)
        {
            
        }

        public void ExecuteCommand(string command, params object[] parameters)
        {   
            base.Database.ExecuteSqlCommand(command, parameters);
        }
        
        public IDbSet<vGLTrialBalanceWithBudget> vGLTrialBalanceBudgets { get; set; }

        public IDbSet<vRockyEmployees> vRockyEmployees { get; set; }
        
        public IDbSet<vBasedataFinancialCalendar> vBaseDataFinancialCalendars { get; set; }
        public IDbSet<UserInfo> UserInfos { get; set; }

        public IDbSet<UserOrderSettings> UserOrderSettings { get; set; }

        public IDbSet<Entity> Entities { get; set; }

        public IDbSet<DomainModel.Entities.Group> Groups { get; set; }

        public IDbSet<DeliveryAddress> DeliveryAddresses { get; set; }

        public IDbSet<GroupMember> GroupMembers { get; set; }

        public IDbSet<CostCentre> CostCentres { get; set; }

        public IDbSet<Account> Accounts { get; set; }

        public IDbSet<State> States { get; set; }

        public IDbSet<Currency> Currencies { get; set; }

        public IDbSet<AccountToCostCentre> AccountToCostCentre { get; set; }

        public IDbSet<CostCentreToEntity> CostCentreToEntity { get; set; }

        public IDbSet<Status> Statuses { get; set; }

        public IDbSet<OrderItem> OrderItems { get; set; }

        public IDbSet<OrderItemKit> OrderItemKits { get; set; }
        public IDbSet<Order> Orders { get; set; }

        public IDbSet<Level> Levels { get; set; }

        public IDbSet<Approver> Approvers { get; set; }
        public IDbSet<Route> Routes { get; set; }

        public IDbSet<SubstituteApprover> SubstituteApprovers { get; set; }

        public IDbSet<MatchOrder> MatchOrders { get; set; }

        public IDbSet<Voucher> Vouchers { get; set; }

        public IDbSet<VoucherStatus> VoucherStatuses { get; set; }

        public IDbSet<VoucherDocument> VoucherDocuments { get; set; }

        public IDbSet<VoucherDocumentType> VoucherDocumentTypes { get; set; }

        public IDbSet<OrderItemLog> OrderItemLogs { get; set; }

        public IDbSet<Capex> Capexes { get; set; }
        public IDbSet<CapexApprover> CapexApprovers { get; set; }

        public IDbSet<CapexRoute> CapexRoutes { get; set; }

        public IDbSet<Division> Divisions { get; set; }

        public IDbSet<DCalendarEventType> DCalendarEventTypes { get; set; }

        public IDbSet<DCalendarEvent> DCalendarEvents { get; set; }

        public IDbSet<AccountCategory> AccountCategories { get; set; }

        public IDbSet<AccountToCategory> AccountToCategories { get; set; }

        public IDbSet<OrderLog> OrderLogs { get; set; }

        public IDbSet<VoucherRoute> VoucherRoutes { get; set; }

        public IDbSet<DNews> DNews { get; set; }
        public IDbSet<DUserGroup> DUserGroups { get; set; }
        public IDbSet<DTile> DTiles { get; set; }
        public IDbSet<UserDashboardSettings> UserDashboardSettings { get; set; }



        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            //var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity );
            var currentUsername = HttpContext.Current != null && HttpContext.Current.User != null
                ? HttpContext.Current.User.Identity.Name.Replace("ONEHARVEST\\", "")
                : "System";

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).DateCreated = DateTime.Now;
                    ((BaseEntity)entity.Entity).CreatedBy = string.IsNullOrEmpty(((BaseEntity)entity.Entity).CreatedBy) ? currentUsername : ((BaseEntity)entity.Entity).CreatedBy;
                }

                ((BaseEntity)entity.Entity).LastModifiedDate = DateTime.Now;
                ((BaseEntity)entity.Entity).LastModifiedBy =
                    string.IsNullOrEmpty(((BaseEntity)entity.Entity).LastModifiedBy)
                        ? currentUsername
                        : ((BaseEntity)entity.Entity).LastModifiedBy;

                //if (System.Data.Entity.Core.Objects.ObjectContext.GetObjectType(entity.Entity.GetType()).Name == typeof(Order).Name && entity.State == EntityState.Modified)
                //{
                //    var orderId = Convert.ToInt32(entity.OriginalValues["Id"]);
                //    var originalStatusId = Orders.AsNoTracking().Where(x=>x.Id==orderId).Select(x => x.Status.Id).First();
                //    var currentStatusId = ((Order)entity.Entity).Status.Id;
                //    if (!Equals(originalStatusId, currentStatusId))
                //    {

                //    }
                //}
            }

            return base.SaveChanges();
        }

        IDbSet<T> IDataContext.Set<T>()
        {
            return base.Set<T>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Configuration.LazyLoadingEnabled = true;

            modelBuilder.Entity<IdentityUserRole>()
                .HasKey(r => new { r.UserId, r.RoleId })
                .ToTable("IdentityUserRoles");

            modelBuilder.Entity<IdentityUserLogin>()
                .HasKey(l => new { l.UserId, l.LoginProvider, l.ProviderKey })
                .ToTable("IdentityUserLogins");

            modelBuilder.Entity<IdentityUserClaim>()
                .ToTable("IdentityUserClaims");

            modelBuilder.Configurations.Add(new vGLTrialBalanceWithBudgetMap());
            var role = modelBuilder.Entity<IdentityRole>()
                .ToTable("IdentityRoles");
            role.Property(r => r.Name).IsRequired();
            role.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<DUserGroup>().Property(c => c.RequiredTiles.SerializedValue).HasColumnName("RequiredTiles");
            modelBuilder.Entity<DUserGroup>().Property(c => c.DefaultTiles.SerializedValue).HasColumnName("DefaultTiles");
            modelBuilder.Entity<UserDashboardSettings>().Property(c => c.MyTiles.SerializedValue).HasColumnName("MyTiles");
        }
    }

  
}


