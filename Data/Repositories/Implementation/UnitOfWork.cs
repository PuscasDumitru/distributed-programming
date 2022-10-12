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
        GenericRepository<User> _userRepository;
        GenericRepository<Post> _postRepository;
       
        public UnitOfWork(RepositoryDbContext dbContext) => _dbContext = dbContext;

        public GenericRepository<User> UserRepository
        {
            get
            {
                _userRepository ??= new GenericRepository<User>(_dbContext);
                return _userRepository;
            }
        }

        public GenericRepository<Post> PostRepository
        {
            get
            {
                _postRepository ??= new GenericRepository<Post>(_dbContext);
                return _postRepository;
            }
        }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _dbContext.SaveChangesAsync(cancellationToken);
    }
}
