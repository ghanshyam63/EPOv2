namespace DomainModel.Entities
{
    using System;

    public class MatchOrder:BaseEntity
    {
        public DateTime ReceviedDate { get; set; }

        public double Qty { get; set; }

        public virtual Order Order { get; set; }

        public virtual OrderItem OrderItem { get; set; }
    }
}