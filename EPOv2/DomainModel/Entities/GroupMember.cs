namespace DomainModel.Entities
{
    public class GroupMember:BaseEntity
    {
        public virtual User User { get; set; }

        public virtual Group Group { get; set; }
    }
}