namespace DomainModel.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    using DomainModel.Entities;

    public class vRockyEmployesMap : EntityTypeConfiguration<vRockyEmployees>
    {
        public vRockyEmployesMap()
        {
            // Primary Key
            this.HasKey(t => t.EmpNo);

            // Properties
            this.Property(t => t.Level)
                .HasMaxLength(1);

            this.Property(t => t.Title)
                .HasMaxLength(10);

            this.Property(t => t.FirstName)
                .HasMaxLength(30);

            this.Property(t => t.Surname)
                .HasMaxLength(30);

            this.Property(t => t.Street)
                .HasMaxLength(50);

            this.Property(t => t.Suburb)
                .HasMaxLength(40);

            this.Property(t => t.State)
                .HasMaxLength(12);

            this.Property(t => t.PostCode)
                .HasMaxLength(10);

            this.Property(t => t.ManagerEmpNo)
                .HasMaxLength(12);

            this.Property(t => t.ManagerLevel)
                .HasMaxLength(1);

            this.Property(t => t.ManagerFirstName)
                .HasMaxLength(30);

            this.Property(t => t.ManagerSurname)
                .HasMaxLength(30);

            this.Property(t => t.Active);
                

            // Table & Column Mappings
            this.ToTable("vRockyEmployees");
            this.Property(t => t.EmpNo).HasColumnName("EmpNo");
            this.Property(t => t.Level).HasColumnName("Level");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.Surname).HasColumnName("Surname");
            this.Property(t => t.Street).HasColumnName("Street");
            this.Property(t => t.Suburb).HasColumnName("Suburb");
            this.Property(t => t.State).HasColumnName("State");
            this.Property(t => t.PostCode).HasColumnName("PostCode");
            this.Property(t => t.ManagerEmpNo).HasColumnName("ManagerEmpNo");
            this.Property(t => t.ManagerLevel).HasColumnName("ManagerLevel");
            this.Property(t => t.ManagerFirstName).HasColumnName("ManagerFirstName");
            this.Property(t => t.ManagerSurname).HasColumnName("ManagerSurname");
            this.Property(t => t.Active).HasColumnName("Active");
        }
    }
}
