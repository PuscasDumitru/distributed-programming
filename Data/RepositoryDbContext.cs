using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class RepositoryDbContext : DbContext
    {
        public RepositoryDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        //public DbSet<Photo> Photos { get; set; }
    }
}
