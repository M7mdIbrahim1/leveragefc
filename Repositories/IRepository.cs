using Backend.Helper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Backend.Auth;

namespace Backend.Repositories
{

    public interface IRepository<T>
    {
        Task<T> AddAsync(T entity, ApplicationUser? user);
        Task<bool> UpdateAsync(T entity, object key, ApplicationUser? user);
        Task<IEnumerable<T>> GetAllAsync(Paging<T> paging = null, params Expression<Func<T, object>>[] include);
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> match, Paging<T> paging = null, params Expression<Func<T, object>>[] include);
        Task<bool> DeleteAsync(Expression<Func<T, bool>> match);
        Task<T> GetAsync(Expression<Func<T, bool>> match, Expression<Func<T, object>> orderBy = null, bool OrderDesc = true, params Expression<Func<T, object>>[] include);
    }
}