namespace EPOv2.ViewModels
{
    using DomainModel.Entities;

    public class OwnerViewModel
    {
        public string Id { get; set; }

        public int EmpId { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public User User { get; set; }

        public int Level { get; set; }

        public int Limit { get; set; }

        public CostCentre CostCentre { get; set; }
    }

    public class AuthorViewModel
    {
        public string Id { get; set; }

        public int EmpId { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public User User { get; set; }

        public int Level { get; set; }

        public int Limit { get; set; }

        public CostCentre CostCentre { get; set; }
    }
}