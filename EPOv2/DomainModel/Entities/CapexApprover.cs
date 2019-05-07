namespace DomainModel.Entities
{
    public class CapexApprover:BaseEntity
    {
        public virtual User User { get; set; }

        public string Role { get; set; }

        public int Level { get; set; }

        public double Limit { get; set; }

        public virtual Division Division { get; set; }
        public string oldapprover { get; set; }
     
    }
}
