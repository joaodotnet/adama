using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Specification.EntityFrameworkCore;
using System.Threading;
using Ardalis.Specification;

namespace Infrastructure.Data
{
    /// <typeparam name="T"></typeparam>
    // public class EfRepository<T> : IRepository<T> where T : BaseEntity, IAggregateRoot
    // {
    //     protected readonly DamaContext _dbContext;

    //     public EfRepository(DamaContext dbContext)
    //     {
    //         _dbContext = dbContext;
    //     }

    //     public virtual async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    //     {
    //         return await _dbContext.Set<T>().FindAsync(id);
    //     }

    //     public async Task<T> GetSingleBySpecAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    //     {
    //         return (await ListAsync(spec)).FirstOrDefault();
    //     }

    //     public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
    //     {
    //         return await _dbContext.Set<T>().ToListAsync();
    //     }

    //     public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    //     {
    //         return await ApplySpecification(spec).ToListAsync();
    //     }

    //     public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    //     {
    //         return await ApplySpecification(spec).CountAsync();
    //     }

    //     public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    //     {
    //         _dbContext.Set<T>().Add(entity);
    //         await _dbContext.SaveChangesAsync();

    //         return entity;
    //     }
    //     public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    //     {
    //         _dbContext.Entry(entity).State = EntityState.Modified;
    //         await _dbContext.SaveChangesAsync();
    //     }
    //     public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    //     {
    //         _dbContext.Set<T>().Remove(entity);
    //         await _dbContext.SaveChangesAsync();
    //     }

    //     private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    //     {
    //         var evaluator = new SpecificationEvaluator();
    //         return evaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), spec, false);
    //     }
    // }
}
