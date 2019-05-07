using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Entities
{
    public class DCalendarEventType:BaseEntity
    {
        public string Name { get; set; }

        public string Icon { get; set; }

        public string IconColor { get; set; }
    }
}
