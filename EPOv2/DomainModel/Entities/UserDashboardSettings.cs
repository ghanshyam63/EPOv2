using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    using DomainModel.BaseClasses;

    public class UserDashboardSettings:BaseEntity
    {
        public virtual DUserGroup DUserGroup { get; set; }

        [Column("MyTiles", TypeName = "ntext")]
        public virtual PersistableIntCollection MyTiles { get; set; }

        //need to add shortcut links ?!

    }
}
