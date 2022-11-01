using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementation
{
    public class PostRepository : GenericRepository<Post>
    {
        public PostRepository(RepositoryDbContext repositoryContext) : base(repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            return await RepositoryContext.Posts
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<IEnumerable<Post>> GetPostByUserIdAsync(Guid userId)
        {
            return await RepositoryContext.Posts
                .Include(p => p.Photos)
                .Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await RepositoryContext.Posts
                .Include(p => p.Photos)
                .ToListAsync();

        }
    }
}
