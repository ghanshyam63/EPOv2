namespace DomainModel.Entities
{
    public class CostCentreToEntity:BaseEntity
    {
        public virtual CostCentre CostCentre { get; set; }

        public virtual Entity Entity { get; set; }
    }
}