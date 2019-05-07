namespace DomainModel.Entities
{
    public class DeliveryAddress:BaseEntity
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public virtual State State { get; set; }

        public int PostCode { get; set; }
    }
}