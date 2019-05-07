namespace DomainModel.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Voucher:BaseEntity
    {
        public int VoucherNumber { get; set; }

        public string SupplierCode { get; set; }

        public string InvoiceNumber { get; set; }

        public virtual VoucherStatus Status { get; set; }

        public string Comment { get; set; }

        public string Terms { get; set; }

        public DateTime DueDate { get; set; }

        public double Amount { get; set; }

        public virtual CostCentre CostCentre { get; set; }

        public virtual Account Account { get; set; }
        
    }

    public class VoucherStatus : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class VoucherDocument : BaseEntity
    {
        public virtual Voucher Voucher { get; set; }
        public virtual VoucherDocumentType  DocumentType { get; set; }

        public bool IsAuthorised { get; set; }

        public virtual User Authoriser { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime? AuthorisedDate { get; set; }

        public string Reference { get; set; } //File name or PO Id or smth else.
        public string oldAuthoriser { get; set; }
    }

    public class VoucherDocumentType:BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }

   // public class Voucher
}