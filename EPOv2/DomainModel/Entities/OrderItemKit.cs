namespace DomainModel.Entities
{
    public class OrderItemKit:BaseEntity
    {
        public virtual CostCentre CostCentre { get; set; }

        public virtual Account Account { get; set; }

        public string Part { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }
    }
}
