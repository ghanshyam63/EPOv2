using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPOv2.Repositories.Interfaces
{
    using DomainModel.Entities;

    using global::Interfaces;

    using Microsoft.AspNet.Identity.EntityFramework;

    public interface IRoleRepository : IRepository<IdentityRole>
    {
    }
}
