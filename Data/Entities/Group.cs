using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Entities
{
    [JsonObject(IsReference = true)]
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid UserId { get; set; }
        public string Users { get; set; }
        public ICollection<string> UsersList { get { return Users.Split(';'); } }

        public ICollection<Post> Posts { get; }
    }
}
