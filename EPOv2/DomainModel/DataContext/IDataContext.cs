using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.DataContext
{
    using System.Data.Entity;

    public interface IDataContext
    {
        IDbSet<T> Set<T>() where T : class;
        int SaveChanges();
        void ExecuteCommand(string command, params object[] parameters);
    }
}
