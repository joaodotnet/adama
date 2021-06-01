using ApplicationCore.Entities;
using Ardalis.Specification;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
     public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
    {
    }

    // public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
    // {
    // }
//   public interface IRepository<T> where T : BaseEntity, IAggregateRoot
//     {
//         Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
//         Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);
//         Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
//         Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
//         Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
//         Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
//         Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
//     }
}
