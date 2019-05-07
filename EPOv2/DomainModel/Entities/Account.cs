namespace DomainModel.Entities
{
    public class Account:BaseEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int Type { get; set; } //0 - Account; 1 - Sub Account

        public string GetFullName()
        {
            return Code + " - " + this.Name;
        }
    }

}