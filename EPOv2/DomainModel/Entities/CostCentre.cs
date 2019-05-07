namespace DomainModel.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class CostCentre:BaseEntity
    {
        [Display(Name="Code")]
        public int Code { get; set; }

        public string Name { get; set; }

         [Display(Name = "Owner")]
        public virtual User Owner { get; set; }
        public string GetFullName()
        {
            return Code + " - " + this.Name;
        }

    }
}