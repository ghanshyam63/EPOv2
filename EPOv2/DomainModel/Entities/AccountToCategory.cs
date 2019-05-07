using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Entities
{
    public class AccountToCategory:BaseEntity
    {
        public virtual Account Account { get; set; }

        public virtual AccountCategory Category { get; set; }
    }
}
