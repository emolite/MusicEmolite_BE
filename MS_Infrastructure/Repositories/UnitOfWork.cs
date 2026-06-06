using Microsoft.EntityFrameworkCore;
using MS_Application.Repositories.Interfaces;
using MS_Infrastructure.DataAccess;
using MS_Infrastructure.DataAccess.DISTS.Contexts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MS_Infrastructure.Repositories
{
    public class UnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext
    {
        private readonly Dictionary<Type, object> _writeRepositories = new();
        private readonly Dictionary<Type, object> _readOnlyRepositories = new();

        public UnitOfWork(TContext context)
        {
            Context = context;
        }

        public TContext Context { get; }

        public IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>()
            where TEntity : class
        {
            var type = typeof(TEntity);
            if (!_writeRepositories.ContainsKey(type))
                _writeRepositories[type] = new RepositoryAsync<TEntity>(Context);
            return (IRepositoryAsync<TEntity>)_writeRepositories[type];
        }

        public IRepositoryReadOnlyAsync<TEntity> GetRepositoryReadOnlyAsync<TEntity>()
            where TEntity : class
        {
            var type = typeof(TEntity);
            if (!_readOnlyRepositories.ContainsKey(type))
                _readOnlyRepositories[type] = new RepositoryReadOnlyAsync<TEntity>(Context);
            return (IRepositoryReadOnlyAsync<TEntity>)_readOnlyRepositories[type];
        }

        public Task<int> SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }

    public class CrmUnitOfWork : UnitOfWork<CrmDbContext>, ICrmUnitOfWork
    {
        public CrmUnitOfWork(CrmDbContext context)
            : base(context)
        {
        }
    }

    public class DistUnitOfWork : UnitOfWork<DistDbContext>, IDistUnitOfWork
    {
        public DistUnitOfWork(DistDbContext context)
            : base(context)
        {
        }
    }
}