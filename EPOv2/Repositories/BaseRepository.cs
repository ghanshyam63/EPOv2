using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.DataContext;

namespace Repositories
{
    using System.Data.Entity;
    using System.Linq.Expressions;
    using EntityFramework.BulkInsert.Extensions;
    using DomainModel.Entities;

    using Interfaces;

    public class BaseRepository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
        /// </summary>
        /// <param name="contextManager">
        /// The context manager.
        /// </param>
        /// <param name="dataContext"></param>
        public BaseRepository(IDataContext dataContext)
        {
            if (dataContext == null)
            {
                throw new ArgumentNullException("contextManager");
            }

            this.Context = (DbContext)dataContext;
        }

        /// <summary>
        /// Gets the context manager.
        /// </summary>
       // protected IDbContextManager ContextManager { get; private set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        private DbContext Context { get; set; }

        /// <summary>
        /// Gets the database set.
        /// </summary>
        /// <value>The database set.</value>
        private IDbSet<T> DbSet
        {
            get { return this.Context.Set<T>(); }
        }

        /// <summary>
        /// Gets the specified entities filtered by specified filter expression.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <returns>Result set.</returns>
        public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter)
        {
            return this.Get().Where(filter);
        }

        /// <summary>
        /// Gets the specified entities filtered by specified filter expression and
        /// loaded with specified dependent objects.
        /// </summary>
        /// <param name="filter">The filter to be used in Where clause.</param>
        /// <param name="includes">The related objects to be loaded.</param>
        /// <returns>Materialized result set.</returns>
        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var get = this.Get();
            foreach (var include in includes)
            {
                get.Include(include);
            }

            return get.Where(filter).ToList();
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var get = this.Get();
            foreach (var include in includes)
            {
                get.Include(include);
            }

            return get.Where(filter).FirstOrDefault();
        }

        public virtual T SingleOrDefault(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var get = this.Get();
            foreach (var include in includes)
            {
                get.Include(include);
            }

            return get.Where(filter).SingleOrDefault();
        }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>Result set</returns>
        public virtual IQueryable<T> Get()
        {
            return this.DbSet;
        }

        /// <summary>
        /// Gets the read only records only.
        /// </summary>
        /// <returns>List of read-only records</returns>
        public virtual IQueryable<T> GetReadOnly()
        {
            return this.DbSet.AsNoTracking();
        }

        public virtual IQueryable<T> GetReadOnly(Expression<Func<T, bool>> filter)
        {
            return this.GetReadOnly().Where(filter);
        }

        /// <summary>
        /// Finds the entity by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Found entity</returns>
        public virtual T Find(object id)
        {
            return this.DbSet.Find(id);
        }

        /// <summary>
        /// Adds the specified entity to database context.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        public virtual void Add(T entity)
        {
            this.DbSet.Add(entity);
        }

        /// <summary>
        /// Perform bulk insert of entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public virtual void BulkInsert(IEnumerable<T> entities)
        {
            var currDate = DateTime.Now;
            var user = "system";
            foreach (var entity in entities.OfType<BaseEntity>())
            {
                // entity.CreatedBy = user;
                // entity.DateCreated = currDate;
                entity.LastModifiedBy = user;
                entity.LastModifiedDate = currDate;
            }

            Context.BulkInsert(entities);
        }

        /// <summary>
        /// Deletes the specified entity by id.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <param name="permanentDelete">Flag that indicates whether entity will be deleted
        /// permanently rather then just deactivated</param>
        public virtual void Delete(object id, bool permanentDelete = false)
        {
            T entityToDelete = this.Find(id);
            this.Delete(entityToDelete, permanentDelete);
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        /// <param name="permanentDelete">Flag that indicates whether entity will be deleted
        /// permanently rather then just deactivated</param>
        public virtual void Delete(T entityToDelete, bool permanentDelete = false)
        {
            if (this.Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                this.DbSet.Attach(entityToDelete);
            }

            var baseEntity = entityToDelete as BaseEntity;

            if (permanentDelete && baseEntity != null)
            {
                this.Context.Entry(entityToDelete).State = EntityState.Modified;
                baseEntity.IsDeleted = false;
            }
            else
            {
                this.DbSet.Remove(entityToDelete);
            }
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entityToUpdate">The entity to update.</param>
        public virtual void Update(T entityToUpdate)
        {
            this.DbSet.Attach(entityToUpdate);
            this.Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        /// <summary>
        /// Loads specified entity with related collection.
        /// </summary>
        /// <typeparam name="TElement">The type of the collection element.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="expression">The collection expression.</param>
        public virtual void LoadCollection<TElement>(T entity, Expression<Func<T, ICollection<TElement>>> expression) where TElement : class
        {
            this.Context.Entry(entity).Collection(expression).Load();
        }

        /// <summary>
        /// Loads specified entity with related object.
        /// </summary>
        /// <typeparam name="TProperty">The type of the related object.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="expression">The object expression.</param>
        public virtual void LoadReference<TProperty>(T entity, Expression<Func<T, TProperty>> expression) where TProperty : class
        {
            this.Context.Entry(entity).Reference(expression).Load();
        }
    }
}
