using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FLive.Web.Data;
using FLive.Web.Models;
using FLive.Web.Shared;
using Microsoft.EntityFrameworkCore;

namespace FLive.Web.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : Entity
    {
        public GenericRepository(ApplicationDbContext applicationDbContext)
        {
            Context = applicationDbContext;
        }

        public ApplicationDbContext Context { get; set; }
        public IQueryable<T> Vault => Context.Set<T>();

        public async Task<T> GetById(long id)
        {
            IQueryable<T> set = Context.Set<T>();
            return await set.Where(a => a.Id == id).FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<T>> GetMultiple(Expression<Func<T, bool>> specification, string[] includes = null)
        {
            IQueryable<T> set = Context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    set = set.Include(include);
                }
            }

            return await set.Where(specification).ToListAsync();

        }

        public virtual async Task<PaginatedResult<T>> GetMultiplePaginated(Expression<Func<T, bool>> specification , Expression<Func<T,dynamic>> orderBy, string[] includes=null, int page=0, int pageSize=20)
        {
            IQueryable<T> set = Context.Set<T>();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    set = set.Include(include);
                }
            }
            var result =  await set.Where(specification).OrderBy(orderBy).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var totalCount = await set.Where(specification).CountAsync();
            int pageCount = (totalCount + pageSize - 1) / pageSize;

            return new PaginatedResult<T> { Results = result, Total = totalCount, PageCount = pageCount };

        }

        public virtual  async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> specification)
        {
            IQueryable<T> set = Context.Set<T>();
            return await set.Where(specification).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            IQueryable<T> set = Context.Set<T>();
            return await set.AsNoTracking().ToListAsync();
        }
        
        public async Task<long> Add(T entity)
        {
            Context.Set<T>().Add(entity);
            await Context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task Remove(T entity)
        {
            Context.Set<T>().Remove(entity);
            await Context.SaveChangesAsync();
        }

        public async Task Save()
        {
            await Context.SaveChangesAsync();
        }

      
    }
}