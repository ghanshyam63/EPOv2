namespace DomainModel.Entities
{
    public class Route : BaseEntity
    {
        public virtual Order Order { get; set; }
        public virtual Approver Approver { get; set; }
        public int Number { get; set; }

    }
}