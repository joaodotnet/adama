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

    public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
    {
    }
}
