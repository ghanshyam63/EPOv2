namespace DomainModel.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    using DomainModel.Entities;

    public class vBasedataFinancialCalendarMap : EntityTypeConfiguration<vBasedataFinancialCalendar>
    {
        public vBasedataFinancialCalendarMap()
        {
            // Primary Key
            this.HasKey(t => t.Date);

            // Properties
            this.Property(t => t.FinancialYear)
                .HasMaxLength(4);

            this.Property(t => t.FinancialPeriod)
                .HasMaxLength(2);

            this.Property(t => t.FinancialPeriodYear)
                .HasMaxLength(10);

            this.Property(t => t.FinancialWeek)
                .HasMaxLength(2);

            this.Property(t => t.FinancialWeekYear)
                .HasMaxLength(10);

            this.Property(t => t.FinancialStartingDate);

            this.Property(t => t.FinancialEndingDate);
                

            // Table & Column Mappings
            this.ToTable("vBaseDataFinancialCalendars");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.FinancialYear).HasColumnName("FinancialYear");
            this.Property(t => t.FinancialPeriod).HasColumnName("FinancialPeriod");
            this.Property(t => t.FinancialWeek).HasColumnName("FinancialWeek");
            this.Property(t => t.FinancialPeriodYear).HasColumnName("FinancialPeriodYear");
            this.Property(t => t.FinancialWeekYear).HasColumnName("FinancialWeekYear");
            this.Property(t => t.FinancialStartingDate).HasColumnName("FinancialStartingDate");
            this.Property(t => t.FinancialEndingDate).HasColumnName("FinancialEndingDate");
        }
    }
}
