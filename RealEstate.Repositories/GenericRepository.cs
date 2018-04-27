using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using RealEstate.Data;
using RealEstate.Repositories.Interfaces;

namespace RealEstate.Repositories
{
    public abstract class GenericRepository<TC, T> :
        IGenericRepository<T> where T : class where TC : RealEstateDbContext, new()
    {
        public GenericRepository(TC db)
        {
            Context = db;
        }

        protected readonly TC Context;

        public virtual IQueryable<T> GetAll()
        {

            IQueryable<T> query = Context.Set<T>();
            return query;
        }

        public IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {

            IQueryable<T> query = Context.Set<T>().Where(predicate);
            return query;
        }

        public IQueryable<T> Include(params Expression<Func<T, object>>[] includes)
        {
            var query = GetAll();
            return includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public virtual void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public virtual void AddRange(List<T> entities)
        {
            Context.Set<T>().AddRange(entities);
        }

        public virtual void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        public virtual void Edit(T entity)
        {
            Context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }

    }
}
