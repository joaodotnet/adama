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
    public class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class, IAggregateRoot
    {
            protected readonly DamaContext _dbContext;

            public EfRepository(DamaContext dbContext) : base(dbContext)
            {
                _dbContext = dbContext;
            }

        // Not required to implement anything. Add additional functionalities if required.
       
    }
}
