using Data.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly RepositoryDbContext _dbContext;
        PostRepository _postRepository;
       
        public UnitOfWork(RepositoryDbContext dbContext) => _dbContext = dbContext;

        public PostRepository PostRepository
        {
            get
            {
                _postRepository ??= new PostRepository(_dbContext);
                return _postRepository;
            }
        }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _dbContext.SaveChangesAsync(cancellationToken);
    }
}
