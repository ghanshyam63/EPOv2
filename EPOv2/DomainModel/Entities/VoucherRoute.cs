using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Entities
{
    public class VoucherRoute:BaseEntity
    {
        public virtual Voucher Voucher { get; set; }
        public virtual Approver Approver { get; set; }
        public int Number { get; set; }
    }
}
