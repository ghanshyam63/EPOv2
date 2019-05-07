using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Entities
{
    using Enums;

    public class DTile:BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Site Site { get; set; }

        public Department Department { get; set; }

        public TileStyle TileStyle { get; set; }

        public TileType TileType { get; set; }

        public TileSubType TileSubType { get; set; }
    }

   
}
