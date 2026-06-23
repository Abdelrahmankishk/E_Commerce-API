using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Repositories
{
    internal class GenericRepository<T, TKey>(StoreDbContext dbContext) : IGenericRepository<T, TKey> where T : BaseEntity<TKey>
    {
        public void Add(T entity)
        {
            dbContext.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            dbContext.Set<T>().Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
           return await dbContext.Set<T>().ToListAsync(ct);
        }

        public async Task<T?> GetByIdAsync(TKey id, CancellationToken ct = default)
        {
            return await dbContext.Set<T>().FindAsync(id, ct);
        }

        public void Update(T entity)
        {
            dbContext.Set<T>().Update(entity);
        }
    }
}
