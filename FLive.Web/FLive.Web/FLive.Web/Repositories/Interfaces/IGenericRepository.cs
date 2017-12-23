using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FLive.Web.Data;
using FLive.Web.Models;

namespace FLive.Web.Repositories
{
    public interface IGenericRepository<T> where T : Entity
    {
         IQueryable<T> Vault { get; }
        ApplicationDbContext Context { get; set; }
        Task<T> GetById(long id);
        Task<IEnumerable<T>> GetMultiple(Expression<Func<T, bool>> specification, string[] includes = null);
        Task<PaginatedResult<T>> GetMultiplePaginated(Expression<Func<T, bool>> specification, Expression<Func<T, dynamic>> orderBy, string[] includes=null, int page=1, int pageSize=20);
        Task<T> GetFirstOrDefault(Expression<Func<T, bool>> specification);
        Task<IEnumerable<T>> GetAll();

        Task<long> Add(T entity);
        Task Remove(T entity);
        Task Save();
    }
}