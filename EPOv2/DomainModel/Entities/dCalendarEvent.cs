using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Entities
{
    public class DCalendarEvent:BaseEntity
    {
        public string Description { get; set; }

        public string Title { get; set; }

        public virtual DCalendarEventType EventType { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public bool IsOneDayEvent { get; set; }

    }
}
