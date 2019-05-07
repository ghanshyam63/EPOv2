namespace DomainModel.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    using DomainModel.Entities;

    public class vGLTrialBalanceWithBudgetMap : EntityTypeConfiguration<vGLTrialBalanceWithBudget>
    {
        public vGLTrialBalanceWithBudgetMap()
        {
            // Primary Key
            //this.HasKey(t => t.Date);

            // Properties
            this.Property(t => t.EntityCode)
                .HasMaxLength(30);

            this.Property(t => t.AccountCode)
                .HasMaxLength(80);

            this.Property(t => t.SubAccountCode)
                .HasMaxLength(80);

            this.Property(t => t.CostCentreCode)
                .HasMaxLength(30);

            this.Property(t => t.Project)
                .HasMaxLength(80);

            this.Property(t => t.AccountName)
                .HasMaxLength(80);

            this.Property(t => t.AccountType)
                .HasMaxLength(30);

            this.Property(t => t.GLYear);

            this.Property(t => t.GLPeriod);

            this.Property(t => t.GLPeriodDate);
                
            this.Property(t => t.ActualPeriodActivity).IsOptional();

            this.Property(t => t.ActualPeriodOpenBalance).IsOptional();

            this.Property(t => t.ActualPeriodCloseBalance);

            this.Property(t => t.BudgetPeriod);

            this.Property(t => t.BudgetYearToDate);

            this.Property(t => t.BudgetFullYear);



            // Table & Column Mappings
            this.ToTable("vGLBalanceWithBudget");
            this.Property(t => t.EntityCode).HasColumnName("Entity");
            this.Property(t => t.AccountCode).HasColumnName("Account");
            this.Property(t => t.SubAccountCode).HasColumnName("SubAccount");
            this.Property(t => t.CostCentreCode).HasColumnName("CostCentre");
            this.Property(t => t.Project).HasColumnName("Project");
            this.Property(t => t.AccountName).HasColumnName("AccountDescription");
            this.Property(t => t.AccountType).HasColumnName("AccountType");
            this.Property(t => t.GLYear).HasColumnName("GLYear");
            this.Property(t => t.GLPeriodDate).HasColumnName("GLPeriodDate");
            this.Property(t => t.GLPeriod).HasColumnName("GLPeriod");
            this.Property(t => t.ActualPeriodOpenBalance).HasColumnName("ActualPeriodOpenBalance");
            this.Property(t => t.ActualPeriodActivity).HasColumnName("ActualPeriodActivity");
            this.Property(t => t.ActualPeriodCloseBalance).HasColumnName("ActualPeriodCloseBalance");
            this.Property(t => t.BudgetPeriod).HasColumnName("BudgetPeriod");
            this.Property(t => t.BudgetYearToDate).HasColumnName("BudgetYearToDate");
            this.Property(t => t.BudgetFullYear).HasColumnName("BudgetFullYear");

        }
    }

   
}
