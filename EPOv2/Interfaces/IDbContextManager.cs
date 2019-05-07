namespace Interfaces
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    //public interface IDbContextManager : IDisposable
    //{
    //    bool HasContext { get; }
    //    DbContext Context { get; }
    //}

    //public interface IDbContextManager<T> : IDbContextManager
    //{
    //}


    //public class DbContextManager<T> : IDbContextManager<T>
    //    where T : DbContext
    //{


    //    IDbContextFactory<T> _factory;

    //    public DbContextManager(IDbContextFactory<T> factory)
    //    {
    //        this._factory = factory;
    //    }

    //    T _context = default(T);
    //    public DbContext Context
    //    {
    //        get
    //        {
    //            if (this._context == null)
    //            {
    //                this._context = this._factory.Create();
    //            }
    //            return this._context as DbContext;
    //        }
    //    }
    //    public bool HasContext
    //    {
    //        get
    //        {
    //            return this._context != null;
    //        }
    //    }


    //    public void Dispose()
    //    {
    //        if (this.HasContext)
    //        {
    //            this._context.Dispose();
    //            this._context = null;
    //        }
    //    }

    //}
}
