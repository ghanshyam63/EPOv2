using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Net.Mime;

    public class DNews:BaseEntity
    {
            public string Title { get; set; }

            [MaxLength]
            public string Text { get; set; }

            public string ImagePath { get; set; }

            public string Icon { get; set; }

            public string IconColor { get; set; }
            
            public bool IsPublished { get; set; }

        
    }
}
