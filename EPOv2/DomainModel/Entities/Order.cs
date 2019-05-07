namespace DomainModel.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Order:BaseEntity
    {
        [Index(IsUnique = true)]
        public int OrderNumber { get; set; }

        public string TempOrderNumber { get; set; }

        public int RevisionQty { get; set; }

        public DateTime OrderDate { get; set; }

        public virtual Status Status { get; set; }

        public virtual Entity Entity { get; set; }

        public virtual CostCentre CostCentre { get; set; }

        public virtual Group ReceiptGroup { get; set; }

        public virtual DeliveryAddress DeliveryAddress { get; set; }

        public virtual User User { get; set; } //Attention

        public virtual User Author { get; set; }

        public int SupplierId { get; set; }

        public int Capex_Id { get; set; }

        public string TransmissionMethod { get; set; }

        public virtual List<OrderItem> OrderItems { get; set; }

        public string Comment { get; set; }

        public string InternalComment { get; set; }
        public double TotalExGST { get; set; }

        public double TotalGST { get; set; }

        public double Total { get; set; }

        public string SupplierEmail { get; set; } //TODO: переделать в таблицу адресов для поставщика.

        public string GetAuthorFullName()
        {
            return this.Author.UserInfo.FirstName + " " + this.Author.UserInfo.LastName;
        }

     }
}