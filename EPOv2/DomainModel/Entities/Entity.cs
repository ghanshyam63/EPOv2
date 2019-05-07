namespace DomainModel.Entities
{
    public class Entity:BaseEntity
    {
        public string Name { get; set; }

        public int CodeNumber { get; set; }

        public string Code { get; set; }

        public string Prefix { get; set; }

        public string ACN { get; set; }

        public string ABN { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }
        public string GetFullName()
        {
            return Code + " - " + this.Name;
        }
    }
}