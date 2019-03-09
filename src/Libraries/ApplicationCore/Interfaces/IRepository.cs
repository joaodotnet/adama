using ApplicationCore.Entities;
using System.Collections.Generic;

namespace ApplicationCore.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
