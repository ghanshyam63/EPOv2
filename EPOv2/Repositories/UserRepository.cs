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

    using Interfaces;

    public partial class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IDataContext contextmanager)
            : base(contextmanager)
        {
        }
    }
}
