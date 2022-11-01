using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementation
{
    public class GroupRepository : GenericRepository<Group>
    {
        public GroupRepository(RepositoryDbContext repositoryContext) : base(repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public async Task<Group> GetGroupByIdAsync(int groupId)
        {
            return await RepositoryContext.Groups
                .Include(p => p.Posts)
                .Include(u => u.Users)
                .SingleOrDefaultAsync(x => x.Id == groupId);
        }

        public async Task<IEnumerable<Group>> GetAllGroupsByUserIdAsync(Guid userId)
        {
            return await RepositoryContext.Groups
                .Include(p => p.Posts)
                .Include(u => u.Users)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetAllGroupsAsync()
        {
            return await RepositoryContext.Groups
                .Include(p => p.Posts)
                .Include(u => u.Users)
                .ToListAsync();
        }
    }
}
