namespace DomainModel.Entities
{
    public class Capex:BaseEntity
    {
        public string CapexNumber { get; set; }

        public string Title { get; set; }

        public int RevisionQty { get; set; }

        public virtual Status Status { get; set; }

        public virtual Entity Entity { get; set; }

        public virtual CostCentre CostCentre { get; set; }

        public virtual User Owner { get; set; } //Owner

        public virtual User Author { get; set; }

        public string Description { get; set; }

        public string CapexType { get; set; }

        public string Reference { get; set; }// file path

        public double TotalExGST { get; set; }

        public double TotalGST { get; set; }

        public double Total { get; set; }

        public string Comment { get; set; }
    }
}
