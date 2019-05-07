namespace DomainModel.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class vRockyEmployees
    {
        [Key]
        public string EmpNo { get; set; }
        public string Level { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Street { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string ManagerEmpNo { get; set; }
        public string ManagerLevel { get; set; }
        public string ManagerFirstName { get; set; }
        public string ManagerSurname { get; set; }
        public int Active { get; set; }
    }
}