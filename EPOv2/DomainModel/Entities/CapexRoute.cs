namespace DomainModel.Entities
{
    public class CapexRoute:BaseEntity
    {
        public virtual Capex Capex { get; set; }
        public virtual CapexApprover Approver { get; set; }
        public int Number { get; set; }
    }
}
