namespace DomainModel.Entities
{
    public class UserOrderSettings:BaseEntity
    {
        public bool AutoApproveItemKit { get; set; }

        public virtual Entity DefaultEntity { get; set; }

        public virtual CostCentre DefaultCostCentre { get; set; }

        public virtual DeliveryAddress DefaultDeliveryAddress { get; set; }

        public virtual Group DefaultGroup { get; set; }

        public int DefaultSupplierId { get; set; }
    }
}
