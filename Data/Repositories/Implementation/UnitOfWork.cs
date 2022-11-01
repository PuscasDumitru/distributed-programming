using Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly RepositoryDbContext _dbContext;
        PostRepository _postRepository;
        GroupRepository _groupRepository;
       
        public UnitOfWork(RepositoryDbContext dbContext) => _dbContext = dbContext;

        public PostRepository PostRepository
        {
            get
            {
                _postRepository ??= new PostRepository(_dbContext);
                return _postRepository;
            }
        }

        public GroupRepository GroupRepository
        {
            get
            {
                _groupRepository ??= new GroupRepository(_dbContext);
                return _groupRepository;
            }
        }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _dbContext.SaveChangesAsync(cancellationToken);
    }
}
