namespace DomainModel.Entities
{
    public class AccountToCostCentre:BaseEntity
    {
        public virtual Account Account { get; set; }

        public virtual CostCentre CostCentre { get; set; }
    }
}