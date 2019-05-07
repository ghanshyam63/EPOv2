using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Entities
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations.Schema;

    using DomainModel.BaseClasses;

    public class DUserGroup :BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [Column("RequiredTiles")]
        public PersistableIntCollection RequiredTiles { get; set; }

        [Column("DefaultTiles")] 
        public PersistableIntCollection DefaultTiles { get; set; }

        //public virtual List<DTile> DefaultTiles { get; set; }

        //public List<int> GetRequiredTileIdList()
        //{
            
        //}
    }

   

   


}
