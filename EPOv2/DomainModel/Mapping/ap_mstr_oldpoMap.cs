namespace DomainModel.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    using DomainModel.Entities;

    public class ap_mstr_oldpoMap : EntityTypeConfiguration<ap_mstr_oldpo>
    {
        public ap_mstr_oldpoMap()
        {
            // Primary Key
            this.HasKey(t => t.VoicherId);

            // Properties
            this.Property(t => t.InvoiceNumber)
                .HasMaxLength(40);

            this.Property(t => t.VoucherNumber)
                .HasMaxLength(50);

            this.Property(t => t.SupplierId)
                .HasMaxLength(100);

            this.Property(t => t.UserComment)
                .HasMaxLength(1024);

            this.Property(t => t.Status);

            this.Property(t => t.PORequired);

            this.Property(t => t.UserId)
                .HasMaxLength(100);

            this.Property(t => t.Created);

            this.Property(t => t.Updated);


            // Table & Column Mappings
            this.ToTable("ap_mstr");
            this.Property(t => t.VoicherId).HasColumnName("voucherID");
            this.Property(t => t.VoucherNumber).HasColumnName("voucherNumber");
            this.Property(t => t.SupplierId).HasColumnName("supplierID");
            this.Property(t => t.InvoiceNumber).HasColumnName("invoiceNumber");
            this.Property(t => t.UserComment).HasColumnName("userComments");
            this.Property(t => t.Status).HasColumnName("status");
            this.Property(t => t.PORequired).HasColumnName("PORequired");
            this.Property(t => t.UserId).HasColumnName("userID");
            this.Property(t => t.Created).HasColumnName("created");
            this.Property(t => t.Updated).HasColumnName("updated");
        }
    }
}
