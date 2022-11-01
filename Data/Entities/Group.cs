using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public Guid UserId { get; set; }

        public ICollection<Guid> Users { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
