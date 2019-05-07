using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    using DomainModel.DataContext;
    using DomainModel.Entities;

    using EPOv2.Repositories.Interfaces;

    using Microsoft.AspNet.Identity.EntityFramework;

    public partial class RoleRepository : BaseRepository<IdentityRole>, IRoleRepository
    {
        public RoleRepository(IDataContext contextmanager)
            : base(contextmanager)
        {
        }
    }
}
