using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Entities
{
    public class OrderLog:BaseEntity
    {
        public virtual Order LatestOrder { get; set; }

        public Status Status { get; set; }

        public string Subject { get; set; }

        public DateTime OrderDateCreated { get; set; }
        public DateTime OrderLastModifiedDate { get; set; }

        public string OrderCreatedBy { get; set; }

        public string OrderLastModifiedBy { get; set; }
    }
}
