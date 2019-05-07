namespace DomainModel.Entities
{
    public class UserInfo: BaseEntity
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneWork { get; set; }
        public string PhoneMobile { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public virtual State State { get; set; }
        public int PostCode { get; set; }

    }
}