namespace DomainModel.Entities
{
    using System;

    public class OrderItemLog:BaseEntity
    {
        public virtual OrderItem LatestOrderItem { get; set; }  
        public int LineNumber { get; set; }

        public int RevisionQty { get; set; }

        public virtual Account Account { get; set; }

        public virtual Account SubAccount { get; set; }

        public virtual Status Status { get; set; }

        public int? Capex_Id { get; set; }

        public DateTime DueDate { get; set; }

        public string Description { get; set; }

        public double Qty { get; set; }

        public double UnitPrice { get; set; }

        public virtual Currency Currency { get; set; }
        public double CurrencyRate { get; set; }

        public bool IsGSTInclusive { get; set; }

        public bool IsTaxable { get; set; }

        public bool IsGSTFree { get; set; }
        public double TotalExTax { get; set; }

        public double TotalTax { get; set; }

        public double Total { get; set; }

        public DateTime ItemDateCreated { get; set; }
        public DateTime ItemLastModifiedDate { get; set; }

        public string ItemCreatedBy { get; set; }

        public string ItemLastModifiedBy { get; set; }
    }
}