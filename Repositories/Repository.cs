using Backend.Helper;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Backend.Auth;

namespace Backend.Repositories
{

    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        internal readonly DbContext dbContext;
        internal IQueryable<T> set;
        public Repository(DbContext dbContext)
        {
            this.dbContext = dbContext;
            set = dbContext.Set<T>();
        }
        public virtual async Task<T> AddAsync(T entity, ApplicationUser? user)
        {
            //TODO: handle creator and update user and rest
            ((BaseEntity)entity).CreatedDate = DateTime.Now;
            ((BaseEntity)entity).UpdatedDate = DateTime.Now;
            ((BaseEntity)entity).IsDeleted = false;
            if (!((BaseEntity)entity).IsActive.HasValue)
            {
                ((BaseEntity)entity).IsActive = true;
            }
            ((BaseEntity)entity).ChangeSequenceNumber = 0;
            ((BaseEntity)entity).CreatorUserId = user != null ? user.Id : null;
            ((BaseEntity)entity).LastUpdateUserId = user != null ? user.Id : null; ;

            await dbContext.Set<T>().AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> match)
        {
            var toRemove = await GetListAsync(match);
            if (toRemove != null)
            {
                dbContext.Set<T>().RemoveRange(toRemove);
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<IEnumerable<T>> GetAllAsync(Paging<T> paging = null, params Expression<Func<T, object>>[] include)
        {
            if (include != null)
            {
                foreach (var includeElement in include)
                {
                    set = set.Include(includeElement.AsPath());
                }
            }
            if (paging is null)
            {
                return await set.ToListAsync();
            }
            paging.TotalCount = await set.CountAsync();
            return await set.OrderByDescending(paging.OrderBy).Skip(paging.Skip).Take(paging.PageSize).ToListAsync();

        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> match, Expression<Func<T, object>> orderBy = null, bool OrderDesc = true, params Expression<Func<T, object>>[] include)
        {
            if (include != null)
            {
                foreach (var includeElement in include)
                {
                    set = set.Include(includeElement.AsPath());
                }
            }
            if (orderBy is null)
                return await set.SingleOrDefaultAsync(match);
            else
            {
                if (OrderDesc)
                {
                    return await set.OrderByDescending(orderBy).FirstOrDefaultAsync(match);
                }
                else
                {
                    return await set.OrderBy(orderBy).FirstOrDefaultAsync(match);
                }
            }
        }

        public virtual async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> match, Paging<T> paging = null, params Expression<Func<T, object>>[] include)
        {

            if (include != null)
            {
                foreach (var includeElement in include)
                {
                    set = set.Include(includeElement.AsPath());
                }
            }
            if (paging is null)
            {
                return await set.Where(match).ToListAsync();
            }
            paging.TotalCount = await set.Where(match).CountAsync();
            return await set.Where(match).OrderByDescending(paging.OrderBy).Skip(paging.Skip).Take(paging.PageSize).ToListAsync();
        }

        public virtual async Task<bool> UpdateAsync(T entity, object key, ApplicationUser? user)
        {
            var exist = await Find(key);
            if (exist != null)
            {
                ((BaseEntity)entity).UpdatedDate = DateTime.Now;
                ((BaseEntity)entity).ChangeSequenceNumber += 1;
                ((BaseEntity)entity).LastUpdateUserId = user != null ? user.Id : null;

                dbContext.Entry(exist).CurrentValues.SetValues(entity);
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public virtual async Task<bool> UpdateAsync(T entity, Expression<Func<T, bool>> match, ApplicationUser? user)
        {
            var exist = await GetAsync(match);
            if (exist != null)
            {
                ((BaseEntity)entity).UpdatedDate = DateTime.Now;
                ((BaseEntity)entity).ChangeSequenceNumber += 1;
                ((BaseEntity)entity).LastUpdateUserId = user != null ? user.Id : null;

                dbContext.Entry(exist).CurrentValues.SetValues(entity);

                await dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<T> Find(object key)
        {
            return await dbContext.Set<T>().FindAsync(key);
        }
    }
}