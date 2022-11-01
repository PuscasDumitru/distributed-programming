using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public Guid UserId { get; set; }
        public Group Group { get; set; }
        public int GroupId { get; set; }

        public ICollection<Photo> Photos { get; set; }
    }
}
