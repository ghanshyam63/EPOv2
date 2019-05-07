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
    using Microsoft.AspNet.Identity.EntityFramework;

    public partial class vBasedataFinancialCalendarRepository : BaseRepository<vBasedataFinancialCalendar>, IVBasedataFinancialCalendarRepository
    {
        public vBasedataFinancialCalendarRepository(IDataContext contextmanager)
            : base(contextmanager)
        {
        }
    }
}
