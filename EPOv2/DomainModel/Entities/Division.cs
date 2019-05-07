namespace DomainModel.Entities
{
    public class Division:BaseEntity
    {
        public string Name { get; set; }

        public int CostCentreRangeFrom { get; set; }
        public int CostCentreRangeTo   { get; set; }
        public virtual User  Owner { get; set; }
    }
}
