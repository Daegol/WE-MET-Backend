using Microsoft.EntityFrameworkCore.Query;
using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repos
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        List<T> GetAll();
        Task<List<T>> GetAllWithInclude(Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        T GetById(int id);
        Task<T> GetByIdWithInclude(int id, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        T Find(Expression<Func<T, bool>> match);
        List<T> FindAll(Expression<Func<T, bool>> match);
        Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match);
        Task<List<T>> FindAllAsyncWithInclude(Expression<Func<T, bool>> match, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        T Insert(T entity);
        bool BulkInsert(List<T> entities);
        Task<T> Update(T entity);
        int Delete(T entity);
    }
}
