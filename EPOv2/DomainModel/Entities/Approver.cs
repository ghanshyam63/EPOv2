namespace DomainModel.Entities
{
    

    public class Approver:BaseEntity
    {
        public virtual User User { get; set; }
        public int Level { get; set; }
        public int Limit { get; set; }
        public string OldApprover { get; set; }
        //public int Number { get; set; }

    }
    
}