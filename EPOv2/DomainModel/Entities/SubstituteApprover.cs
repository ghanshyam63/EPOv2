namespace DomainModel.Entities
{
    using System;

    public class SubstituteApprover:BaseEntity
    {
        public virtual User ApproverUser { get; set; }

        public virtual User SubstitutionUser { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

    }
}